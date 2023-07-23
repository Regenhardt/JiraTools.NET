namespace JiraLib.Jira;

using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Text;
using System.Text.Json;

public class JiraSearch
{
    private const string IssueApiUrl = "/rest/api/latest/issue/";
    private const string SearchApiUrl = "/rest/api/latest/search?jql=";
    private const int LengthOfJSessionId = 32;
    private const int LengthOfOAuthToken = 44;

    private HttpClient HttpClient { get; set; }

    /// <summary>
    /// URI to the connected jira instance, e.g. https://jira.example.com.
    /// </summary>
    public string JiraUrl { get; private init; }

    /// <summary>
    /// Creates a new JiraSearch instance. Tests the token if provided.
    /// </summary>
    /// <param name="jiraInstanceUri">URI to your jira instance, e.g. https://jira.example.com.</param>
    /// <param name="token">JSESSIONID from the cookie or personal OAuth token. Used for authentication. Without it, only public data can be read.</param>
    /// <returns>A fully initialized <see cref="JiraSearch"/> instance ready to be used.</returns>
    public static async Task<JiraSearch> CreateAsync(string jiraInstanceUri, string? token)
    {
        var httpClientHandler = new HttpClientHandler();

        var httpClient = new HttpClient(httpClientHandler)
        {
            BaseAddress = new Uri(jiraInstanceUri)
        };

        if (!string.IsNullOrWhiteSpace(token))
        {
            if (token.Length == LengthOfJSessionId)
            {
                // JSessionId
                httpClientHandler.CookieContainer = new CookieContainer();
                httpClientHandler.CookieContainer.Add(new Uri(jiraInstanceUri), new Cookie("JSESSIONID", token));
            }
            else
            {
                // OAuth token
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            // Check if authentication is valid by reading the currently logged in user
            var response = await httpClient.GetAsync("rest/auth/1/session");
            response.EnsureSuccessStatusCode();
        }

        return new JiraSearch(httpClient) { JiraUrl = jiraInstanceUri };
    }

    /// <summary>
    /// Creates a new JiraSearch instance. Authenticates with the given credentials and then discards them. The session id from the login is used for further requests.
    /// </summary>
    /// <param name="jiraInstanceUri">URI to your jira instance, e.g. https://jira.example.com.</param>
    /// <param name="username">Username of an authorized user.</param>
    /// <param name="password">Password for the given username.</param>
    /// <returns>A fully initialized <see cref="JiraSearch"/> instance ready to be used.</returns>
    public static async Task<JiraSearch> CreateAsync(string jiraInstanceUri, string username, string password)
    {
        var httpClientHandler = new HttpClientHandler();
        httpClientHandler.CookieContainer = new CookieContainer();

        var httpClient = new HttpClient(httpClientHandler)
        {
            BaseAddress = new Uri(jiraInstanceUri)
        };

        var loginResponse = await httpClient.PostAsync("rest/auth/1/session",
            new StringContent(JsonSerializer.Serialize(new { username, password }), Encoding.UTF8, "application/json"));
        loginResponse.EnsureSuccessStatusCode();

        var loginInfo = JsonSerializer.Deserialize<LoginResponse>(await loginResponse.Content.ReadAsStringAsync())!;
        var loginToken = loginInfo.Session.Value;

        httpClientHandler.CookieContainer.Add(new Uri(jiraInstanceUri), new Cookie(loginInfo.Session.Name, loginToken));

        return new JiraSearch(httpClient) { JiraUrl = jiraInstanceUri };
    }

    private JiraSearch(HttpClient httpClient)
    {
        HttpClient = httpClient;
        JiraUrl = HttpClient.BaseAddress!.ToString();
    }

    /// <summary>
    /// Gets the list of issues matching the given JQL query.
    /// </summary>
    /// <param name="jqlQuery">Query to pass to jira.</param>
    /// <returns>A list of Issues.</returns>
    /// <exception cref="AuthenticationException">Thrown if the given authentication data is invalid. Create new instance with fresh authentication data if this happens.</exception>
    /// <exception cref="Exception">Thrown if for some other, unexpected reason jira doesn't provide a result. Message will have jira's http status code and reason phrase.</exception>
    public async Task<List<JiraIssue>> GetIssues(string jqlQuery)
    {
        var apiUrl = SearchApiUrl + Uri.EscapeDataString(jqlQuery);
        var response = await SendRequest(apiUrl);

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var searchResult = JsonSerializer.Deserialize<JiraSearchResult>(content)!;

            return searchResult.Issues;
        }

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            throw new AuthenticationException(
                "401 Unauthorized - invalid jira authentication. Please provide up-to-date user/password or JSESSIONID.");
        }

        throw new Exception($"Failed to list issues: {response.StatusCode} {response.ReasonPhrase}");
    }

    public async Task<List<string>> ListIds(string jqlQuery)
        => (await GetIssues(jqlQuery)).Select(issue => issue.Key).ToList();

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        AllowTrailingCommas = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        NumberHandling = JsonNumberHandling.AllowReadingFromString
    };

    /// <summary>
    /// Load an issue by its id.
    /// </summary>
    /// <param name="issueId">ID of the issue to load, e.g. SIERRA-117.</param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<JiraIssue?> GetIssue(string issueId)
    {
        var apiUrl = IssueApiUrl + Uri.EscapeDataString(issueId);
        var response = await SendRequest(apiUrl);

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var issue = JsonSerializer.Deserialize<JiraIssue>(content, SerializerOptions);
            return issue;
        }

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            throw new AuthenticationException(
                "401 Unauthorized - invalid jira authentication. Please provide up-to-date user/password or JSESSIONID.");
        }

        throw new Exception($"Failed to get issue '{issueId}': {response.StatusCode} {response.ReasonPhrase}");
    }

    private async Task<HttpResponseMessage> SendRequest(string apiUrl)
    {
        return await HttpClient.GetAsync(apiUrl);
    }

    /// <summary>
    /// Construct the URI to the issue in the jira web interface. Does not check whether or not the issue actually exists.
    /// </summary>
    /// <param name="issueId">ID of the issue.</param>
    /// <returns>A complete URI for a browser to find the given issue in your jira instance.</returns>
    public string GetIssueUri(string issueId)
    {
        return $"{JiraUrl}/browse/{issueId}";
    }

    public async Task<TimeSpan> GetTotalWorkTime(string username, DateTime fromDate, DateTime toDate)
    {
        var jqlQuery = $"worklogAuthor = {username} and worklogDate >= \"{fromDate:yyyy-MM-dd}\" and worklogDate <= \"{toDate:yyyy-MM-dd}\"";
        var searchUrl = SearchApiUrl + Uri.EscapeDataString(jqlQuery);
        var response = await HttpClient.GetAsync(searchUrl);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var issues = JsonSerializer.Deserialize<JiraSearchResult>(content)!.Issues;

        TimeSpan totalTimeSpent = TimeSpan.Zero;
        foreach (var issue in issues)
        {
            var fields = issue.Fields;
            var worklogs = fields.Worklog["worklogs"]!.AsArray();
            foreach (var worklog in worklogs)
            {
                var timeSpent = TimeSpan.Parse(worklog!["timeSpentSeconds"]!.ToString());
                totalTimeSpent += timeSpent;
            }
        }

        return totalTimeSpent;
    }
}
namespace JiraLib.Services;

using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Models;

/// <summary>
/// Service to search for issues in a jira instance.
/// </summary>
public class JiraSearch
{
    private const string IssueApiUrl = "/rest/api/latest/issue/";
    private const string SearchApiUrl = "/rest/api/latest/search?jql=";
    private const int LengthOfServerJSessionId = 32; // OAuth token is 44
    private const int LengthOfCloudSessionId = 40;

    private static readonly JsonSerializerOptions SerializerOptions = new JsonSerializerOptions
    {
        AllowTrailingCommas = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        NumberHandling = JsonNumberHandling.AllowReadingFromString
    }.Apply(opts => opts.Converters.Add(new DateTimeConverterUsingDateTimeParseAsFallback()));

    private HttpClient HttpClient { get; }

    /// <summary>
    /// URI to the connected jira instance, e.g. https://jira.example.com.
    /// </summary>
    public string JiraUrl { get; private init; }

    private JiraSearch(HttpClient httpClient)
    {
        HttpClient = httpClient;
        JiraUrl = HttpClient.BaseAddress!.ToString();
    }

    private static bool IsSessionId(string token) => token.Length is LengthOfServerJSessionId or LengthOfCloudSessionId;

    /// <summary>
    /// Creates a new JiraSearch instance. Tests the token if provided.
    /// </summary>
    /// <param name="jiraInstanceUri">URI to your jira instance, e.g. https://jira.example.com.</param>
    /// <param name="username">Username or e-mail-address.</param>
    /// <param name="token">JSESSIONID from the cookie or personal OAuth token. Used for authentication. Without it, only public data can be read.</param>
    /// <returns>A fully initialized <see cref="JiraSearch"/> instance ready to be used.</returns>
    public static async Task<JiraSearch> CreateFromTokenAsync(string jiraInstanceUri, string username, string? token)
    {
        var httpClientHandler = new HttpClientHandler();

        var httpClient = new HttpClient(httpClientHandler)
        {
            BaseAddress = new Uri(jiraInstanceUri)
        };

        if (!string.IsNullOrWhiteSpace(token))
        {
            if (IsSessionId(token))
            {
                // JSessionId
                httpClientHandler.CookieContainer = new CookieContainer();
                httpClientHandler.CookieContainer.Add(new Uri(jiraInstanceUri), new Cookie("JSESSIONID", token));
            }
            else
            {
                if(jiraInstanceUri.ToLower().Contains("atlassian.net"))
                {
                    // Jira Cloud needs basic auth
                    token = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{token}"));
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", token);
                }
                else
                {
                    // Jira server uses bearer auth
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
            }

            // Check if authentication is valid by reading the currently logged in user
            var response = await httpClient.GetAsync("rest/auth/latest/session");
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
    public static async Task<JiraSearch> CreateFromPasswordAsync(string jiraInstanceUri, string username, string password)
    {
        var httpClientHandler = new HttpClientHandler();

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
            throw new AuthenticationException(
                "401 Unauthorized - invalid jira authentication. Please provide up-to-date user/password or JSESSIONID.");

        throw new Exception($"Failed to list issues: {response.StatusCode} {response.ReasonPhrase}");
    }

    /// <summary>
    /// Get a list of IDs of issues matching the given JQL query.
    /// </summary>
    /// <param name="jqlQuery"></param>
    /// <returns></returns>
    public async Task<List<string>> ListIds(string jqlQuery)
    {
        return (await GetIssues(jqlQuery)).Select(issue => issue.Key).ToList();
    }

    /// <summary>
    /// Load an issue by its id.
    /// </summary>
    /// <param name="issueId">ID of the issue to load, e.g. SIERRA-117.</param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<JiraIssue?> GetIssue(string issueId)
    {
        var apiUrl = IssueApiUrl + Uri.EscapeDataString(issueId);
        var response = await SendRequest(apiUrl + "?expand=names");

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var issue = JsonSerializer.Deserialize<JiraIssue>(content, SerializerOptions);
            return issue;
        }

        if (response.StatusCode == HttpStatusCode.Unauthorized)
            throw new AuthenticationException(
                "401 Unauthorized - invalid jira authentication. Please provide up-to-date user/password or JSESSIONID.");

        throw new Exception($"Failed to get issue '{issueId}': {response.StatusCode} {response.ReasonPhrase}");
    }

    /// <summary>
    /// Get a list of issues that are linked to the given epic.
    /// </summary>
    /// <param name="epicId">ID of the epic, e.g. SIERRA-117</param>
    /// <param name="epicFieldName">Name of the epic field in the frontend. Depends on the language of your jira instance, e.g. in english it will be "Epic Link", in german "Epic-Verknüpfung". If you already have a <seealso cref="JiraIssue"/>, you can read it from its <seealso cref="JiraIssue.EpicFieldName"/> property.</param>
    /// <returns></returns>
    public async Task<List<JiraIssue>> GetIssuesByEpicLink(string epicId, string epicFieldName)
    {
        return await GetIssues($"'{epicFieldName}' = {epicId}");
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
}

internal class DateTimeConverterUsingDateTimeParseAsFallback : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        Debug.Assert(typeToConvert == typeof(DateTime));

        if (!reader.TryGetDateTime(out var value)) value = DateTime.Parse(reader.GetString()!);

        return value;
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString("dd/MM/yyyy"));
    }
}
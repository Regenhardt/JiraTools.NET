namespace JiraLib.Jira;

internal class LoginResponse
{
    public SessionInfo Session { get; set; }
    public Dictionary<string, object> LoginInfo { get; set; }
}

internal class SessionInfo
{
    public string Name { get; set; }
    public string Value { get; set; }
}
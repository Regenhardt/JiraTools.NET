namespace JiraLib.Jira;

using System.Text.Json.Serialization;

public class JiraStatus
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;

    public bool IsClosed()
    {
        return Name.ToLower() == "closed";
    }

}

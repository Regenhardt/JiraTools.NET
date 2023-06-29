namespace JiraLib.Jira;

using System.Text.Json.Serialization;

public class JiraIssueType
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    public bool IsEpic()
    {
        return Name?.ToLower() == "epic";
    }

    public bool IsSubtask()
    {
        return Name?.ToLower() == "sub-task";
    }

}

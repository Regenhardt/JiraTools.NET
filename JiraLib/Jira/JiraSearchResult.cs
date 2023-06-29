namespace JiraLib.Jira;

using System.Collections.Generic;
using System.Text.Json.Serialization;

public class JiraSearchResult
{
    [JsonPropertyName("issues")]
    public List<JiraIssue> Issues { get; set; }
}

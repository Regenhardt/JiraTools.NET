namespace JiraLib.Jira;

using System.Text.Json.Serialization;

public class JiraIssue
{
    [JsonPropertyName("key")]
    public string Key { get; set; } = null!;

    [JsonPropertyName("fields")] public JiraFields Fields { get; set; } = new();

    public string GetColour() => Fields.Status.Name.ToLower() switch
    {
        "closed" => "green",
        "done" => "green",
        "fertig" => "green",
        "it fertig" => "green",
        "für update bereit" => "green",
        "für update freigegeben" => "green",
        "geschlossen" => "green",
        "in progress" => "yellow",
        "in arbeit" => "yellow",
        "in qm-review" => "blue",
        "zu testen" => "blue",
        "blockiert" => "red",
        _ => "white"
    };
}

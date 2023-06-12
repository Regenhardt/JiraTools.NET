export interface Options {
    User?: string;
    Password?: string;
    Cookie?: string;
    NoAuth?: boolean;
    JiraUrl: string;
    ImageFile?: string;
    Local?: boolean;
    IncludeEpics?: boolean;
    ExcludeLinks?: Set<string>;
    IgnoreClosed?: boolean;
    Includes?: string;
    IssueExcludes?: Set<string>;
    ShowDirections?: Set<string>;
    WalkDirections?: Set<string>;
    Traverse?: boolean;
    WordWrap?: boolean;
    JqlQuery?: string;
    Issues?: string[];
    IncludeSubtasks?: boolean;
    NodeShape?: string;
}
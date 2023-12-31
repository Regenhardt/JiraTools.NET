﻿export interface Options {
    User: string;
    Password?: string;
    Token?: string;
    NoAuth: boolean;
    JiraUrl: string;
    ImageFile?: string;
    Local?: boolean;
    IncludeEpics?: boolean;
    ExcludeLinks?: string[];
    IgnoreClosed?: boolean;
    Includes?: string;
    IssueExcludes?: string[];
    ShowDirections?: string[];
    WalkDirections?: string[];
    Traverse?: boolean;
    WordWrap?: boolean;
    JqlQuery?: string;
    Issues?: string[];
    IncludeSubtasks?: boolean;
    NodeShape?: string;
    OutputFormat?: 'Png' | 'Svg' | 'Dot';
}
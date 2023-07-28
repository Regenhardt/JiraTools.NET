namespace JiraLib.Graph;

internal interface IGraphElement
{
    string GetGraphvizCode(bool wordWrap);
}

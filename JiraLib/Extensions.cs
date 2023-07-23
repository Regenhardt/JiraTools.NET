namespace JiraLib;
using System;

internal static class Extensions
{
    public static T Apply<T>(this T it, Action<T> action)
    {
        action(it);
        return it;
    }
}

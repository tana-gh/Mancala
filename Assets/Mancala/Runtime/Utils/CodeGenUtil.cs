using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace tana_gh.Mancala
{
    public static class CodeGenUtil
    {
        public static string GetTypeName(this Type type)
        {
            return
                (type.DeclaringType == null ? "" : $"{GetTypeName(type.DeclaringType)}.") +
                GetName(type) +
                (type.IsGenericType ? $"<{string.Join(", ", type.GenericTypeArguments.Select(t => t.GetTypeName()))}>" : "");
        }

        public static string GetVarName(this Type type, bool isProperty)
        {
            var name = GetName(type);
            return isProperty ? name : $"_{char.ToLower(name[0])}{name[1..]}";
        }

        public static string GetArrayVarName(this Type type, bool isProperty)
        {
            return $"{type.GetVarName(isProperty)}s";
        }

        public static string GetStoreVarName(this Type type, bool isProperty)
        {
            return $"{type.GetGenericArguments()[0].GetVarName(isProperty)}Store";
        }

        private static string GetName(Type type)
        {
            return Regex.Replace(type.Name, @"(.+\+)*(.+?)(`.+)?", "$2");
        }

        public static string ToLines(this IEnumerable<string> sources, int indent)
        {
            return string.Join("", sources.Select(source => $"{Environment.NewLine}{new string(' ', indent)}{source}"));
        }
    }
}

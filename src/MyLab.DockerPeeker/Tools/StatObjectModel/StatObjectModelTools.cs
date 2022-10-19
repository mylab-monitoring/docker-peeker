using System;
using System.Collections.Generic;
using System.Linq;

namespace MyLab.DockerPeeker.Tools.StatObjectModel
{
    static class StatObjectModelTools
    {
        private static readonly char[] Separators = { '\r', '\n' };

        public static IEnumerable<string> SplitLines(string stringContent)
        {
            return stringContent
                .Split(Separators, StringSplitOptions.RemoveEmptyEntries)
                //.Select(s => s.Trim())
                .Where(s => !string.IsNullOrWhiteSpace(s));
        }
    }
}

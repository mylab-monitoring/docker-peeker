using System;
using System.Collections.Generic;
using System.Linq;

namespace MyLab.DockerPeeker.Tools
{
    static class DockerStatParser
    {
        public static IEnumerable<DockerStatItem> Parse(string input)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));

            return input
                .Split("\n", StringSplitOptions.RemoveEmptyEntries)
                .Select(DockerStatItem.Parse)
                .ToArray() ;
        }
    }
}

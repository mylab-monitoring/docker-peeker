using System;
using System.Collections.Generic;
using System.Linq;
using MyLab.Log;

namespace MyLab.DockerPeeker.Tools.StatObjectModel
{
    public class KeyValueStat : Dictionary<string, long>
    {
        public KeyValueStat()
        {
            
        }
        
        public KeyValueStat(IDictionary<string, long> initial)
            :base(initial)
        {
            
        }

        public static KeyValueStat Parse(string fileContent)
        {
            var dict = StatObjectModelTools.SplitLines(fileContent)
                .Select(l =>
                {
                    var delimiterIndex = l.IndexOf(" ");

                    try
                    {
                        if (delimiterIndex < 0)
                            throw new FormatException("Delimiter not found");
                        if(delimiterIndex == 0)
                            throw new FormatException("Delimiter at the start of line");
                        if (delimiterIndex == l.Length-1)
                            throw new FormatException("Delimiter at the end of line");

                        return (l.Remove(delimiterIndex), long.Parse(l.Substring(delimiterIndex + 1)));
                    }
                    catch(Exception e)
                    {
                        e.AndFactIs("line", l);

                        throw;
                    }
                })
                .ToDictionary(l => l.Item1, l => l.Item2);

            return new KeyValueStat(dict);
        }
    }
}

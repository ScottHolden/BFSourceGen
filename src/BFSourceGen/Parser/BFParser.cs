using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace BFSourceGen
{
    public static class BFParser
    {
        private static readonly Dictionary<char, BFOp> BFOpMap = new Dictionary<char, BFOp>
        {
            { '<', BFOp.Left },
            { '>', BFOp.Right },
            { '+', BFOp.Inc },
            { '-', BFOp.Dec },
            { '.', BFOp.Write },
            { ',', BFOp.Read },
            { '[', BFOp.Loop },
            { ']', BFOp.EndLoop },
        };

        public static (IEnumerable<BFOp> operations, BFTranspilerOptions options) Parse(AdditionalText file)
        {
            List<BFOp> operations = new List<BFOp>();
            BFTranspilerOptions options = new BFTranspilerOptions();

            options.ClassName = Path.GetFileNameWithoutExtension(file.Path);

            foreach (string line in file.GetText()!.Lines.Select(x => x.ToString()))
            {
                if (line.StartsWith("#", StringComparison.OrdinalIgnoreCase))
                {
                    string[] split = line.Substring(1).Split(new char[] { ' ', '\t', '=' }, 2, StringSplitOptions.RemoveEmptyEntries);

                    if (split[0].Equals("memsize", StringComparison.OrdinalIgnoreCase) &&
                        split.Length == 2 && 
                        int.TryParse(split[1].Trim(), out int parsedBufferSize))
                    {
                        options.MemSize = parsedBufferSize;
                    }

                    if (split[0].Equals("memtype", StringComparison.OrdinalIgnoreCase) &&
                        split.Length == 2)
                    {
                        options.MemType = split[1].ToLowerInvariant().Trim();
                    }

                    if (split[0].Equals("eofvalue", StringComparison.OrdinalIgnoreCase) &&
                        split.Length == 2)
                    {
                        options.EoFValue = split[1].Trim();
                    }

                    if ((split[0].Equals("optimize", StringComparison.OrdinalIgnoreCase) ||
                            split[0].Equals("optimise", StringComparison.OrdinalIgnoreCase)) &&
                        (split.Length == 1 ||
                            (split.Length == 2 && bool.TryParse(split[1].Trim(), out bool shouldOptimize)
                                && shouldOptimize)))
                    {
                        options.Optimize = true;
                    }

                    if (split[0].Equals("class", StringComparison.OrdinalIgnoreCase) &&
                        (split.Length == 1 ||
                            (split.Length == 2 && bool.TryParse(split[1].Trim(), out bool shouldBeClass)
                                && shouldBeClass)))
                    {
                        options.Class = true;
                    }

                    if (split[0].Equals("classname", StringComparison.OrdinalIgnoreCase) &&
                        split.Length == 2)
                    {
                        options.ClassName = split[1].Trim();
                    }
                }
                else
                {
                    foreach (char c in line)
                    {
                        if (c == '#')
                        {
                            // Inline comment, skip the line
                            break;
                        }
                        else if (BFOpMap.ContainsKey(c))
                        {
                            BFOp op = BFOpMap[c];

                            operations.Add(op);
                        }
                    }
                }
            }

            // Validate operations

            if (operations.Count(x => x == BFOp.Loop) != operations.Count(x => x == BFOp.EndLoop))
            {
                throw new Exception("Unbalanced loop, did you miss a ]?");
            }

            // Validate options

            options.ValidateWithExceptions();

            return (operations, options);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace BFSourceGen
{
	public static class BFParser
    {
        private const int DefaultMemSize = 1024;
        private const string DefaultMemType = "int";
        private const string DefaultEOFValue = "0";

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

        public static (IEnumerable<BFOp> operations, int memSize, string memType, string eofValue) Parse(AdditionalText file)
        {
            List<BFOp> opperations = new List<BFOp>();
            int memSize = DefaultMemSize;
            string memType = DefaultMemType;
            string eofValue = DefaultEOFValue;

            foreach (string line in file.GetText()!.Lines.Select(x => x.ToString()))
            {
                if (line.StartsWith("#memsize", StringComparison.OrdinalIgnoreCase))
                {
                    string[] split = line.Split(new char[] { ' ', '\t', '=' }, 2, StringSplitOptions.RemoveEmptyEntries);
                    if (split.Length == 2 && int.TryParse(split[1].Trim(), out int parsedBufferSize))
                    {
                        memSize = parsedBufferSize;
                    }
                }
                else if (line.StartsWith("#memtype", StringComparison.OrdinalIgnoreCase))
                {
                    string[] split = line.Split(new char[] { ' ', '\t', '=' }, 2, StringSplitOptions.RemoveEmptyEntries);
                    if (split.Length == 2)
                    {
                        memType = split[1].ToLowerInvariant().Trim();
                    }
                }
                else if (line.StartsWith("#eofvalue", StringComparison.OrdinalIgnoreCase))
                {
                    string[] split = line.Split(new char[] { ' ', '\t', '=' }, 2, StringSplitOptions.RemoveEmptyEntries);
                    if (split.Length == 2)
                    {
                        eofValue = split[1].Trim();
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

                            opperations.Add(op);
                        }
                    }
                }
            }

            return (opperations, memSize, memType, eofValue);
        }
    }
}
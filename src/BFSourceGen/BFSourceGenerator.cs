using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace BFTranspile
{
    [Generator]
    public class BFSourceGenerator : ISourceGenerator
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
            { ']', BFOp.Check },
        };
        private enum BFOp
        {
            Left,
            Right,
            Inc,
            Dec,
            Write,
            Read,
            Loop,
            Check
        }

        public void Execute(SourceGeneratorContext context)
        {
            // Parse

			AdditionalText bfFile = context.AdditionalFiles.First(x => x.Path.EndsWith(".bf"));
            int memSize = 1024;
            bool needsInput = false;
            List<BFOp> opperations = new List<BFOp>();
            foreach (string line in bfFile.GetText()!.Lines.Select(x=>x.ToString()))
            {
                if(line.StartsWith("#memsize", StringComparison.OrdinalIgnoreCase))
                {
                    string[] split = line.Split(new char[] { ' ', '\t', '=' }, 2, StringSplitOptions.RemoveEmptyEntries);
                    if(split.Length == 2 && int.TryParse(split[1].Trim(), out int parsedBufferSize))
                    {
                        memSize = parsedBufferSize;
                    }
                }
                else
                {
                    foreach(char c in line)
                    {
                        if (BFOpMap.ContainsKey(c))
                        {
                            opperations.Add(BFOpMap[c]);
                        }
					}
				}
            }

            // Validate

            if(opperations.Count(x=>x == BFOp.Loop) != opperations.Count(x => x == BFOp.Check))
            {
                throw new Exception("Unbalanced loop, did you miss a ]?");
			}

            // Output

            StringBuilder sb = new StringBuilder(@"using System; public static class BFProgram{ public static void Main(){");

            sb.Append("long memIndex = 0;");
            sb.Append($"byte[] mem = new byte[{memSize}];");

            if (needsInput)
            {
                sb.Append("Console.Write(\"Input: \");");
                sb.Append("string input = Console.ReadLine();");
                sb.Append("int inputIndex = 0;");
			}
            foreach(BFOp op in opperations)
            {
                sb.Append(op switch
				{
					BFOp.Left => "memIndex = memIndex > 0 ? memIndex - 1 : mem.Length - 1;",
					BFOp.Right => "memIndex = (memIndex+1) % mem.Length;",
					BFOp.Inc => "mem[memIndex]++;",
					BFOp.Dec => "mem[memIndex]--;",
					BFOp.Write => "Console.Write((char)mem[memIndex]);",
					BFOp.Read => "mem[memIndex] = (byte)input[inputIndex++];",
					BFOp.Loop => "while(true){",
					BFOp.Check => "if(mem[memIndex] == 0) break;}",
					_ => throw new Exception("Unknown OpCode")
				});
			}
            sb.Append(@"}}");

            context.AddSource("BFProgram.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
        }

        public void Initialize(InitializationContext context)
        {
        }
    }
}

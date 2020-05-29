using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace BFSourceGen
{
    public static class BFTranspiler
    {
        public static string Transpile(IEnumerable<BFOp> operations, int memSize, string memType, string eofValue)
        {
            StringBuilder sb = new StringBuilder(@"using System; public static class BFProgram{ public static void Main(){");

            sb.Append("long memIndex = 0;");
            sb.Append($"{memType}[] mem = new {memType}[{memSize}];");

            if (operations.Any(x => x == BFOp.Read))
            {
                sb.Append("Console.Write(\"Input: \");");
                sb.Append("char[] input = Console.ReadLine().ToCharArray();");
                sb.Append("int inputIndex = 0;");
            }
            foreach (BFOp op in operations)
            {
                sb.Append(op switch
                {
                    BFOp.Left => "memIndex = memIndex > 0 ? memIndex - 1 : mem.Length - 1;",
                    BFOp.Right => "memIndex = (memIndex+1) % mem.Length;",
                    BFOp.Inc => "mem[memIndex]++;",
                    BFOp.Dec => "mem[memIndex]--;",
                    BFOp.Write => "Console.Write((char)mem[memIndex]);",
                    BFOp.Read => $"mem[memIndex] = inputIndex<input.Length?(({memType})input[inputIndex++]):" +
                                   (eofValue.Equals("same", StringComparison.OrdinalIgnoreCase) ?
                                    "mem[memIndex];" :
                                    $"(({memType}){eofValue});"),
                    BFOp.Loop => "while(mem[memIndex] != 0){",
                    BFOp.EndLoop => "}",
                    _ => throw new Exception("Unknown OpCode")
                });
            }
            sb.Append(@"}}");

            return sb.ToString();
        }

    }
}
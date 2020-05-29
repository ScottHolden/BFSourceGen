using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace BFSourceGen
{
    public class BFTranspiler
    {
        private readonly BFTranspilerOptions _options;

        public BFTranspiler(BFTranspilerOptions options)
        {
            _options = options;
        }

        public string Transpile(IEnumerable<BFOp> operations)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append($"using System; using System.Text;");
            sb.Append($"namespace {_options.Namespace}{{");
            sb.Append($"public static class {_options.ClassName}{{");

            bool takesInput = operations.Any(x => x == BFOp.Read);
            bool hasOutput = operations.Any(x => x == BFOp.Write);

            string returnType = hasOutput ? "string" : "void";
            string parameters = takesInput ? "string input" : "";

            if (_options.Class)
            {
                sb.Append($"public static {returnType} Invoke({parameters}){{");
            }
            else
            {
                sb.Append("public static void Main(){");
            }

            sb.Append("long memIndex = 0;");
            sb.Append($"{_options.MemType}[] mem = new {_options.MemType}[{_options.MemSize}];");

            if (takesInput)
            {
                sb.Append("int inputIndex = 0;");

                if (_options.Class)
                {
                    sb.Append("char[] inputBuffer = input.ToCharArray();");
                }
                else
                {
                    sb.Append("Console.Write(\"Input: \");");
                    sb.Append("char[] inputBuffer = Console.ReadLine().ToCharArray();");
                }
            }

            if (hasOutput)
            {
                if (_options.Class)
                {
                    sb.Append("StringBuilder outputBuilder = new StringBuilder();");
                    sb.Append("Action<char> write = c => outputBuilder.Append(c);");
                }
                else
                {
                    sb.Append("Action<char> write = c => Console.Write(c);");
                }
            }

            if (_options.Optimize)
            {
                OptimizedTranspiler(operations, sb);
            }
            else
            {
                BasicTranspiler(operations, sb);
            }

            if (hasOutput && _options.Class)
            {
                sb.Append("return outputBuilder.ToString();");
            }

            sb.Append(@"}}}");

            return sb.ToString();
        }

        private void BasicTranspiler(IEnumerable<BFOp> operations, StringBuilder builder)
        {
            foreach (BFOp op in operations)
            {
                builder.Append(OpToCode(op));
            }
        }

        private string OpToCode(BFOp op) => op switch
        {
            BFOp.Left => "memIndex = memIndex > 0 ? memIndex - 1 : mem.Length - 1;",
            BFOp.Right => "memIndex = (memIndex+1) % mem.Length;",
            BFOp.Inc => "mem[memIndex]++;",
            BFOp.Dec => "mem[memIndex]--;",
            BFOp.Write => "write((char)mem[memIndex]);",
            BFOp.Read => $"mem[memIndex] = inputIndex<inputBuffer.Length?(({_options.MemType})inputBuffer[inputIndex++]):" +
                           (_options.EoFValue.Equals("same", StringComparison.OrdinalIgnoreCase) ?
                            "mem[memIndex];" :
                            $"(({_options.MemType}){_options.EoFValue});"),
            BFOp.Loop => "while(mem[memIndex] != 0){",
            BFOp.EndLoop => "}",
            _ => throw new Exception("Unknown OpCode")
        };

        private void OptimizedTranspiler(IEnumerable<BFOp> operations, StringBuilder builder)
        {
            BFOp current = BFOp.EndLoop;
            int count = 0;
            foreach (BFOp op in operations)
            {
                if (IsOptimisedOp(op))
                {
                    if (current == op)
                    {
                        count++;
                    }
                    else
                    {
                        if (count > 0)
                        {
                            OptimizedWrite(current, count, builder);
                        }
                        current = op;
                        count = 1;
                    }
                }
                else
                {
                    if (count > 0)
                    {
                        OptimizedWrite(current, count, builder);
                        count = 0;
                    }
                    OptimizedWrite(op, 1, builder);
                }
            }
            if (count > 0)
            {
                OptimizedWrite(current, count, builder);
            }
        }

        private bool IsOptimisedOp(BFOp op) =>
            op == BFOp.Left ||
            op == BFOp.Right ||
            op == BFOp.Inc ||
            op == BFOp.Dec;

        private void OptimizedWrite(BFOp op, int count, StringBuilder builder)
        {
            builder.Append(op switch
            {
                BFOp.Left => $"memIndex = (memIndex - {count}) % mem.Length;",
                BFOp.Right => $"memIndex = (memIndex + {count}) % mem.Length;",
                BFOp.Inc => $"mem[memIndex] += {count};",
                BFOp.Dec => $"mem[memIndex] -= {count};",
                _ => OpToCode(op)
            });
        }
    }
}
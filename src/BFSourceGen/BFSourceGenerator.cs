using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace BFSourceGen
{
	[Generator]
	public class BFSourceGenerator : ISourceGenerator
    {
        public void Execute(SourceGeneratorContext context)
        {
            // Parse

            AdditionalText bfFile = context.AdditionalFiles.FirstOrDefault(x => x.Path.EndsWith(".bf"));

            if (bfFile == null)
            {
                throw new Exception("No .bf files found, did you mark it as additional?");
            }

            (IEnumerable<BFOp> operations, int memSize, string memType, string eofValue) = BFParser.Parse(bfFile);


            // Validate

            if (!operations.Any())
            {
                throw new Exception("Can't see any valid BF op's");
            }

            if (memSize <= 0)
            {
                throw new Exception("Invalid memSize!");
            }

            if (memType != "byte" && memType != "int" && memType != "long")
            {
                throw new Exception("Invalid memType! Valid types are byte, int, long");
            }

            if (!byte.TryParse(eofValue, out byte _) &&
                !int.TryParse(eofValue, out int _) &&
                !long.TryParse(eofValue, out long _) &&
                !eofValue.Equals("same", StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception("Invalid eofValue! Does it match the memType?");
            }

            if (operations.Count(x => x == BFOp.Loop) != operations.Count(x => x == BFOp.EndLoop))
            {
                throw new Exception("Unbalanced loop, did you miss a ]?");
            }

            // Output

            string csFile = BFTranspiler.Transpile(operations, memSize, memType, eofValue);

            context.AddSource("BFProgram.cs", SourceText.From(csFile, Encoding.UTF8));
        }

        public void Initialize(InitializationContext context)
        {
        }
    }
}
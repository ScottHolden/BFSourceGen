using System;
using System.Collections.Generic;
using System.IO;
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
            foreach (AdditionalText bfFile in context.AdditionalFiles.Where(x => x.Path.EndsWith(".bf")))
            {
                try
                {
                    (IEnumerable<BFOp> operations, BFTranspilerOptions options) = BFParser.Parse(bfFile);

                    if (!operations.Any())
                    {
                        continue;
                    }

                    BFTranspiler btf = new BFTranspiler(options);

                    string csFile = btf.Transpile(operations);

                    context.AddSource(options.ClassName + ".cs", SourceText.From(csFile, Encoding.UTF8));
                }
                catch (Exception e)
                {
                    context.ReportDiagnostic(
                        Diagnostic.Create(
                            new DiagnosticDescriptor("P0T4T0", "BFError", "Error when transpiling BF: {0}", "BF.Transpile", DiagnosticSeverity.Error, true),
                            Location.Create(Path.GetFileName(bfFile.Path), new TextSpan(), new LinePositionSpan()),
                            e.Message));
                }
            }
        }

        public void Initialize(InitializationContext context)
        {
        }
    }
}
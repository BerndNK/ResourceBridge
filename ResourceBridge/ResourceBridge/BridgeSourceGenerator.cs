using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using System.Diagnostics;
using System.IO;
using System.Text;
using Microsoft.CodeAnalysis.Text;

namespace ResourceBridge
{
    [Generator]
    public class BridgeSourceGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            //Debugger.Launch();
            //throw new NotImplementedException();
        }

        public void Execute(GeneratorExecutionContext context)
        {
            //Debugger.Launch();
            var metadata2 = context.Compilation.Assembly.GetAttributes()
                .Where(x => x.AttributeClass?.Name == nameof(System.Reflection.AssemblyMetadataAttribute) &&
                            Microsoft.CodeAnalysis.CSharp.SyntaxFacts.IsValidIdentifier((string)x.ConstructorArguments[0].Value))
                .ToDictionary(x => (string)x.ConstructorArguments[0].Value, x => (string)x.ConstructorArguments[1].Value);

            var factory = new StringFactory();
            var reader = new ResourceFileReader();
            var files = FilesToProcess(context);
            foreach (var file in files)
            {
                if (context.CancellationToken.IsCancellationRequested)
                    return;

                var fileName = Path.GetFileNameWithoutExtension(file.Path).Split('.').First();
                var metadata = new StringFactory.ResourceGenerationMetadata { FileName = fileName, Namespace = "BridgeSource" };

                var content = reader.Read(file.GetText(context.CancellationToken)?.ToString() ?? string.Empty);
                var group = factory.CreateGroup(content);

                var source = factory.GenerateSource(group, metadata);

                if (!context.CancellationToken.IsCancellationRequested)
                    context.AddSource(fileName, SourceText.From(source, Encoding.UTF8));
            }
        }

        static IEnumerable<AdditionalText> FilesToProcess(GeneratorExecutionContext context)
        {
            foreach (AdditionalText file in context.AdditionalFiles)
            {
                if (Path.GetExtension(file.Path).Equals(".resx", StringComparison.OrdinalIgnoreCase))
                {
                    yield return file;
                }
            }
        }

    }
}

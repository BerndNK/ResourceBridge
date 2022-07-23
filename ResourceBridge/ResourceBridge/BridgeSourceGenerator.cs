using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
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
        }

        public void Execute(GeneratorExecutionContext context)
        {
            try
            {
                var factory = new StringFactory();
                var reader = new ResourceFileReader();
                var files = FilesToProcess(context);
                foreach (var file in files)
                {
                    if (context.CancellationToken.IsCancellationRequested)
                        return;

                    var fileName = Path.GetFileNameWithoutExtension(file.Path).Split('.').First();
                    var metadata = new ResourceGenerationMetadata(fileName, "BridgeSource");

                    var content = reader.Read(file.GetText(context.CancellationToken)?.ToString() ?? string.Empty);
                    var group = factory.CreateGroup(content);

                    var source = factory.GenerateSource(group, metadata);

                    if (!context.CancellationToken.IsCancellationRequested)
                        context.AddSource(fileName, SourceText.From(source, Encoding.UTF8));
                }
            }
            catch (Exception)
            {
                // ignored
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

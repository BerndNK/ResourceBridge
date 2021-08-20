namespace ResourceBridge
{
    internal sealed class ResourceGenerationMetadata
    {
        public string FileName { get; }

        public string Namespace { get; }

        public ResourceGenerationMetadata(string fileName, string @namespace)
        {
            FileName = fileName;
            Namespace = @namespace;
        }
    };
}
using System.Collections.Generic;
using System.Xml;

namespace ResourceBridge
{
    internal class ResourceFileReader
    {
        public IEnumerable<TextResource> Read(string xmlContent)
        {
            var doc = new XmlDocument();
            doc.LoadXml(xmlContent);
            var childNodes = doc.DocumentElement?.ChildNodes;
            if (childNodes == null)
                yield break;

            foreach (XmlNode xmlNode in childNodes)
            {
                if (xmlNode.Attributes == null || xmlNode.Name != "data")
                    continue;

                var name = xmlNode.Attributes["name"]?.Value;
                var value = ResolveValue(xmlNode);

                if(name == null)
                    continue;
                
                yield return new TextResource(name, value);
            }
        }

        private string ResolveValue(XmlNode fromName)
        {
            foreach (XmlNode node in fromName.ChildNodes)
            {
                if (node.Name == "value")
                {
                    foreach (XmlNode childNode in node.ChildNodes)
                    {
                        return childNode.Value;
                    }
                }
            }

            return string.Empty;
        }
    }

    internal class TextResource
    {
        public string Name { get; }

        public string Value { get; }

        public TextResource(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}

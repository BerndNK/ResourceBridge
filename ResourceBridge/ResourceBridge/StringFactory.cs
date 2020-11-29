using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ResourceBridge
{
    partial class StringFactory
    {
        public struct ResourceGenerationMetadata
        {
            public string FileName { get; set; }

            public string Namespace { get; set; }

        };

        public string GenerateSource(Group forGroup, ResourceGenerationMetadata metadata)
        {
            var sb = new StringBuilder(FileTemplate);
            
            Append(sb, GenerateClass(forGroup, metadata), Content, 1);
            Append(sb, GenerateInterface(forGroup, metadata), InterfaceContent, 1);
            
            Replace(sb, Namespace, metadata.Namespace);

            return sb.ToString();
        }

        public string GenerateClass(Group forGroup, ResourceGenerationMetadata metadata, int currentLevel = 0)
        {
            var sb = new StringBuilder(ClassTemplate);
            var isRoot = string.IsNullOrEmpty(forGroup.CommonKey);

            string modifier;
            var className = NameFor(forGroup, metadata.FileName);
            var inherits = $" : {InterfaceName(forGroup, metadata.FileName)}";

            if (isRoot)
            {
                modifier = "public sealed";
            }
            else
            {
                modifier = "private";
                className = $"{className}Class";
            }

            AddClassProperties(sb, forGroup, metadata, currentLevel + 1);

            var subClassBuilder = new StringBuilder();
            foreach (var subGroup in forGroup.SubGroups)
            {
                subClassBuilder.AppendLine(GenerateClass(subGroup, metadata, currentLevel));
            }

            Append(sb, subClassBuilder.ToString(), SubClasses, currentLevel + 1);

            Replace(sb, ClassModifier, modifier);
            Replace(sb, FileName, metadata.FileName);
            Replace(sb, TypeName, className);
            Replace(sb, Inherits, inherits);

            return sb.ToString();
        }

        private void AddClassProperties(StringBuilder classBuilder, Group forGroup, ResourceGenerationMetadata metadata, int depth)
        {
            var subPropertiesBuilder = new StringBuilder();
            var entriesBuilder = new StringBuilder();
            var initializerBuilder = new StringBuilder();
            var propertyNames = new HashSet<string>();

            foreach (var group in forGroup.SubGroups)
            {
                var singleBuilder = new StringBuilder(SubClassPropertyTemplate);

                Replace(singleBuilder, PropertyType, InterfaceName(group, metadata.FileName));

                var name = NameFor(@group, metadata.FileName);
                Replace(singleBuilder, PropertyName, name);
                propertyNames.Add(name);

                subPropertiesBuilder.AppendLine(singleBuilder.ToString());
                initializerBuilder.AppendLine($"{name} = new {name}Class(stringLocalizer);");
            }


            foreach (var entry in forGroup.Entries)
            {
                var parameter = ResolveParameter(entry);
                StringBuilder singleBuilder;
                if (parameter.Any())
                {
                    singleBuilder = new StringBuilder(TextPropertyMethodTemplate);
                    Replace(singleBuilder, MethodParameter, PropertyMethodParameter(entry));
                    Append(singleBuilder, MethodImplementation(entry), MethodContent, depth);
                }
                else
                {
                    singleBuilder = new StringBuilder(TextPropertyTemplate);
                }

                var name = entry.Name.Split('.').Last();
                if (propertyNames.Contains(name))
                {
                    name = $"{name}String";
                }

                Replace(singleBuilder, StringLocalizerKey, entry.Name);

                Replace(singleBuilder, PropertyName, name);
                entriesBuilder.AppendLine(singleBuilder.ToString());
            }

            Append(classBuilder, subPropertiesBuilder.ToString(), SubClassProperties, depth);
            Append(classBuilder, entriesBuilder.ToString(), Properties, depth);
            Append(classBuilder, initializerBuilder.ToString(), PropertyInitializer, depth);
        }

        private string MethodImplementation(TextResource entry)
        {
            var sb = new StringBuilder();
            var parameter = ResolveParameter(entry);
            foreach (var singleParameter in parameter)
            {
                sb.AppendLine($@"sb.Replace(""{singleParameter}"", {singleParameter});");
            }

            return sb.ToString();
        }

        public string GenerateInterface(Group forGroup, ResourceGenerationMetadata metadata, int currentLevel = 0)
        {
            var sb = new StringBuilder(InterfaceTemplate);

            var interfaceName = InterfacePrefix + NameFor(forGroup, metadata.FileName);

            AddInterfaceProperties(sb, forGroup, metadata, currentLevel + 1);

            var subInterfaceBuilder = new StringBuilder();
            foreach (var subGroup in forGroup.SubGroups)
            {
                subInterfaceBuilder.AppendLine(GenerateInterface(subGroup, metadata, currentLevel));
            }

            Append(sb, subInterfaceBuilder.ToString(), SubInterfaces, currentLevel + 1);

            Replace(sb, TypeName, interfaceName);

            return sb.ToString();
        }

        private void AddInterfaceProperties(StringBuilder interfaceBuilder, Group forGroup, ResourceGenerationMetadata metadata, int depth)
        {
            var subPropertiesBuilder = new StringBuilder();
            var propertyNames = new HashSet<string>();

            foreach (var group in forGroup.SubGroups)
            {
                var singleBuilder = new StringBuilder(InterfacePropertyTemplate);

                var name = NameFor(@group, metadata.FileName);
                Replace(singleBuilder, PropertyType, $"{InterfacePrefix}{name}");
                Replace(singleBuilder, PropertyName, name);
                propertyNames.Add(name);

                subPropertiesBuilder.AppendLine(singleBuilder.ToString());
            }

            foreach (var entry in forGroup.Entries)
            {
                var parameter = ResolveParameter(entry);
                StringBuilder singleBuilder;
                if (parameter.Any())
                {
                    singleBuilder = new StringBuilder(InterfacePropertyMethodTemplate);
                    Replace(singleBuilder, MethodParameter, PropertyMethodParameter(entry));
                }
                else
                {
                    singleBuilder = new StringBuilder(InterfacePropertyTemplate);
                }
                
                var name = entry.Name.Split('.').Last();
                if (propertyNames.Contains(name))
                {
                    name = $"{name}String";
                }

                
                Replace(singleBuilder, PropertyType, "string");

                Replace(singleBuilder, PropertyName, name);
                subPropertiesBuilder.AppendLine(singleBuilder.ToString());
            }

            Append(interfaceBuilder, subPropertiesBuilder.ToString(), InterfaceProperties, depth);
        }

        private HashSet<string> ResolveParameter(TextResource resource)
        {
            var parameter = new HashSet<string>();
            var regex = new Regex(@"{(\w+)}"); // finds all {word} constructs
            
            foreach (Match match in regex.Matches(resource.Value))
            {
                parameter.Add(match.Groups[1].Value);
            }

            return parameter;
        }

        private string PropertyMethodParameter(TextResource resource) => string.Join(", ", ResolveParameter(resource).Select(p => $"string {p}"));

        private string NameFor(Group forGroup, string fileName) => new[] { fileName }.Concat(forGroup.CommonKey.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries)).Last();

        private string InterfaceName(Group fromGroup, string fileName)
        {
            var allParts = new[] { fileName }.Concat(fromGroup.CommonKey.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries));

            return $"{InterfacePrefix}{string.Join($".{InterfacePrefix}", allParts)}";
        }

        private void Append(StringBuilder target, string toAppend, string withKey, int depth)
        {
            var tempBuilder = new StringBuilder();

            using var reader = new StringReader(toAppend);
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                var whitespace = string.Join("", Enumerable.Repeat("    ", depth));
                tempBuilder.AppendLine(whitespace + line);
            }

            Replace(target, withKey, tempBuilder.ToString());
        }

        private void Replace(StringBuilder sb, string key, string value) => sb.Replace($"{{{key}}}", value);

        public Group CreateGroup(IEnumerable<TextResource> resources, string commonKey = "")
        {
            var elementsForThisLevel = new List<TextResource>();
            var elementsForBelowLevel = new List<TextResource>();

            foreach (var textResource in resources)
            {
                if (string.IsNullOrWhiteSpace(commonKey))
                {
                    var hasDots = textResource.Name.Contains('.');
                    if (hasDots)
                        elementsForBelowLevel.Add(textResource);
                    else
                        elementsForThisLevel.Add(textResource);
                }
                else
                {
                    var hasCommonKey = textResource.Name.StartsWith(commonKey);
                    if (hasCommonKey)
                    {
                        var substringPosition = Math.Min(textResource.Name.Length, commonKey.Length);
                        var subStr = textResource.Name.Substring(substringPosition);

                        if (subStr.Contains('.'))
                            elementsForBelowLevel.Add(textResource);
                        else
                            elementsForThisLevel.Add(textResource);
                    }
                }
            }

            var dotsNow = commonKey.Count(c => c == '.') + 1;

            var subGroups = elementsForBelowLevel
                .GroupBy(e => string.Join(".", e.Name.Split('.').Take(dotsNow)))
                .Select(g => CreateGroup(g, g.Key + "."))
                .ToList();

            return new Group(commonKey, subGroups, elementsForThisLevel);
        }
    }


    internal class Group
    {
        public string CommonKey { get; }

        public IReadOnlyList<Group> SubGroups { get; }

        public IReadOnlyList<TextResource> Entries { get; }

        public Group(string commonKey, IReadOnlyList<Group> subGroups, IReadOnlyList<TextResource> entries)
        {
            CommonKey = commonKey;
            SubGroups = subGroups;
            Entries = entries;
        }
    }
}

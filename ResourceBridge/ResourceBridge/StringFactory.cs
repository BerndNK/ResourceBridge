using System;
using System.Collections.Generic;
using System.Linq;

namespace ResourceBridge
{
    class StringFactory
    {
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

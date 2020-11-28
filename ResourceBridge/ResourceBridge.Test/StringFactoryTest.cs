using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace ResourceBridge.Test
{
    public class StringFactoryTest
    {
        [Test]
        public void EntriesWithNoEntriesInParentGroup_StillCreatesGroups()
        {
            var entries = new List<TextResource>
            {
                new TextResource("A.B.D", "A.B.D"),
                new TextResource("A", "A"),
                new TextResource("A.B.D.E", "A.B.D.E"),
            };

            var factory = new StringFactory();
            var group = factory.CreateGroup(entries);

            var lowestElement = group.SubGroups.First().SubGroups.First().SubGroups.First();
            Assert.That(lowestElement.CommonKey, Is.EqualTo("A.B.D."));
            Assert.That(lowestElement.Entries.First().Value, Is.EqualTo("A.B.D.E"));

        }
    }
}

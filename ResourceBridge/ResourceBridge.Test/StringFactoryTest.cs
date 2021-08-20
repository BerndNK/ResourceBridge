using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace ResourceBridge.Test
{
    public class StringFactoryTest
    {
        private StringFactory _uut = new StringFactory();

        [SetUp]
        public void Setup()
        {
            _uut = new StringFactory();
        }

        [Test]
        public void EntriesWithNoEntriesInParentGroup_StillCreatesGroups()
        {
            var entries = new List<TextResource>
            {
                new TextResource("A.B.D", "A.B.D"),
                new TextResource("A", "A"),
                new TextResource("A.B.D.E", "A.B.D.E"),
            };

            var group = _uut.CreateGroup(entries);

            var lowestElement = group.SubGroups.First().SubGroups.First().SubGroups.First();
            Assert.That(lowestElement.CommonKey, Is.EqualTo("A.B.D."));
            Assert.That(lowestElement.Entries.First().Value, Is.EqualTo("A.B.D.E"));

        }

        [Test]
        public void GeneratingSource_FromValidGroup_ResultsInValidSource()
        {
            var entries = new List<TextResource>
            {
                new TextResource("A.B.D", "A.B.D"),
                new TextResource("A", "A"),
                new TextResource("A.B.D.E", "A.B.D.E"),
            };

            var group = _uut.CreateGroup(entries);
            var metadata = new ResourceGenerationMetadata("Test", "TestNamespace");
            var source = _uut.GenerateSource(group, metadata);
            
            Assert.That(source, Is.EqualTo(GeneratingSourceFromValidGroupResultsInValidSourceExpectedResult));
        }

        [Test]
        public void GeneratingSource_WithParameterizedValues_ResultsInMethods()
        {
            var entries = new List<TextResource>
            {
                new TextResource("A.B.D", "A.B.D"),
                new TextResource("A", "A{param1}{param2}{param1}"),
                new TextResource("A.B.D.E", "A.B.D.E"),
            };

            var group = _uut.CreateGroup(entries);
            var metadata = new ResourceGenerationMetadata ("Test", "TestNamespace");
            var source = _uut.GenerateSource(group, metadata);

            Assert.That(source, Is.EqualTo(GeneratingSourceWithParameterizedValuesResultsInMethodsExpectedResult));
        }

        #region ExpectedResults

        private const string GeneratingSourceFromValidGroupResultsInValidSourceExpectedResult = @"// GENERATED CLASS
using Microsoft.Extensions.Localization;
using System.Text;

namespace TestNamespace
{
    public interface ILocalizedTest
    {
        ILocalizedA A { get; }
        string AString { get; }
        
        public interface ILocalizedA
        {
            ILocalizedB B { get; }
            
            public interface ILocalizedB
            {
                ILocalizedD D { get; }
                string DString { get; }
                
                public interface ILocalizedD
                {
                    string E { get; }
                    
                
                }
            
            }
        
        }
    
    }

    public sealed class Test : ILocalizedTest
    {
        private readonly IStringLocalizer<Test> _stringLocalizer;
    
        public ILocalizedTest.ILocalizedA A { get; }
    
        public string AString => _stringLocalizer[""A""] as string ?? ""A"";
    
    
        public Test(IStringLocalizer<Test> stringLocalizer)
        {
            _stringLocalizer = stringLocalizer;
                A = new AClass(stringLocalizer);
    
        }
    
        private class AClass : ILocalizedTest.ILocalizedA
        {
            private readonly IStringLocalizer<Test> _stringLocalizer;
        
            public ILocalizedTest.ILocalizedA.ILocalizedB B { get; }
        
        
        
            public AClass(IStringLocalizer<Test> stringLocalizer)
            {
                _stringLocalizer = stringLocalizer;
                    B = new BClass(stringLocalizer);
        
            }
        
            private class BClass : ILocalizedTest.ILocalizedA.ILocalizedB
            {
                private readonly IStringLocalizer<Test> _stringLocalizer;
            
                public ILocalizedTest.ILocalizedA.ILocalizedB.ILocalizedD D { get; }
            
                public string DString => _stringLocalizer[""A.B.D""] as string ?? ""A.B.D"";
            
            
                public BClass(IStringLocalizer<Test> stringLocalizer)
                {
                    _stringLocalizer = stringLocalizer;
                        D = new DClass(stringLocalizer);
            
                }
            
                private class DClass : ILocalizedTest.ILocalizedA.ILocalizedB.ILocalizedD
                {
                    private readonly IStringLocalizer<Test> _stringLocalizer;
                
                
                    public string E => _stringLocalizer[""A.B.D.E""] as string ?? ""A.B.D.E"";
                
                
                    public DClass(IStringLocalizer<Test> stringLocalizer)
                    {
                        _stringLocalizer = stringLocalizer;
                        
                    }
                
                
                }
            
            }
        
        }
    
    }

}";

        private const string GeneratingSourceWithParameterizedValuesResultsInMethodsExpectedResult =
                @"// GENERATED CLASS
using Microsoft.Extensions.Localization;
using System.Text;

namespace TestNamespace
{
    public interface ILocalizedTest
    {
        ILocalizedA A { get; }
        string AString (string param1, string param2);
        
        public interface ILocalizedA
        {
            ILocalizedB B { get; }
            
            public interface ILocalizedB
            {
                ILocalizedD D { get; }
                string DString { get; }
                
                public interface ILocalizedD
                {
                    string E { get; }
                    
                
                }
            
            }
        
        }
    
    }

    public sealed class Test : ILocalizedTest
    {
        private readonly IStringLocalizer<Test> _stringLocalizer;
    
        public ILocalizedTest.ILocalizedA A { get; }
    
        public string AString(string param1, string param2) {
            var sb = new StringBuilder(_stringLocalizer[""A""]);
            sb.Replace(""{param1}"", param1);
            sb.Replace(""{param2}"", param2);
        
             return sb.ToString();
        }
    
    
        public Test(IStringLocalizer<Test> stringLocalizer)
        {
            _stringLocalizer = stringLocalizer;
                A = new AClass(stringLocalizer);
    
        }
    
        private class AClass : ILocalizedTest.ILocalizedA
        {
            private readonly IStringLocalizer<Test> _stringLocalizer;
        
            public ILocalizedTest.ILocalizedA.ILocalizedB B { get; }
        
        
        
            public AClass(IStringLocalizer<Test> stringLocalizer)
            {
                _stringLocalizer = stringLocalizer;
                    B = new BClass(stringLocalizer);
        
            }
        
            private class BClass : ILocalizedTest.ILocalizedA.ILocalizedB
            {
                private readonly IStringLocalizer<Test> _stringLocalizer;
            
                public ILocalizedTest.ILocalizedA.ILocalizedB.ILocalizedD D { get; }
            
                public string DString => _stringLocalizer[""A.B.D""] as string ?? ""A.B.D"";
            
            
                public BClass(IStringLocalizer<Test> stringLocalizer)
                {
                    _stringLocalizer = stringLocalizer;
                        D = new DClass(stringLocalizer);
            
                }
            
                private class DClass : ILocalizedTest.ILocalizedA.ILocalizedB.ILocalizedD
                {
                    private readonly IStringLocalizer<Test> _stringLocalizer;
                
                
                    public string E => _stringLocalizer[""A.B.D.E""] as string ?? ""A.B.D.E"";
                
                
                    public DClass(IStringLocalizer<Test> stringLocalizer)
                    {
                        _stringLocalizer = stringLocalizer;
                        
                    }
                
                
                }
            
            }
        
        }
    
    }

}";
        #endregion
    }
}

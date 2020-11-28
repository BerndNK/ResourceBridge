using System.Linq;
using NUnit.Framework;

namespace ResourceBridge.Test
{
    [TestFixture]
    public class ResourceFileReaderTest
    {
        private const string TestFile = @"<root>
  <resheader name=""resmimetype"">
    <value>text/microsoft-resx</value>
  </resheader>
  <resheader name=""version"">
    <value>1.3</value>
  </resheader>
  <resheader name=""reader"">
    <value>System.Resources.ResXResourceReader, System.Windows.Forms, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value>
  </resheader>
  <resheader name=""writer"">
    <value>System.Resources.ResXResourceWriter, System.Windows.Forms, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value>
  </resheader>
  <data name=""GroupA.Hello"" xml:space=""preserve"">
    <value>Hello</value>
  </data>
  <data name=""GroupA.World"" xml:space=""preserve"">
    <value>World</value>
  </data>
  <data name=""GroupB.Child.Hello"" xml:space=""preserve"">
    <value>Hello B</value>
  </data>
  <data name=""GroupB.Child.World"" xml:space=""preserve"">
    <value>World B</value>
  </data>
</root>";

        [Test]
        public void ValidXml_ResultsInListOfNodes()
        {
            var reader = new ResourceFileReader();

            var result = reader.Read(TestFile).ToList();

            Assert.That(result.Count, Is.Not.Zero);
            Assert.That(result.All(r => !string.IsNullOrEmpty(r.Value) && !string.IsNullOrEmpty(r.Name)));
        }
    }
}
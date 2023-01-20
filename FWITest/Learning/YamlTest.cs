using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.RepresentationModel;

namespace FWITest.Learning
{
    [TestClass]
    public class YamlTest
    {
        [TestMethod]
        public void TestYaml()
        {
            var writer = new MockStreamWriter();
            writer.WriteLine("home:");
            writer.WriteLine("  address: 1234");
            var input = new StringReader(writer.GetOutputAsString());

            var yaml = new YamlStream();
            yaml.Load(input);

            var document = yaml.Documents[0];

            var root = (YamlMappingNode)document.RootNode;
            var home = (YamlMappingNode)root.Children[new YamlScalarNode("home")];
            var address = home.Children[new YamlScalarNode("address")];


            var expected = "1234";
            var actual = address.ToString();
            Assert.AreEqual(expected, actual);
        }

    }
}

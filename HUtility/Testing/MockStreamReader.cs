using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUtility.Testing
{
    public class MockStreamReader : StreamReader
    {
        public MockStreamReader(string input) : base(new MemoryStream())
        {
            // write the input string to the underlying stream
            StreamWriter writer = new StreamWriter(BaseStream);
            writer.Write(input);
            writer.Flush();

            // reset the position of the stream to the beginning
            BaseStream.Position = 0;
        }

        public MockStreamReader(MockStreamWriter writer) : this(writer.GetOutputAsString())
        {

        }
    }
}

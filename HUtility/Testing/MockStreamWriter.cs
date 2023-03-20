using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUtility.Testing
{
    public class MockStreamWriter : StreamWriter
    {
        public MockStreamWriter() : base(new MemoryStream())
        {

        }

        public string OutputAsString => GetOutputAsString();

        public string GetOutputAsString()
        {
            // flush the stream to ensure all data is written
            this.Flush();

            // reset the position of the stream to the beginning
            this.BaseStream.Position = 0;

            // read the contents of the stream into a string
            StreamReader reader = new StreamReader(this.BaseStream);
            return reader.ReadToEnd();
        }
    }
}

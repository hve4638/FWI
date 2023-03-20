using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FWI
{
    static public class TimelineSerializableDeserializer
    {
        public static TimelineSerializable Deserialize(string name, SerializeType serializeType)
        {
            using var stream = new StreamReader(name);
            return Deserialize(stream, serializeType);
        }

        public static TimelineSerializable Deserialize(StreamReader writer, SerializeType serializeType)
        {
            switch (serializeType)
            {
                case SerializeType.Xml:
                    return DeserializeXml(writer);

                default:
                    throw new NotImplementedException();
            }
        }

        public static TimelineSerializable DeserializeXml(StreamReader stream)
        {
            var serializer = new XmlSerializer(typeof(TimelineSerializable));

            return (TimelineSerializable)serializer.Deserialize(stream);
        }
    }
}

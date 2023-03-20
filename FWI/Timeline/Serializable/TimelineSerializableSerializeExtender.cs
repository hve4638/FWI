using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FWI
{
    static public class TimelineSerializableSerializeExtender
    {
        public static void Serialize(this TimelineSerializable serializable,
            string name, SerializeType serializeType)
        {
            using var stream = new StreamWriter(name);
            serializable.Serialize(stream, serializeType);
        }

        public static void Serialize(this TimelineSerializable serializable,
            StreamWriter writer, SerializeType serializeType)
        {
            switch (serializeType)
            {
                case SerializeType.Xml:
                    serializable.SerializeXml(writer);
                    break;
            }
        }

        public static void SerializeXml(this TimelineSerializable serializable, StreamWriter writer)
        {
            var serializer = new XmlSerializer(typeof(TimelineSerializable));

            serializer.Serialize(writer, serializable);
        }
    }
}

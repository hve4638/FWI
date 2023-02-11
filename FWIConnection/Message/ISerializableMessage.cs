using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWIConnection.Message
{
    public interface ISerializableMessage
    {
        byte[] Serialize();
        byte[] Serialize(bool debug);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient
{
    public interface IHandleTransmissionStrategy
    {
        void handle(ChatClient client, byte[] inBuffer);
    }
}

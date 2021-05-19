using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer
{
    public interface IClientHandler
    {
        void startWorking();

        string HandledUserName { get; set; }

        bool isWorking();

        void sendMessage(byte typeByte, byte[] message);

        void shutdown();
    }
}

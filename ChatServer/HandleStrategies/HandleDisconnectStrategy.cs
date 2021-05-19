using System;
using System.Collections.Generic;
using ChatModel;

namespace ChatServer.HandleStrategies
{
    class HandleDisconnectStrategy : IHandleStrategy
    {
        public void handleRequest(List<IClientHandler> allHandlers, ChatSystem chatSystem, IClientHandler handlerThread, byte[] messageBytes)
        {
            Console.WriteLine("DEBUG: {0} request received", "disconnect");
            handlerThread.shutdown();
        }
    }
}

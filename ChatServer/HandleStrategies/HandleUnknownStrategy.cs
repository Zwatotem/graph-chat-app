using System;
using System.Collections.Generic;
using ChatModel;

namespace ChatServer.HandleStrategies
{
    class HandleUnknownStrategy : IHandleStrategy
    {
        public void handleRequest(List<IClientHandler> allHandlers, ChatSystem chatSystem, IClientHandler handlerThread, byte[] messageBytes)
        {
            Console.WriteLine("DEBUG: unknown request receive");
        }
    }
}

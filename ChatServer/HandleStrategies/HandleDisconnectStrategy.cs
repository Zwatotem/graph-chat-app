using System;
using System.Collections.Generic;
using ChatModel;

namespace ChatServer.HandleStrategies;

class HandleDisconnectStrategy : IHandleStrategy
{
    /// <summary>
    /// Class handling request to disconnect.
    /// </summary>
    public void handleRequest(List<IClientHandler> allHandlers, IServerChatSystem chatSystem, IClientHandler handlerThread, byte[] messageBytes)
    {
        Console.WriteLine("DEBUG: {0} request received", "disconnect");
        handlerThread.shutdown(); //shutdown the handler
    }
}

/*
One of concrete strategies of the implemented strategy pattern.
This class has only one responsibility.
Complies with Liskov Substitution Principle - all interface methods are properly implemented.
*/
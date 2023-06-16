using System;
using System.Collections.Generic;
using ChatModel;

namespace ChatServer.HandleStrategies;

/// <summary>
/// Class handling an unknown request.
/// </summary>
class HandleUnknownStrategy : IHandleStrategy
{
    public void handleRequest(List<IClientHandler> allHandlers, IServerChatSystem chatSystem, IClientHandler handlerThread, byte[] messageBytes)
    {
        Console.WriteLine("DEBUG: unknown request receive");
    }
}

/*
One of concrete strategies of the implemented strategy pattern.
This class has only one responsibility.
Complies with Liskov Substitution Principle - all interface methods are properly implemented.
*/
using System.Collections.Generic;
using ChatModel;

namespace ChatServer;

/// <summary>
/// Interface representing a strategy of handling a request from the client.
/// </summary>
public interface IHandleStrategy
{
    /// <summary>
    /// Handles a resquest from the client in a way dependant on the concrete implementation
    /// </summary>
    /// <param name="allHandlers">List of all active handlers</param>
    /// <param name="chatSystem">Server chat system instance</param>
    /// <param name="handlerThread">Handler receiving the request</param>
    /// <param name="messageBytes">Request in form of byte array</param>
    void handleRequest(List<IClientHandler> allHandlers, IServerChatSystem chatSystem, IClientHandler handlerThread, byte[] messageBytes);
}

/*
This is the interface part of implemented strategy design pattern.
It is an example of compliance with SOLID principles:
1. It has only a single responsibility - handling a request.
2. It discourages modification of code and favours extension - to change how requests are handled just create new implementations.
4. This interaface has only one method - in compliance with interface segregation principle.
5. It allows for dependency inversion references are of abstract interface type rather that of concrete class type.
*/
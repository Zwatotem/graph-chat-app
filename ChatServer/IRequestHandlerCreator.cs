
namespace ChatServer;

/// <summary>
/// Interface representing factory of request handlers.
/// </summary>
public interface IRequestHandlerCreator
{
    /// <summary>
    /// Factory method creating new request handler.
    /// </summary>
    /// <param name="typeByte">Type of the request for which a handler is needed</param>
    /// <returns>Concrete instance of class implementing IRequestHandler</returns>
    IRequestHandler createRequestHandler(byte typeByte);
}

/*
This interface is part of the implementation of factory method design pattern.
It's a perfect example of compliance with SOLID principles:
1. It has a single resposibility (and even a signle method).
2. It encourages extension over modification - if you want to implement the factory differently just create a new extending class.
4. This interface has only one method (interface segregation).
5. It allows for dependency inversion - other classes can reference this abstract type rather than concrete implementations.
*/
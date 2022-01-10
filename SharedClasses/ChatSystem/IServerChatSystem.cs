using System;
using System.Collections.Generic;

namespace ChatModel;

/// <summary>
/// Interface extending IChatSystem, adding functionality necessary only on the server side.
/// </summary>
public interface IServerChatSystem : IChatSystem
{
    /// <summary>
    /// Returns updates concerning a given user that happened after a given time.
    /// </summary>
    /// <param name="userName">User name of user for whom updates are to be returned</param>
    /// <param name="t">Time from after which we return updates</param>
    /// <returns>UserUpdates object with updates to user or null if there is no such user.</returns>
    UserUpdates getUpdatesToUser(string userName, DateTime t);

    /// <summary>
    /// Return conversations of a given user.
    /// </summary>
    /// <param name="userName">User name of user of whom we get conversations</param>
    /// <returns>List of conversations or null if there is no user with such user name.</returns>
    List<Conversation> getConversationsOfUser(string userName);
}

/*
Single responsibility of this interface is to contain logic necessary only on the server side of the app. It has only two methods
(interface segregation) and doesn't meddle with inherited methods (as to comply with Liskov Substitution). It encourages code extension over
modification as the interface can always be implemented anew, rather than modifying existing code.
*/
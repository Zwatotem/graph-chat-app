﻿using System.IO;
using ChatModel.Util;

namespace ChatModel;

/// <summary>
/// Interface extending IChatSystem and adding functionality necessary on the client side of the app.
/// </summary>
public interface IClientChatSystem : IChatSystem
{
    /// <summary>
    /// Name of the currently logged in user.
    /// </summary>
    /// <remarks>Null if no user logged in.</remarks>
    string LoggedUserName { get; }

    /// <summary>
    /// Logs the user in.
    /// </summary>
    /// <param name="login">User name under which to log in.</param>
    /// <returns>True if successful, false otherwise.</returns>
    bool logIn(IUser user);

    /// <summary>
    /// Updates the state of chat system based on UserUpdates object.
    /// </summary>
    /// <param name="updates">Object containig updates to apply</param>
    void applyUpdates(UserUpdates updates);
}

/*
This interface has only a few essential methods, doesn't meddle with base inherited ones (thus complying with Liskov Substitution),
allows other classes to referene it rather than something concrete (dependency inversion) and favours code extension over modification
- when one wants the client chat system to behave differently one can just implement the interface anew instead of modifying existing code.
*/
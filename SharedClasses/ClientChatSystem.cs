using System;
using System.Collections.Generic;
using System.IO;
using ChatModel.Util;

namespace ChatModel
{
    /// <summary>
    /// Concrete implementation of IClientChatSystem.
    /// </summary>
    public class ClientChatSystem : ChatSystem, IClientChatSystem
    {
        private string userName; //name of logged in user

        public ClientChatSystem() : base()
        {
            this.userName = null; //indicates that there is no logged in user at the start
        }

        public string LoggedInName { get => userName; }

        public bool logIn(string login)
        {
            if (userName == null && users.Find(u => u.Name == login) != null) //if no one is logged in and the user exists
            {
                userName = login; //it is possible to login
                return true;
            }
            else
            {
                return false;
            }
        }

        public Conversation addConversation(Stream stream, IDeserializer deserializer)
        {
            Conversation conv = (Conversation)deserializer.deserialize(stream);
            if (conversations.ContainsKey(conv.ID))
            {
                return null; //if there is already a conversation with this id
            }
            var newUsers = new List<IUser>(); // List of overlapping IUser objects
            foreach (var user in conv.Users)
            {
                Predicate<IUser> p = (u => u.Name == user.Name);
                if (users.Exists(p))
                {
                    newUsers.Add(users.Find(p)); // Collect the users already present in the chat system that we'll inject to the new conversation
                }
                else
                {
                    users.Add(user); //if the user doesn't yet exist in the chat system they are added
                }
            }
            foreach (var user in newUsers)
            {
                //fix references in the conversation that point to users already present in the system so that they point to correct objects
                conv.reMatchWithUser(user);
                user.matchWithConversation(conv);
            }
            conversations.Add(conv.ID, conv); //add the conversation to the chat system
            return conv;
        }

        public void applyUpdates(UserUpdates updates)
        {
            foreach (var convUpdate in updates)
            {
                if (!conversations.ContainsKey(convUpdate.ID)) //if there is no such conversation we add it
                {
                    var newUsers = new List<IUser>(); // List of overlapping IUser objects
                    conversations.Add(convUpdate.ID, new Conversation(convUpdate));
                    foreach (var user in convUpdate.Users)
                    {
                        Predicate<IUser> p = (u => u.Name == user.Name);
                        if (users.Exists(p))
                        {
                            newUsers.Add(users.Find(p)); // Collect the users already present in the chat system that we'll inject to the new conversation
                        }
                        else
                        {
                            users.Add(user); //if the user doesn't yet exist in the chat system they are added
                        }
                    }
                    foreach (var user in newUsers)
                    {
                        //fix references in the conversation that point to users already present in the system so that they point to correct objects
                        conversations[convUpdate.ID].reMatchWithUser(user);
                        user.matchWithConversation(conversations[convUpdate.ID]);
                    }
                    if (freedIds.Contains(convUpdate.ID))
                    {
                        //fix the stack of freed ids if the added conversation has an id present in the stack
                        var stackNoID = new List<int>(freedIds);
                        stackNoID.Remove(smallestFreeId);
                        freedIds = new Stack<int>(stackNoID);
                    }
                    else
                    {
                        //if smallest free id smaller than new conversation's id fix it and put smaller free ones onto the stack
                        for (int i = smallestFreeId; i < convUpdate.ID; i++)
                        {
                            freedIds.Push(i);
                        }
                        smallestFreeId = convUpdate.ID + 1;
                    }
                }
                else //else we update the one already present
                {
                    conversations[convUpdate.ID].applyUpdates(convUpdate);
                }
            }
        }
    }
}

/*
Class complies with Liskov Substitution as it properly implements all interface and base methods. Its only resposibility is chat system logic
necessary only on the client side of the app.
*/
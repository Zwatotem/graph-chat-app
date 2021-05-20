using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using ChatModel.Util;

namespace ChatModel
{
    [Serializable]
    public class Message
    {
        private Refrence<User> authorRef; // Helper for merging user lists after Conversation deserialization
        private User author;
        private IMessageContent content;
        private DateTime sentTime;
        private Message targetedMessage;
        private int targetId; // Redundant value used to recover the message structure after deserialization
        private int id;

        public Refrence<User> AuthorRef { get => authorRef; set => authorRef = value; }

        public User Author
        {
            get
            {
                if (authorRef.Reference == null) // Author was probably removed from the conversation
                {
                    return author;
                }
                else if (authorRef.Reference != author) // Conversation was merged with a new ChatSystem
                {
                    author = authorRef.Reference;
                }
                return author;
            }
        }

        public IMessageContent Content { get => content; }

        public DateTime SentTime { get => sentTime; }

        public Message Parent
        {
            get => targetedMessage;
            set
            {
                targetedMessage = value;
                targetId = (value == null) ? -1 : value.ID;
            }
        }

        public int TargetId { get => targetId; }

        public int ID { get => id; }       

        public Message(User user, Message targeted, IMessageContent messageContent, DateTime datetime, int id)
            : this(targeted, messageContent, datetime, id)
        {
            this.author = user;
        }

        public Message(Refrence<User> user, Message targeted, IMessageContent messageContent, DateTime datetime, int id)
            : this(targeted, messageContent, datetime, id)
        {
            this.authorRef = user;
        }

        private Message(Message targeted, IMessageContent messageContent, DateTime datetime, int id)
        {
            this.content = messageContent;
            this.sentTime = datetime;
            this.id = id;
            this.Parent = targeted;
        }

        /// <summary>
        /// Constructs new message by doing shallow copy of the object provided
        /// </summary>
        /// <param name="other">Template message for construction</param>
        public Message(Message other)
        {
            this.authorRef = other.authorRef;
            this.author = other.author;
            this.content = other.content;
            this.sentTime = other.sentTime;
            this.id = other.id;
            this.targetId = other.targetId;
            this.targetedMessage = other.targetedMessage;
        }

        public void setParentUnsafe(Message t)
        {
            targetedMessage = t;
        }

        public MemoryStream serialize(ISerializer serializer)
        {
            Message copy = new Message(this);
            copy.targetedMessage = null;
            return serializer.serialize(copy);
        }
    }
}
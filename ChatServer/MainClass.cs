using System;
using System.Threading;

namespace ChatServer
{
    /// <summary>
    /// Seperate class for the Main method.
    /// </summary>
    public class MainClass
    {
        /// <summary>
        /// Creates and starts the server.
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            IChatServer chatServer = new ConcreteChatServer("192.168.42.225", 50000, 5);
            chatServer.startServer();
        }
    }
}

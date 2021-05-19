using System;
using System.Threading;

namespace ChatServer
{
    public class MainClass
    {
        public static void Main(string[] args)
        {
            IChatServer chatServer = new ConcreteChatServer("192.168.42.225", 50000, 5);
            chatServer.startServer();
            /*
            Console.WriteLine(chatServer.isWorking());
            Thread.Sleep(5000);
            chatServer.shutdown();
            Console.WriteLine(chatServer.isWorking());
            */
        }
    }
}

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ChatServer
{
    class Program
    {
        static void MainOld(string[] args)
        {
            byte[] bytes = new Byte[1024];
            IPAddress serverIpAddress = IPAddress.Parse("192.168.42.225");
            IPEndPoint serverEndPoint = new IPEndPoint(serverIpAddress, 50000);
            Socket serverSocket = new Socket(serverIpAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                serverSocket.Bind(serverEndPoint);            
                serverSocket.Listen(3);
                while (true)
                {
                    Console.WriteLine("Listening...");
                    Socket handler = serverSocket.Accept();
                    string data = null;
                    // An incoming connection needs to be processed.  
                    int headerLength = BitConverter.GetBytes(4).Length;
                    byte[] headerBytes = new byte[headerLength];
                    int bytesReceived = 0;
                    while (bytesReceived < headerLength)
                    {
                        bytesReceived += handler.Receive(headerBytes, bytesReceived, headerLength - bytesReceived, SocketFlags.None);
                    }
                    int messageLength = BitConverter.ToInt32(headerBytes);
                    byte[] contentBytes = new byte[messageLength];
                    bytesReceived = 0;
                    while (bytesReceived < messageLength)
                    {
                        bytesReceived += handler.Receive(contentBytes, bytesReceived, messageLength - bytesReceived, SocketFlags.None);
                    }
                    data = Encoding.UTF8.GetString(contentBytes);
                    // Show the data on the console.  
                    Console.WriteLine("Text received : {0}", data);

                    // Echo the data back to the client.  
                    byte[] msg = BitConverter.GetBytes(2);
                    handler.Send(msg);
                    msg = new byte[2];
                    msg[0] = 0;
                    msg[1] = 1;
                    handler.Send(msg);
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }
            } catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}

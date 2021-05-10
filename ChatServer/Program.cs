using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ChatServer
{
    class Program
    {
        static void Main(string[] args)
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
                    bool flag = true;
                    while (flag)
                    {
                        int bytesRec = handler.Receive(bytes);
                        Console.WriteLine("Received something...");
                        data += Encoding.UTF8.GetString(bytes, 0, bytesRec);
                        if (data.IndexOf("<EOF>") > -1)
                        {
                            data = data.Substring(0, data.Length - 5);
                            break;
                        }
                    }

                    // Show the data on the console.  
                    Console.WriteLine("Text received : {0}", data);

                    // Echo the data back to the client.  
                    byte[] msg = Encoding.ASCII.GetBytes(data);

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

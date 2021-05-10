using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ChatClient
{
    class Program
    {
        static void Main(string[] args)
        {
            IPAddress serverIpAddress = IPAddress.Parse("192.168.42.225");
            IPEndPoint ipe = new IPEndPoint(serverIpAddress, 50000);
            string request = Console.ReadLine();
            request = request + "<EOF>";
            Byte[] bytesSent = Encoding.UTF8.GetBytes(request);
            Byte[] bytesReceived = new Byte[256];
            string page = "";

            using (Socket s = new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp))
            {
                s.Connect(ipe);
                // Send request to the server.
                s.Send(bytesSent, bytesSent.Length, 0);

                // Receive the server home page content.
                int bytes = 0;

                // The following will block until the page is transmitted.
                do
                {
                    bytes = s.Receive(bytesReceived, bytesReceived.Length, 0);
                    page = page + Encoding.ASCII.GetString(bytesReceived, 0, bytes);
                }
                while (bytes > 0);
            }
            Console.WriteLine(page);
        }
    }
}

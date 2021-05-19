using System;
using System.Net.Sockets;

namespace ChatServer
{
    public class ConcreteIOSocketFacade : IIOSocketFacade
    {
        private Socket socket;

        public ConcreteIOSocketFacade(Socket socket)
        {
            this.socket = socket;
        }

        public byte[] receiveMessage(int length)
        {
            if (length <= 0)
            {
                return null;
            }
            byte[] buffer = new byte[length];
            int bytesReceived = 0;
            while (bytesReceived < length)
            {
                bytesReceived += socket.Receive(buffer, bytesReceived, length - bytesReceived, SocketFlags.None);
            }
            return buffer;
        }

        public void sendMessage(byte type, byte[] message)
        {
            byte[] header = new byte[5];
            header[0] = type;
            Array.Copy(BitConverter.GetBytes(message.Length), 0, header, 1, 4);
            lock (socket)
            {
                if (socket.Connected)
                {
                    socket.Send(header);
                    socket.Send(message);
                }
            }
        }

        public void shutdown()
        {
            lock (socket)
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
                socket.Dispose();
            }
        }
    }
}

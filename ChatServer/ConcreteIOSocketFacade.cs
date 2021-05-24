using System;
using System.Net.Sockets;

namespace ChatServer
{
    /// <summary>
    /// Concrete implementation of the IIOSocketFacade.
    /// </summary>
    public class ConcreteIOSocketFacade : IIOSocketFacade
    {
        private Socket socket; //the socket which the facede is encapsulating

        public ConcreteIOSocketFacade(Socket socket)
        {
            this.socket = socket;
        }

        public byte[] receiveMessage(int length)
        {
            if (length <= 0) //cannot read message of non-positive length
            {
                return null;
            }
            byte[] buffer = new byte[length];
            int bytesReceived = 0;
            while (bytesReceived < length) //read while haven't yet read all bytes
            {
                bytesReceived += socket.Receive(buffer, bytesReceived, length - bytesReceived, SocketFlags.None);
            }
            return buffer;
        }

        public void sendMessage(byte type, byte[] message)
        {
            byte[] header = new byte[5];
            header[0] = type; //first byte of header is the type byte
            Array.Copy(BitConverter.GetBytes(message.Length), 0, header, 1, 4); //next four bytes are the length of the message
            lock (socket) //prohibit for instance sending 2 messages at once
            {
                if (socket.Connected) //if there wasn't a disconnect in the meantime
                {
                    socket.Send(header);
                    socket.Send(message);
                }
            }
        }

        public void shutdown()
        {
            lock (socket) //don't want to close the socket while some other thread is receiving/sending
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
                socket.Dispose();
            }
        }
    }
}

/*
This is the concrete implementation of the facade desing pattern. It complies with Liskov Substitution as it properly implements all of the
interface methods. It separates socket input output operations from the rest of the program.
*/
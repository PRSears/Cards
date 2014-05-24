using System;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;

namespace Cards.Net
{
    class TestSender
    {
        public const int ServerPort = Cards.Structs.GameConstants.SERVER_PORT;

        public TestSender()
        {
            TcpClient testClient = new TcpClient();
            IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), ServerPort);

            testClient.Connect(serverEndPoint);

            NetworkStream clientStream = testClient.GetStream();
            ASCIIEncoding encoder = new ASCIIEncoding();

            string message = "";
            do
            {
                message = Console.ReadLine();
                byte[] buffer = encoder.GetBytes(message);

                clientStream.Write(buffer, 0, buffer.Length);
                clientStream.Flush();
            }
            while (!message.Equals("terminate"));
        }
    }
}

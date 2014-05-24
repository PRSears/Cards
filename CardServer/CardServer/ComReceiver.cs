using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.Xna.Framework;
using Networking;
using Networking.Structs;

namespace CardServer
{
    public class ComReceiver
    {
        public TcpClient Client
        {
            get;
            private set;
        }
        private Thread ReceiveClientComThread;
        private GameServer Server;

        public Guid ID;
        public bool Listening
        {
            get;
            private set;
        }

        public ComReceiver(TcpClient client, GameServer server)
        {
            Server = server;

            ID = Guid.NewGuid();
            Listening = false;

            Client = client;

            ReceiveClientComThread = new Thread(new ThreadStart(ListenForComs));
            ReceiveClientComThread.Name = "ReceiveClientComThread [" + ID.ToString() + "]";
        }

        private void ListenForComs()
        {
            if (CardServer.Properties.Settings.Default.Debug)
                Console.WriteLine(ReceiveClientComThread.Name + " successfully started.");

            NetworkStream clientStream = Client.GetStream();

            byte[] byteBuffer = new byte[32];
            int bufferFilledSize = 0;
            int bytesRead;

            while (Listening)
            {
                bytesRead = 0;

                try
                {
                    bufferFilledSize += bytesRead = clientStream.Read(byteBuffer, bufferFilledSize, 4); // read 4 bytes at a time.
                }
                catch (Exception e)
                {
                    if (CardServer.Properties.Settings.Default.Debug)
                        Console.WriteLine(e.Message);
                    break;
                }

                if (bytesRead == 0)
                    break; // Connection timed out before any data was received.
            }

            if (bufferFilledSize > 0)
            {
                NetWrapper WrappedData = new NetWrapper(byteBuffer);
                Server.MessageQueue.Enqueue(WrappedData);
            }

            Client.Close();
            Server.ComReceivers.Remove(this);
        }

        /// <summary>
        /// Trims the byte array to the given size.
        /// </summary>
        /// <param name="buffer">original byte array to trim.</param>
        /// <param name="size">final size to trim to.</param>
        /// <returns>Returns trimmed byte array of length 'size.'</returns>
        private byte[] TrimBuffer(byte[] buffer, int size)
        {
            byte[] trimmed = new byte[size];

            for (int i = 0; i < size; i++)
            {
                trimmed[i] = buffer[i];
            }

            return trimmed;
        }

        /// <summary>
        /// Starts listening for communications from the client.
        /// </summary>
        public void Start()
        {
            if (CardServer.Properties.Settings.Default.Debug)
                Console.WriteLine(ReceiveClientComThread.Name + " is starting.");

            Listening = true;
            ReceiveClientComThread.Start();
        }

        /// <summary>
        /// Tries to cleanly stop the listener and close the thread.
        /// </summary>
        public void Stop()
        {
            Listening = false;

            Client.Close();
            ReceiveClientComThread.Abort();
        }
    }
}

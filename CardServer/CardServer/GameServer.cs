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
    public class GameServer
    {        
        private Thread ClientListenerThread;
        private TcpListener NewConnectionsListener;
        public List<TcpClient> TcpClientList;
        public List<ComReceiver> ComReceivers;
        public Queue<NetWrapper> MessageQueue;

        public bool AcceptingConnections
        {
            get;
            private set;
        }
        public int GamePort
        { 
            get; 
            private set; 
        }

        /// <summary>
        /// 
        /// </summary>
        public GameServer()
        {
            GamePort = CardServer.Properties.Settings.Default.DefaultServerPort;
            MessageQueue = new Queue<NetWrapper>();
            TcpClientList = new List<TcpClient>();
            ComReceivers = new List<ComReceiver>();

            ClientListenerThread = new Thread(new ThreadStart(ListenForConnections));
            ClientListenerThread.Name = "ClientListenedThread";

            if (CardServer.Properties.Settings.Default.Debug)
                Console.WriteLine("ClientListenerThread is starting.");

            AcceptingConnections = true;
            ClientListenerThread.Start();
        }        

        private void ListenForConnections()
        {
            NewConnectionsListener = new TcpListener(IPAddress.Any, this.GamePort);

            if (CardServer.Properties.Settings.Default.Debug)
                Console.WriteLine("ClientListenerThread successfully started.");

            try
            {
                NewConnectionsListener.Start();
            }
            catch(SocketException e)
            {
                // TODO Write message explaining error, allow retry.
                throw;
            }

            while (AcceptingConnections)
            {
                // Blocks until timeout / a new client attempts to connect.
                TcpClient newClient = this.NewConnectionsListener.AcceptTcpClient();

                if (!InClientList(newClient))
                    TcpClientList.Add(newClient);

                ComReceiver r = new ComReceiver(newClient, this);
                ComReceivers.Add(r);
                r.Start();
            }

            try
            {
                NewConnectionsListener.Stop();
            }
            catch (SocketException e)
            {
                // If there was a problem while closing, I don't give a fuck. 
                Console.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// Checks if the passed client object has been added to the ClientList
        /// </summary>
        /// <param name="client">TcpClient to search for</param>
        /// <returns>True if the client already exists in the ClientList.</returns>
        public bool InClientList(TcpClient client)
        {
            foreach (TcpClient listed in TcpClientList)
                if (listed.Equals(client))
                    return true;
            return false;
        }

        /// <summary>
        /// Tries to close the connection with the given client.
        /// </summary>
        /// <param name="client">Client to attempt disconnect with.</param>
        public void DisconnectClient(TcpClient client)
        {
            // TODO Search clientlist and close connections with matching client(s).
            throw new NotImplementedException();
        }

        /// <summary>
        /// Tries to close the connection with all clients.
        /// </summary>
        public void DisconnectAllClients()
        {
            foreach (ComReceiver r in ComReceivers)
                r.Stop();
            ComReceivers.Clear();
            foreach (TcpClient c in TcpClientList)
                c.Close();
            TcpClientList.Clear();
        }

        /// <summary>
        /// Tries to cleanly close any connections and threads.
        /// </summary>
        public void Stop()
        {
            AcceptingConnections = false;
            DisconnectAllClients();
            // TODO cleanly close connections and threads.
        }
    }
}

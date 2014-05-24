using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input; // <= DELETE THIS. TESTING ONLY.
using Cards.Networking.Structs;

namespace Cards.Networking
{
	[Obsolete("Moved to seperate solution.", true)]
    public class GameServer    
    {     

        private List<TcpClient> ClientList;

        private TcpListener tcpListen;
        private Thread listenerThread;
        //private List<Thread> playerCommsThreads;
        private const int GamePort = Cards.Structs.GameConstants.SERVER_PORT;

        private bool debug = false;
        private bool exiting = false;

        public GameServer()
        {
            ClientList = new List<TcpClient>();

            // Spawn a new thread for a listener to operate on, and start listening for actions
            tcpListen = new TcpListener(IPAddress.Any, GamePort);
            listenerThread = new Thread(new ThreadStart(ListenForClients));
            listenerThread.Name = "GameServerThread";
            listenerThread.IsBackground = true;
            listenerThread.Start();

            System.Diagnostics.Debug.WriteLine("GameServerThread started");

            //playerCommsThreads = new List<Thread>();

            System.Diagnostics.Debug.WriteLine("Server started.");
        }

        private void ListenForClients()
        {
            try
            {
                tcpListen.Start();
            }
            catch (SocketException e)
            {
                //TODO show pop up and (cleanly) close the game on SocketException
                throw;
            }

            while (!exiting)
            {
                // does nothing until a client connects to the server
                TcpClient playerClient = this.tcpListen.AcceptTcpClient();
                // add the client to the ClientList if it's not already added
                if (!InClientList(playerClient))
                    ClientList.Add(playerClient);

                // start a new thread of the playerClient to communicate on. not sure if I need players to be able to act at the same time on their own threads...
                //playerCommsThreads.Add(new Thread(new ParameterizedThreadStart(ReceivePlayerComms)));
                //playerCommsThreads[playerCommsThreads.Count - 1].Start(playerClient);

                Thread playerCommThread = new Thread(new ParameterizedThreadStart(ReceivePlayerComms));
                playerCommThread.Name = "playerCommThread" + ClientList.Count;
                playerCommThread.Start(playerClient);
                System.Diagnostics.Debug.WriteLine("playerCommThread" + ClientList.Count + " started");
            }
        }

        private void ReceivePlayerComms(object client)
        {
            TcpClient playerClient = (TcpClient)client;
            NetworkStream playerStream = playerClient.GetStream();

            playerStream.ReadTimeout = 3000;

            byte[] byteBuffer = new byte[4];
            int bytesRead;

            while (!exiting)
            {
                bytesRead = 0;

                try
                {
                    // waits for a playerClient to send something, stores data in bytesRead once it arrives
                    bytesRead = playerStream.Read(byteBuffer, 0, 4);
                }
                catch (Exception e)
                {
                    if(debug)
                        System.Diagnostics.Debug.WriteLine(e.Message);
                    break;
                }

                // playerClient disconnected
                if (bytesRead == 0)
                    break;

                // TODO do something with the recieved data
                //ASCIIEncoding encoder = new ASCIIEncoding();
                //Console.WriteLine(encoder.GetString(action, 0, bytesRead));
                //ActionEncoding encoder = new ActionEncoding();
                //System.Diagnostics.Debug.WriteLine(encoder.BytesToVector3(byteBuffer).ToString());
            }

            playerStream.Close();
            playerClient.Close();
        }

        public string GetState()
        {
            return ("Listener thread: " + listenerThread.ThreadState);
        }

        // TODO add parameter for the GameAction object to send to the playerClient
        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        private void UpdatePlayerClient(TcpClient client)
        {
            NetworkStream playerStream = client.GetStream();
            ASCIIEncoding asciiEncoder = new ASCIIEncoding();
            //ActionEncoding actionEncoder = new ActionEncoding();
            byte[] action = {0,0,0,0};
            //actionEncoder.SetType(ref action, 0);
            //actionEncoder.SetOriginator(ref action, ActionEncoding.ORIGINATOR_SERVER);
            //actionEncoder.SetPayload(ref action, new byte[] {2, 3, 4, 5});

            playerStream.Write(action, 0, action.Length);
            playerStream.Flush();
        }

        /// <summary>
        /// Sets exiting flag.
        /// </summary>
        public void BeginShutdown()
        {
            exiting = true;
            tcpListen.Stop();

            listenerThread.Abort();
        }

        public void UpdatePlayerClient()
        {
            if (ClientList.Count < 1)
                return;

            NetworkStream playerStream = ClientList[0].GetStream();

            MouseState cm = Mouse.GetState();
            NetWrapper testPackage = new NetWrapper(Categories.NetMouseState, Originators.Server, new NetMouseState(cm));

            playerStream.Write(testPackage.ToBytes(), 0, testPackage.Length);
            playerStream.Flush();

            System.Diagnostics.Debug.WriteLine("Data sent from server.");
        }

        /// <summary>
        /// Checks if the passed client object has been added to the ClientList
        /// </summary>
        /// <param name="client">TcpClient to search for</param>
        /// <returns>True if the client already exists in the ClientList.</returns>
        private bool InClientList(TcpClient client)
        {
            foreach (TcpClient listed in ClientList)
                if (listed.Equals(client))
                    return true;
            return false;
        }
    }
}

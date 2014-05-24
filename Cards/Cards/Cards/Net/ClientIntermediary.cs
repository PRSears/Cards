using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;
using Networking;
using Cards.Player;
using Networking.Structs;

namespace Cards.Net
{
    public class ClientIntermediary
    {
        public Queue<NetWrapper> RecievedActions
        { 
            get; 
            protected set; 
        }
        public Queue<NetWrapper> SendingActions
        {
            get;
            protected set;
        }
        
        protected int ServerPort = Cards.Structs.GameConstants.SERVER_PORT;
        protected IPEndPoint serverEndPoint;

        private PokemonCardGame currentGame;
        private TcpListener tcpListen;
        private Thread listenerThread;
        private TcpClient server;
        private Guid ID;

        /// <summary>
        /// Continue to receive updates from server while true.
        /// </summary>
        public bool Listening { get; set; }

        public ClientIntermediary(PokemonCardGame game)
        {
            currentGame = game;

            RecievedActions = new Queue<NetWrapper>();
            SendingActions = new Queue<NetWrapper>();

            ID = Guid.NewGuid();
        }

        public void ConnectToServer(IPEndPoint endpoint)
        {
            serverEndPoint = endpoint;

            server = new TcpClient();
            try
            {
                server.Connect(serverEndPoint);
            }
            catch (SocketException e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                throw new ArgumentException("No server at IP, or host refused connection.", e);
            }

            tcpListen = new TcpListener(serverEndPoint);
            listenerThread = new Thread(new ThreadStart(ListenToServer));
            listenerThread.Name = "ClientIntermediary.listenerThread [" + ID.ToString() + "]";
            Start();
        }

        /// <summary>
        /// Starts listener thread to receive actions from the server. Called automatically by ConnectToServer().
        /// </summary>
        public void Start()
        {
            if(!listenerThread.IsAlive)
                listenerThread.Start();
        }

        /// <summary>
        /// Adds a new NetWrapper object to the SendingActions queue.
        /// </summary>
        /// <param name="netObject">Payload data to wrap and add to the SendingActions queue.</param>
        public void EnqueueSend(Payload netObject)
        {
            NetWrapper action = new NetWrapper(Categories.NetMouseState, Originator, netObject);
            SendingActions.Enqueue(action);
        }

        /// <summary>
        /// Forces any objects in the SendingActions queue to be sent to the server immediately. 
        /// Call Update() from the game's Update loop for normal sending functions.
        /// </summary>
        public void ForceSend()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Send contents of SendingActions queue to the server when ready.
        /// </summary>
        public void Update()
        {

            if (SendingActions.Count > 0)
            {
                if (!server.Connected)
                {
                    server = new TcpClient();
                    server.Connect(serverEndPoint);
                }

                System.Diagnostics.Debug.WriteLine("SendingActions > 0.");

                NetworkStream sendStream = server.GetStream();

                try
                {
                    while(SendingActions.Count > 0)
                    {
                        byte[] buffer = SendingActions.Dequeue().ToBytes();
                        sendStream.Write(buffer, 0, buffer.Length);
                    }
                }
                catch (System.IO.IOException ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    if (SendingActions.Count > 0)
                        System.Diagnostics.Debug.WriteLine(SendingActions.Peek().DebugText());
                    throw;
                }

                sendStream.Close();
                sendStream.Dispose(); 
            }
        }

        private void ListenToServer()
        {
            // TODO Handle server disconnects (try ... catch)

            NetworkStream serverStream = server.GetStream();
            byte[] dataBuffer;

            while (Listening)
            {
                System.Diagnostics.Debug.WriteLine("Trying to read data from server.");

                byte[] sizeBuffer = new Byte[4]; // Size is always going to be a 32bit int
                int totalBytes = 0;
                do
                {
                    totalBytes += serverStream.Read(sizeBuffer, totalBytes, sizeBuffer.Length - totalBytes);
                } while (totalBytes < sizeBuffer.Length);
                int dataSize = BitConverter.ToInt32(sizeBuffer, 0);         

                System.Diagnostics.Debug.WriteLine("Received stream size: " + dataSize.ToString());

                dataBuffer = new byte[dataSize];
                for (int i = 0; i < 4; i++)
                    dataBuffer[i] = sizeBuffer[i];

                int bytesRead = totalBytes;
                do
                {
                    //System.Diagnostics.Debug.WriteLine("BytesRead: " + bytesRead.ToString());
                    bytesRead += serverStream.Read(buffer: dataBuffer, offset: bytesRead, size: dataBuffer.Length - bytesRead);
                } while (bytesRead < dataSize);

                if (bytesRead == 0)
                {
                    System.Diagnostics.Debug.WriteLine("[Client] no data recieved.");
                    break;
                }

                System.Diagnostics.Debug.WriteLine("Enqueuing data from server.");
                RecievedActions.Enqueue(new NetWrapper(dataBuffer));
            }

            serverStream.Close();
            serverStream.Dispose();
            tcpListen.Stop();
            // HACK the TcpClient needs to be disposed of somewhere...
            //server.Close();
        }

        public Originators Originator
        {
            get
            {
                if (currentGame.PlayerType.Equals(NPlayer.TYPE_PLAYER1))
                    return Originators.Player1;
                if (currentGame.PlayerType.Equals(NPlayer.TYPE_PLAYER2))
                    return Originators.Player2;
                if (currentGame.PlayerType.Equals(NPlayer.TYPE_OBSERVER))
                    return Originators.Observer;

                throw new InvalidOperationException("No valid player type");
            }
        }

        public bool Connected
        {
            get { return server.Connected; }
        }
    }
}

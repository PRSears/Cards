using System;
using System.Collections.Generic;
using System.Text;

namespace CardServer
{
    class GameServerCLI
    {
        public static void Main(string[] args)
        {
            Console.WriteLine(Title("Pokemon Card Game Multiplayer Server"));
            Console.WriteLine("Debug is " + (CardServer.Properties.Settings.Default.Debug ? "ON" : "OFF"));
            GameServer gs = new GameServer();

            // HACK need to implement proper Update() methods
            while (true)
            {
                while (gs.MessageQueue.Count > 0)
                {
                    Console.WriteLine(gs.MessageQueue.Dequeue().Data.DebugString());
                    Console.WriteLine("ComReceivers: " + gs.ComReceivers.Count);
                }

                System.Threading.Thread.Sleep(1000);
            }

            //WaitForExit();
        }
       
        public static string Title(string text)
        {
            return (" -== " + text + " ==- ");
        }

        public static void WaitForExit()
        {
            while (true)
            {
                if (Console.ReadLine().ToLower().Equals("q"))
                {
                    Environment.Exit(0);
                    break;
                }
            }
        }



    }
}

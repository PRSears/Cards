using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Networking;
using Networking.Structs;

namespace TestHarness
{
    class Program
    {
        public static void Main(string[] args)
        {
            NetMouseState testMS = NetMouseState.CreateTestNetMouseState();
            NetWrapper testWrapper = new NetWrapper(Categories.NetMouseState, Originators.Observer, testMS);

            Console.WriteLine(testWrapper.DebugText());

            byte[] bytes = testWrapper.ToBytes();
            foreach (byte b in bytes)
                Console.Write(b + " ");

            Console.Read();
        }
    }
}

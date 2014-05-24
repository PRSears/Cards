using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using PokemonXML;
using Networking.Structs;

namespace Networking
{
    /// <summary>
    /// Helper class to convert between bytes and objects.
    /// </summary>
    [Obsolete("This is an obsolete mess, use NetWrapper whenever possible", false)]
    public class ActionEncoding
    {
        #region Originator constants
        public static byte ORIGINATOR_SERVER = 0;
        public static byte ORIGINATOR_PLAYER1 = 1;
        public static byte ORIGINATOR_PLAYER2 = 2;
        public static byte ORIGINATOR_OBSERVER = 3;
        #endregion
        #region Type constants
        public static byte TYPE_INPUT = 1;
        public static byte TYPE_UI_ELEMENT = 2;
        public static byte TYPE_SCREEN_MANAGER = 3;
        public static byte TYPE_GAMESTAGE = 4;
        public static byte TYPE_CARD_DATA = 5;
        public static byte TYPE_DECK_DATA = 6;
        public static byte TYPE_PLAYER_DATA = 7;
        public static byte TYPE_TEXTURE_UPDATE = 8;
        #endregion
        #region Bitmasks
        private static int bitmask4 = 15;
        private static int bitmask8 = 255;            // mask for all but first octet
        private static int bitmask12 = 3840;
        private static int bitmask16 = 65280;         // mask for all but second octet

        #endregion
        #region Header starting location constants
        public static int HEADER_SIZE = 0; 
        public static int HEADER_TYPE = 4;
        public static int HEADER_ORIG = 5;
        public static int HEADER_PAYL = 6;
        public static int HEADER_END = 10;
        #endregion

        /// <summary>
        /// Creates a new byte array (generic action object) with a NetSprite as the payload.
        /// </summary>
        /// <param name="NSprite"></param>
        /// <param name="playerNum"></param>
        /// <returns></returns>
        public byte[] CreateAction (NetSprite NSprite, byte playerNum)
        {
            byte[] bytesOut = new byte[HEADER_PAYL + NSprite.ENDPOINT];

            SetType(ref bytesOut, TYPE_TEXTURE_UPDATE);
            SetOriginator(ref bytesOut, ORIGINATOR_SERVER);
            SetPayload(ref bytesOut, NSprite.ToBytes());

            return bytesOut;
        }

        /*
        public byte[] CreateAction(Categories type, Originators originator, Payload payload)
        {
            byte[] bytesOut = new Byte[HEAD
        }
         * */

        public NetSprite GetNetSprite(byte[] action)
        {

            return new NetSprite(GetPayload(action));
        }

        public void SetOriginator(ref byte[] action, byte originator)
        {
            action[HEADER_ORIG] = originator; 
        }

        public byte GetOriginatorRaw(byte[] action)
        {
            return action[HEADER_ORIG];
        }

        public Originators GetOriginator(byte[] action)
        {
            return Originators.Parse(GetOriginatorRaw(action));
        }

        /// <summary>
        /// Sets the size header. Gets called automatically by SetPayload.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="size"></param>
        public void SetSize(ref byte[] action, int size)
        {
            byte[] sizeBytes = Int32ToBytes(size);
            for (int i = HEADER_SIZE; i < HEADER_TYPE; i++)
                action[i] = sizeBytes[i - HEADER_SIZE];
        }

        public int GetSize(byte[] action)
        {
            byte[] sizeBytes = new byte[4];
            for (int i = HEADER_SIZE; i < HEADER_TYPE; i++)
                sizeBytes[i - HEADER_SIZE] = action[i];

            return BytesToInt32(sizeBytes);
        }

        public void SetType(ref byte[] action, byte type)
        {
            action[HEADER_TYPE] = type;
        }

        public byte GetTypeRaw(byte[] action)
        {
            return action[HEADER_TYPE];
        }

        public Categories GetType(byte[] action)
        {
            return Categories.Parse(GetTypeRaw(action));
        }

        public void SetPayload(ref byte[] action, byte[] payload)
        {
            byte[] buffer = action;            
            action = new byte[action.Length + payload.Length];

            // Carry over the headers
            for (int i = 0; i < HEADER_PAYL; i++)
                action[i] = buffer[i];
            for (int i = HEADER_PAYL; i < payload.Length; i++)
                action[i] = payload[i - HEADER_PAYL];

            SetSize(ref action, payload.Length);
        }

        public byte[] GetPayload(byte[] action)
        {
            byte[] t = new byte[action.Length - HEADER_PAYL];
            for (int i = HEADER_PAYL; i < t.Length; i++)
            {
                t[i - HEADER_PAYL] = action[i];
            }

            return t; 
        }

        public byte[] Vector2ToBytes(Vector2 coords)
        {
            int x = (int)coords.X;
            int y = (int)coords.Y;

            byte[] bytesOut = new byte[4]; // first two octets for X, third and fourth for y coords.

            int X_FirstOctet = x & bitmask8;
            int X_SecondOctet = x & bitmask16;

            int Y_FirstOctet = y & bitmask8;
            int Y_SecondOctet = y & bitmask16;

            bytesOut[0] = Convert.ToByte(X_FirstOctet >> 0);
            bytesOut[1] = Convert.ToByte(X_SecondOctet >> 8);
            bytesOut[2] = Convert.ToByte(Y_FirstOctet >> 0);
            bytesOut[3] = Convert.ToByte(Y_SecondOctet >> 8);

            return bytesOut;
        }

        public Vector2 BytesToVector2(byte[] bytes)
        {
            if (bytes.Length != 4)
                throw new ArgumentException("bytes array was not formatted correctly.");

            int x = (bytes[0] << 0) | (bytes[1] << 8);
            int y = (bytes[2] << 0) | (bytes[3] << 8);

            return new Vector2(x, y);
        }

        public byte[] Vector3ToBytes(Vector3 coords)
        {
            byte[] bytes = new byte[6];
            Int16 X = (Int16)coords.X;
            Int16 Y = (Int16)coords.Y;
            Int16 Z = (Int16)coords.Z;

            bytes[0] = (byte)((X >> (8 * 0)) & bitmask8);
            bytes[1] = (byte)((X >> (8 * 1)) & bitmask8);


            bytes[2] = (byte)((Y >> (8 * 0)) & bitmask8);
            bytes[3] = (byte)((Y >> (8 * 1)) & bitmask8);


            bytes[4] = (byte)((Z >> (8 * 0)) & bitmask8);
            bytes[5] = (byte)((Z >> (8 * 1)) & bitmask8);

            return bytes;
        }

        public Vector3 BytesToVector3(byte[] bytes)
        {
            int X = (bytes[0] << 0) | (bytes[1] << 8);
            int Y = (bytes[2] << 0) | (bytes[3] << 8);
            int Z = (bytes[4] << 0) | (bytes[5] << 8);

            return new Vector3(X, Y, Z);
        }

        public byte[] Int32ToBytes(int value)
        {
            byte[] bytes = new byte[4];

            for (int i = 0; i < 4; i++)
                bytes[i] = (byte)((value >> (8 * i)) & bitmask8);

            return bytes;
        }

        public int BytesToInt32(byte[] bytes)
        {
            if (bytes.Length != 4)
                throw new ArgumentException("bytes array was not formatted correctly.");
            
            int value = 0;

            for (int i = 0; i < 4; i++)
                value = value | (bytes[i] << (8 * i));

            return value;
        }

        public void TestHarness()
        {
            byte[] TestBytes = Int32ToBytes(8267821);
            System.Diagnostics.Debug.WriteLine(BytesToInt32(TestBytes));
        }
    }
}

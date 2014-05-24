using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Networking.Structs;

namespace Networking
{
    /// <summary>
    /// A wrapper class to encapsulate data required for communication between a player and the server. 
    /// </summary>
    public class NetWrapper
    {
        /// <summary>
        /// The total number of bytes contained by the byte array Action object.
        /// Includes any dummy data sent with the object. Use NetWrapper.Size 
        /// if you want the exact size of relevent elements.
        /// </summary>
        public Int32 RawLength
        { 
            get { return RawLength; }
        }

        /// <summary>
        /// The size of the byte array as specified in the object's header.
        /// </summary>
        public Int32 Size
        {
            get 
            {                 
                return BitConverter.ToInt32(raw, iSIZE); 
            }
        }

        private Categories _type;
        /// <summary>
        /// Describes what type of action is contained.
        /// </summary>
        public Categories Type 
        {
            get { return _type; }
            protected set
            {
                raw[iTYPE] = value.ToByte();
                _type = value;
            }
        }

        private Originators _originator;
        /// <summary>
        /// Describes the sender of this action.
        /// </summary>
        public Originators Originator 
        {
            get { return _originator; }
            protected set
            {
                raw[iORIG] = value.ToByte();
                _originator = value;
            }
        }

        private Payload _data;
        /// <summary>
        /// The data encapsulated by this Action
        /// </summary>
        public Payload Data 
        {
            get { return _data; }
            protected set
            {
                byte[] payload = value.ToBytes();
                for (int i = iPAYL; (i - iPAYL) < value.Size; i++)
                    raw[i] = payload[i - iPAYL];

                _data = value;

                // update wrapper size to relfect new payload
                byte[] hSize = BitConverter.GetBytes(payload.Length + iPAYL + 1); // size of the payload plus payload starting point
                for (int i = iSIZE; i < 4; i++)
                    raw[i] = hSize[i - iSIZE];
            }
        }
        
        protected byte[] raw;

        #region Byte index constants
        /// <summary>
        /// Starting index of the size header
        /// </summary>
        public static byte iSIZE = 0;
        /// <summary>
        /// Starting index of the Categories header
        /// </summary>
        public static byte iTYPE = 4;
        /// <summary>
        /// Starting index of the Originators header
        /// </summary>
        public static byte iORIG = 5;
        /// <summary>
        /// Starting index of the payload
        /// </summary>
        public static byte iPAYL = 6;
        #endregion

        public NetWrapper(byte[] rawBytes)
        {
            raw = rawBytes;
            Type = Categories.Parse(raw[iTYPE]);
            Originator = Originators.Parse(raw[iORIG]);

            byte[] bytesPayload = new byte[this.Size - iPAYL - 1]; // total size of sent data minus where the payload begins
            for (int i = iPAYL; i < (this.Size - 1); i++)
                bytesPayload[i - iPAYL] = raw[i];

            if (Type == Categories.NetSprite)
                Data = new NetSprite(bytesPayload);
            if (Type == Categories.NetMouseState)
                Data = new NetMouseState(bytesPayload);
            if (Type == Categories.NetKeyboardState)
                Data = new NetKeyboardState(bytesPayload);
        }

        public NetWrapper(Categories type, Originators originator, Payload data)
        {
            raw = new byte[data.Size + iPAYL];
            Data = data;
            Type = type;
            Originator = originator;
        }

        public byte[] ToBytes()
        {
            if (this.Size < 1) throw new InvalidOperationException("Wrapper contained no/misformed data.");

            return raw;
        }

        public string DebugText()
        {
            StringBuilder s = new StringBuilder();
            s.Append("{ [" + this.RawLength.ToString() + "] ");
            s.Append("[" + this.Type.ToString() + "] ");
            s.Append("[" + this.Originator.ToString() + "] ");

            if (Data is NetMouseState)
            {
                NetMouseState p = (NetMouseState)Data;
                s.Append(p.DebugString() + " }");
                                
                return s.ToString();
            }
            if (Data is NetKeyboardState)
            {
                NetKeyboardState p = (NetKeyboardState)Data;
                s.Append(p.DebugString() + " }");

                return s.ToString();
            }
            if (Data is NetSprite)
            {
                NetSprite p = (NetSprite)Data;
                s.Append(p.DebugString() + " }");

                return s.ToString();
            }

            s.Append("Malformed Payload }");

            return s.ToString();
        }

        public static string DebugText(byte[] NetWrapperBytes)
        {
            NetWrapper w = new NetWrapper(NetWrapperBytes);
            return w.DebugText();
        }
    }
}

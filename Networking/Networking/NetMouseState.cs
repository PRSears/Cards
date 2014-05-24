using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace Networking
{
    /// <summary>
    /// Encapsulates a MouseState so it can be used as a payload in a NetWrapper.
    /// </summary>
    public class NetMouseState : Payload
    {        
        public MouseState State { get; private set; }

        private const byte LBUTTON = 0;
        private const byte RBUTTON = 1;
        private const byte MBUTTON = 2;

        #region Byte index constants
        /// <summary>
        /// Start index of the x coordinate
        /// </summary>
        private const byte iX = 0; 
        /// <summary>
        /// Start index of the y coordinate
        /// </summary>
        private const byte iY = 2; 
        /// <summary>
        /// Start index of the button state date
        /// </summary>
        private const byte iB = 4; 
        /// <summary>
        /// End index of this object
        /// </summary>
        private const byte iEND = 6;
        #endregion

        public NetMouseState(byte[] bytes) : base(bytes)
        {
            if (bytes.Length != (iEND + 1))
                throw new ArgumentException("bytes is not an acceptable length. Are you sure it's coming from a NetMouseState?");
        }

        public NetMouseState(MouseState state)
        {
            State = state;
            raw = new byte[iEND + 1];
            Initialize();
        }

        protected override void Initialize()
        {
            byte[] BytesX = BitConverter.GetBytes((Int16)State.X);
            byte[] BytesY = BitConverter.GetBytes((Int16)State.Y);
            byte[] BytesB = ButtonsDown();

            int i = 0;
            foreach (byte b in BytesX) raw[i++] = b;
            foreach (byte b in BytesY) raw[i++] = b;
            foreach (byte b in BytesB) raw[i++] = b;

            /* // Old method of adding data to raw[]
            for (int i = iX; i < iY; i++)
                raw[i] = BytesX[i - iX];
            for (int i = iY; i < iB; i++)
                raw[i] = BytesY[i - iY];
            for (int i = iB; i < raw.Length; i++)
                raw[i] = BytesB[i - iB];
             */
        }

        public override void Parse(byte[] data)
        {
            Int16 mx = BitConverter.ToInt16(raw, iX);
            Int16 my = BitConverter.ToInt16(raw, iY);
            ButtonState left = ((raw[iB + LBUTTON] == 1) ? ButtonState.Pressed : ButtonState.Released);
            ButtonState right = ((raw[iB + RBUTTON] == 1) ? ButtonState.Pressed : ButtonState.Released);
            ButtonState middle = ((raw[iB + MBUTTON] == 1) ? ButtonState.Pressed : ButtonState.Released);

            State = new MouseState(mx, my, 0, left, middle, right, ButtonState.Released, ButtonState.Released);
        }              

        public override string ToString()
        {
            return State.ToString();
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != this.GetType())
                return false;
            
            NetMouseState other = (NetMouseState)obj;

            return this.State.Equals(other.State);
        }

        private byte[] ButtonsDown()
        {
            byte[] buttonBytes = new byte[3];
            buttonBytes[LBUTTON] = ((State.LeftButton == ButtonState.Pressed) ? (byte)1 : (byte)0);
            buttonBytes[RBUTTON] = ((State.RightButton == ButtonState.Pressed) ? (byte)1 : (byte)0);
            buttonBytes[MBUTTON] = ((State.MiddleButton == ButtonState.Pressed) ? (byte)1 : (byte)0);

            return buttonBytes;
        }

        public static void TestHarness()
        {
            MouseState curMouseState = Mouse.GetState();
            System.Diagnostics.Debug.WriteLine(DebugString(curMouseState));

            NetMouseState testState1 = new NetMouseState(curMouseState);
            byte[] testBytes1 = testState1.ToBytes();
            System.Diagnostics.Debug.WriteLine(DebugString(testBytes1));
            NetMouseState testState1b = new NetMouseState(testBytes1);

            System.Diagnostics.Debug.WriteLine(testState1b.DebugString());
        }

        public static NetMouseState CreateTestNetMouseState()
        {
            MouseState curMouseState = Mouse.GetState();
            return new NetMouseState(curMouseState);
        }

        public override string DebugString()
        {
            StringBuilder s = new StringBuilder();
            s.Append("[ (" + State.X);
            s.Append(", " + State.Y);
            s.Append(") { " + State.LeftButton.ToString());
            s.Append(" , " + State.RightButton.ToString());
            s.Append(" , " + State.MiddleButton.ToString());
            s.Append(" } ]");

            return s.ToString();
        }

        private static string DebugString(MouseState state)
        {            
            StringBuilder s = new StringBuilder();
            s.Append("[ (" + state.X);
            s.Append(", " + state.Y);
            s.Append(") { " + state.LeftButton.ToString());
            s.Append(" , " + state.RightButton.ToString());
            s.Append(" , " + state.MiddleButton.ToString());
            s.Append(" } ]");

            return s.ToString();
        }

        private static string DebugString(byte[] bytes)
        {
            StringBuilder s = new StringBuilder();
            foreach (byte b in bytes)
                s.Append(b.ToString());
            return s.ToString();
        }
    }
}

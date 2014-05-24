using System;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Networking.Structs;

namespace Networking
{

    /// <summary>
    /// Encapsulates information about a sprite object so it can be used as a payload in a NetWrapper.
    /// </summary>
    public class NetSprite : Payload
    {
        private TextureID Texture;      // 7-bytes
        private byte RawScale;          // 1-byte
        private Int16 RawRotation;      // 2-bytes
        private Vector4 RawColor;       // 4-bytes

        public Vector3 Position         // 6-bytes
        { 
            get;
            private set;
        }
        public string Path { get { return Texture.GetPath(); } }
        public Single Scale
        {
            get
            {
                return ((Single)RawScale) / 10f;
            }
            private set
            {
                RawScale = (byte)Convert.ToSByte(value / 10);
            }
        }
        public Single Rotation 
        {
            get
            {
                return ((Single)RawRotation) / 100f;
            }
            private set
            {
                RawRotation = Convert.ToInt16(value / 100);
            }
        }
        public Color Tint 
        {
            get
            {
                return new Color(RawColor);
            }
            private set
            {
                RawColor = value.ToVector4();
            }
        }

        #region Byte index constants
        /// <summary>
        /// Start index of the TextureID
        /// </summary>
        private const byte ID_INDEX = 0;
        /// <summary>
        /// Start index of the Scale value
        /// </summary>
        private const byte SCALE_INDEX = 7;
        /// <summary>
        /// Start index of Rotation value
        /// </summary>
        private const byte ROTATION_INDEX = 8;
        /// <summary>
        /// Start index of ARGB color values
        /// </summary>
        private const byte COLOR_INDEX = 10;
        /// <summary>
        /// Start index of Position values
        /// </summary>
        private const byte POSITION_INDEX = 14;
        /// <summary>
        /// Final index. (Length - 1)
        /// </summary>
        private const byte END_INDEX = 19;
        #endregion

        public NetSprite(byte[] bytes)
            : base(bytes)
        {
            // The base calls Parse(bytes) 

            if (bytes.Length != (END_INDEX + 1))
                throw new ArgumentException("bytes is not an acceptable length.");
        }

        public NetSprite(TextureID textureID, Vector3 position, Single scale, Single rotation, Color tint)
        {
            Texture = textureID;
            Scale = scale;
            Rotation = rotation;
            Tint = tint;
            Position = position;

            raw = new byte[END_INDEX + 1];
            Initialize();
        }

        protected override void Initialize()
        {
            // Convert everything into bytes
            byte[] TextureBytes = Texture.ToBytes();
            byte[] ScaleBytes = new byte[1] { RawScale };
            byte[] RotationBytes = BitConverter.GetBytes(RawRotation);
            byte[] ARGB = new byte[4] { (byte)RawColor.W, (byte)RawColor.X, (byte)RawColor.Y, (byte)RawColor.Z };
            byte[] X = BitConverter.GetBytes((Int16)Position.X);
            byte[] Y = BitConverter.GetBytes((Int16)Position.Y);
            byte[] Z = BitConverter.GetBytes((Int16)Position.Z);

            // Combine all the byte arrays into base.raw
            int i = 0;

            foreach (byte b in TextureBytes) raw[i++] = b;
            foreach (byte b in ScaleBytes) raw[i++] = b;
            foreach (byte b in RotationBytes) raw[i++] = b;
            foreach (byte b in ARGB) raw[i++] = b;
            foreach (byte b in X) raw[i++] = b;
            foreach (byte b in Y) raw[i++] = b;
            foreach (byte b in Z) raw[i++] = b;
        }

        public override void Parse(byte[] data)
        {
            Texture = new TextureID(data, ID_INDEX);
            RawScale = data[SCALE_INDEX];
            RawRotation = BitConverter.ToInt16(data, ROTATION_INDEX);
            RawColor = new Vector4(
                (float)data[COLOR_INDEX], 
                (float)data[COLOR_INDEX + 1], 
                (float)data[COLOR_INDEX + 2], 
                (float)data[COLOR_INDEX + 3]);
            Position = new Vector3(
                BitConverter.ToInt16(raw, POSITION_INDEX), 
                BitConverter.ToInt16(raw, POSITION_INDEX + 2),
                BitConverter.ToInt16(raw, POSITION_INDEX + 4));
        }

        public override string DebugString()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            throw new NotImplementedException();
        }

        public override bool Equals(object obj)
        {
            throw new NotImplementedException();
        }
    }
}

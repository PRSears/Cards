using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Networking
{
    /// <summary>
    /// Encapulates data for a message between player and server. Used in the NetWrapper.
    /// </summary>
    public abstract class Payload
    {
        public Int32 Size { get { return raw.Length; } }
        protected byte[] raw;

        protected Payload(byte[] bytes)
        {
            raw = bytes;
            Parse(bytes);
        }

        protected Payload()
        {
            raw = new byte[0];
        }

        public virtual byte[] ToBytes()
        {
            return raw;
        }

        public virtual int ENDPOINT
        {
            get { return (raw.Length - 1); }
        }

        protected abstract void Initialize();
        public abstract void Parse(byte[] data);
        public abstract override string ToString();
        public abstract override bool Equals(object obj);
        public abstract string DebugString();
    }
}

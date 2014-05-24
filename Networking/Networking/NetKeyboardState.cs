using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Networking
{
    /// <summary>
    /// Encapsulates a KeyboardState so it can be used as a payload in a NetWrapper.
    /// </summary>
    public class NetKeyboardState : Payload
    {
        public NetKeyboardState(byte[] bytes)
            : base(bytes)
        {
            throw new NotImplementedException();
        }

        protected override void Initialize()
        {
        }

        public override void Parse(byte[] data)
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

        public override string DebugString()
        {
            throw new NotImplementedException();
        }
    }
}

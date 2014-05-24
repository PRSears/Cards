using System;

namespace Cards.Structs
{
    public struct Edge
    {
        public string Value { get; private set; }

        public static Edge Right { get { return new Edge("right"); } }
        public static Edge Left { get { return new Edge("left"); } }
        public static Edge Top { get { return new Edge("top"); } }
        public static Edge Bottom { get { return new Edge("bottom"); } }
        public static Edge None { get { return new Edge("none"); } }

        private Edge(string value) :this()
        {
            Value = value.ToLower();
        }

        public override string ToString()
        {
            return Value;
        }

        public override bool Equals(object obj)
        {
            if (this.GetType() != obj.GetType())
                return false;

            Edge other = (Edge)obj;
            if (!(this.Value.Equals(other.Value)))
                return false;

            return true;
        }
    }
}

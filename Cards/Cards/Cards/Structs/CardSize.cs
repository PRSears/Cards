using System;

namespace Cards.Structs
{
    public struct CardSize : IComparable
    {
        private float Size;

        public static CardSize Full { get { return new CardSize(1f); } }
        public static CardSize Bench { get { return new CardSize(0.23f); } }
        public static CardSize Deck { get { return new CardSize(0.20f); } }
        public static CardSize Prize { get { return new CardSize(0.2f); } }
        public static CardSize Active { get { return new CardSize(0.33f); } }
        public static CardSize Hand { get { return new CardSize(0.40f); } }

        private CardSize(float size)
        {
            Size = size;
        }

        public override int GetHashCode()
        {
            return Size.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (this.GetType() != obj.GetType())
                return false;

            CardSize objCardSize = (CardSize)obj;

            return Size.Equals(objCardSize.Size);
        }

        public override string ToString()
        {
            return Size.ToString();
        }

        public Single ToSingle()
        {
            return (Single)Size;
        }

        public int CompareTo(object obj)
        {
            if (obj.GetType() != this.GetType())
                throw new ArgumentException("obj is not a CardSize.");

            CardSize objCardSize = (CardSize)obj;

            return this.Size.CompareTo(objCardSize.Size);
        }

        #region operator overloads
        public static Boolean operator ==(CardSize a, CardSize b)
        {
            return (a.Equals(b));
        }
        public static Boolean operator !=(CardSize a, CardSize b)
        {
            return !(a == b);
        }
        public static Boolean operator >(CardSize a, CardSize b)
        {
            return (a.Size > b.Size);
        }
        public static Boolean operator <(CardSize a, CardSize b)
        {
            return (a.Size < b.Size);
        }
        public static Boolean operator >=(CardSize a, CardSize b)
        {
            return (a.Size >= b.Size);
        }
        public static Boolean operator <=(CardSize a, CardSize b)
        {
            return (a.Size <= b.Size);
        }
        public static float operator +(float a, CardSize b)
        {
            return (a + b.Size);
        }
        public static float operator -(float a, CardSize b)
        {
            return (a - b.Size);
        }
        public static float operator *(float a, CardSize b)
        {
            return (a * b.Size);
        }
        public static float operator /(float a, CardSize b)
        {
            return (a / b.Size);
        }
        public static double operator %(float a, CardSize b)
        {
            return (a % b.Size);
        }
        public static float operator +(CardSize a, float b)
        {
            return (a.Size + b);
        }
        public static float operator -(CardSize a, float b)
        {
            return (a.Size - b);
        }
        public static float operator *(CardSize a, float b)
        {
            return (a.Size * b);
        }
        public static float operator /(CardSize a, float b)
        {
            return (a.Size / b);
        }
        public static double operator %(CardSize a, float b)
        {
            return (a.Size % b);
        }
        public static float operator +(CardSize a, CardSize b)
        {
            return (a.Size + b.Size);
        }
        public static float operator -(CardSize a, CardSize b)
        {
            return (a.Size - b.Size);
        }
        public static float operator *(CardSize a, CardSize b)
        {
            return (a.Size * b.Size);
        }
        public static float operator /(CardSize a, CardSize b)
        {
            return (a.Size / b.Size);
        }
        public static double operator %(CardSize a, CardSize b)
        {
            return (a.Size % b.Size);
        }
        #endregion
    }
}

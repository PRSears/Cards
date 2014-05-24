using System;

namespace Cards.Structs
{
    public struct CardPlacement
    {
        private string Placement;

        public static CardPlacement Hand
        {
            get { return new CardPlacement("hand"); }
        }
        public static CardPlacement Deck
        {
            get { return new CardPlacement("deck"); }
        }
        public static CardPlacement Bench
        {
            get { return new CardPlacement("bench"); }
        }
        public static CardPlacement Prize
        {
            get { return new CardPlacement("prize"); }
        }
        public static CardPlacement Discard
        {
            get { return new CardPlacement("discard"); }
        }
        public static CardPlacement Playing
        {
            get { return new CardPlacement("playing"); }
        }

        private CardPlacement(string placement)
        {
            Placement = placement;
        }
        
        public override int GetHashCode()
        {
            return Placement.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (this.GetType() != obj.GetType())
                return false;
            
            CardPlacement oCardPlacement = (CardPlacement)obj;

            return Placement.Equals(oCardPlacement.Placement);
        }

        public override string ToString()
        {
            return Placement;
        }

        public static Boolean operator ==(CardPlacement a, CardPlacement b)
        {
            return (a.Equals(b));
        }

        public static Boolean operator !=(CardPlacement a, CardPlacement b)
        {
            return !(a == b);
        }
    }
}

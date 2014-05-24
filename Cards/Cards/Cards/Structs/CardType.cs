using System;

namespace Cards.Structs
{
    public struct CardType
    {
        public string CardKind { get; private set; }
        public string CardColor { get; private set; }

        // ------------------------------------------------------------------------------------------- //
        // If this list of static types is altered, remember to change the FromString method below.
        // ------------------------------------------------------------------------------------------- //

        public static CardType Green { get { return new CardType("none", "green"); } }
        public static CardType Red { get { return new CardType("none", "red"); } }
        public static CardType Blue { get { return new CardType("none", "blue"); } }
        public static CardType Yellow { get { return new CardType("none", "yellow"); } }
        public static CardType Purple { get { return new CardType("none", "purple"); } }
        public static CardType Brown { get { return new CardType("none", "brown"); } }
        public static CardType Black { get { return new CardType("none", "black"); } }
        public static CardType Silver { get { return new CardType("none", "silver"); } }
        public static CardType Colorless { get { return new CardType("none", "colorless"); } }
        public static CardType Pokemon { get { return new CardType("pokemon", "none"); } }
        public static CardType Trainer { get { return new CardType("trainer", "none"); } }
        public static CardType Supporter { get { return new CardType("supporter", "none"); } }
        public static CardType Stadium { get { return new CardType("stadium", "none"); } }
        public static CardType Energy { get { return new CardType("energy", "none"); } }
        public static CardType None { get { return new CardType("none", "none"); } }

        private CardType(string type, string color) :this()
        {
            CardKind = type;
            CardColor = color;
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        public override bool Equals(Object o)
        {
            if (this.GetType() != o.GetType())
                return false;

            CardType other = (CardType)o;
            if (!(this.CardKind.Equals(other.CardKind)))
                return false;
            if (this.CardColor.Equals(other.CardColor))
                return false;
            return true;
        }

        public override string ToString()
        {
            string o = String.Format("[ {0} : {1} ]", CardKind, CardColor);
            return o;
        }

        public static CardType FromString(string cardKind, string cardColor)
        {
            CardType t;

            // Match the type of card
            if (cardKind.Equals(CardType.Pokemon.CardKind))
                t = CardType.Pokemon;
            else if (cardKind.Equals(CardType.Trainer.CardKind))
                t = CardType.Trainer;
            else if (cardKind.Equals(CardType.Supporter.CardKind))
                t = CardType.Supporter;
            else if (cardKind.Equals(CardType.Stadium.CardKind))
                t = CardType.Stadium;
            else if (cardKind.Equals(CardType.Energy.CardKind))
                t = CardType.Energy;
            else
            {
                System.Diagnostics.Debug.WriteLine("Refer to PokeCards.Structs.CardType for a list of acceptable values.");
                throw new ArgumentException("cardKind has an unexpected value: " + cardKind);
            }

            // Match the color of the card
            if (cardColor.Equals(CardType.Green.CardColor))
                t += CardType.Green;
            else if (cardColor.Equals(CardType.Red.CardColor))
                t += CardType.Red;
            else if (cardColor.Equals(CardType.Blue.CardColor))
                t += CardType.Blue;
            else if (cardColor.Equals(CardType.Yellow.CardColor))
                t += CardType.Yellow;
            else if (cardColor.Equals(CardType.Purple.CardColor))
                t += CardType.Purple;
            else if (cardColor.Equals(CardType.Brown.CardColor))
                t += CardType.Brown;
            else if (cardColor.Equals(CardType.Black.CardColor))
                t += CardType.Black;
            else if (cardColor.Equals(CardType.Silver.CardColor))
                t += CardType.Silver;
            else if (cardColor.Equals(CardType.Colorless.CardColor))
                t += CardType.Colorless;
            else if (cardColor.Equals(CardType.None.CardColor))
                t += CardType.None;
            else
            {
                System.Diagnostics.Debug.WriteLine("Refer to PokeCards.Structs.CardType for a list of acceptable values.");
                throw new ArgumentException("cardColor has an unexpected value: " + cardColor);
            }

            return t;
        }

        public static Boolean operator ==(CardType a, CardType b)
        {
            return (a.Equals(b));
        }

        public static Boolean operator !=(CardType a, CardType b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Replaces any "none" field with the correstpoding field from the other object
        /// </summary>
        /// <returns></returns>
        public static CardType operator +(CardType a, CardType b)
        {
            if (a.CardColor.Equals("none"))
            {
                a.CardColor = b.CardColor;
                return a;
            }
            if (a.CardKind.Equals("none"))
            {
                a.CardKind = b.CardKind;
                return a;
            }

            if (b.CardColor.Equals("none"))
            {
                b.CardColor = a.CardColor;
                return b;
            }
            if (b.CardKind.Equals("none"))
            {
                b.CardKind = a.CardKind;
                return b;
            }

            return a;
        }
    }
}

using System;

namespace Cards.Structs
{
    public struct CardStatus
    {
        private string Condition;
        private float _rotation;

        public float Rotation { get { return _rotation; } }
        
        public static CardStatus Poison { get { return new CardStatus("poison", 0f); } }
        public static CardStatus Paralyzed { get { return new CardStatus("paralyzed", (float)(Math.PI / 8)); } }
        public static CardStatus Confused { get { return new CardStatus("confused", (float)(Math.PI / 4)); } }
        public static CardStatus Burned { get { return new CardStatus("burned", 0f); } }
        public static CardStatus Asleep { get { return new CardStatus("asleep", (float)(3*(Math.PI) / 2)); } }

        private CardStatus(string condition, float rotation)
        {
            Condition = condition;
            _rotation = rotation;
        }

        public override bool Equals(object obj)
        {
            if (this.GetType() != obj.GetType())
                return false;

            CardStatus objCardStatus = (CardStatus)obj;
            return this.Condition.Equals(objCardStatus.Condition);
        }

        public override string ToString()
        {
            return Condition + Rotation.ToString();
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        public static Boolean operator ==(CardStatus a, CardStatus b)
        {
            return a.Equals(b);
        }

        public static Boolean operator !=(CardStatus a, CardStatus b)
        {
            return !(a == b);
        }
    }
}

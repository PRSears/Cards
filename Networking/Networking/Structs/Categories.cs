using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Networking.Structs
{
    /// <summary>
    /// NetWrapper catagory type. 
    /// </summary>
    public struct Categories
    {
        private byte Category;
        private string Description;

        public static Categories NetMouseState { get { return new Categories(0, "Mouse Input"); } }
        public static Categories NetKeyboardState { get { return new Categories(1, "Keyboard Input"); } } 
        public static Categories NetSprite { get { return new Categories(2, "NetSprite: Interface Update"); } }
        public static Categories TextureSync { get { return new Categories(3, "Texture Sync"); } }
        public static Categories DeckSync { get { return new Categories(4, "Deck Sync"); } }

        private Categories(byte category, string description)
        {
            Category = category;
            Description = description;
        }

        public static Categories Parse(byte category)
        {
            if (category == NetMouseState.Category)
                return NetMouseState;
            if (category == NetKeyboardState.Category)
                return NetKeyboardState;
            if (category == NetSprite.Category)
                return NetSprite;
            if (category == TextureSync.Category)
                return TextureSync;
            if (category == DeckSync.Category)
                return DeckSync;

            throw new FormatException("Category byte is not a valid category.");
        }

        public override bool Equals(object obj)
        {
            if (this.GetType() != obj.GetType())
                return false;
            Categories other = (Categories)obj;

            return Category.Equals(other.Category);
        }

        public override string ToString()
        {
            return Description;
        }

        public byte ToByte()
        {
            return Category;
        }

        public static Boolean operator ==(Categories a, Categories b)
        {
            return a.Equals(b);
        }

        public static Boolean operator !=(Categories a, Categories b)
        {
            return !a.Equals(b);
        }
    }
}

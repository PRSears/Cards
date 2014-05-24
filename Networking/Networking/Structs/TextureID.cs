using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Networking.Structs
{
    /// <summary>
    /// Unique identifier of a Card texture. Combines content folder name and card number.
    /// </summary>
    public struct TextureID
    {
        private string Folder;
        private int CardNum;

        /// <summary>
        /// Constructs a new TextureID from a folder and a card number.
        /// </summary>
        /// <param name="folder">Folder in which (deck type) the card resides.</param>
        /// <param name="cardNumber">Card identification number.</param>
        public TextureID(string folder, int cardNumber)
        {
            Folder = folder;
            CardNum = cardNumber;

            if ((Folder.Length > 4) || (Folder.Length < 1)) throw new ArgumentOutOfRangeException("Folder id is out of range. Must be of length (0,4]");
            if (CardNum > 999 || CardNum < 0) throw new ArgumentOutOfRangeException("CardNum is out of range. Must be in [0,999]"); 
        }

        /// <summary>
        /// Constructs a new TextureID based on a byte array representation of another TextureID. 
        /// Generate byte array by using TextureID.ToBytes().
        /// </summary>
        /// <param name="textureIdBytes">Byte array to convert to new TextureID.</param>
        public TextureID(byte[] textureIdBytes)
        {
            Folder = TextureID.Parse(textureIdBytes).Folder;
            CardNum = TextureID.Parse(textureIdBytes).CardNum;

            if ((Folder.Length > 4) || (Folder.Length < 1)) throw new ArgumentOutOfRangeException("Folder id is out of range. Must be of length (0,4]");
            if (CardNum > 999 || CardNum < 0) throw new ArgumentOutOfRangeException("CardNum is out of range. Must be in [0,999]"); 
        }

        /// <summary>
        /// Constructs a new TextureID based on a raw byte array representation of a NetWrapper (Action object).
        /// </summary>
        /// <param name="raw">Raw byte array to convert to new TextureID.</param>
        /// <param name="startingIndex">The (0-based) position in the array of the first byte of the payload.</param>
        public TextureID(byte[] raw, int startingIndex)
        {
            byte[] textureIdBytes = new byte[7];
            Array.Copy(raw, startingIndex, textureIdBytes, 0, textureIdBytes.Length);

            Folder = TextureID.Parse(textureIdBytes).Folder;
            CardNum = TextureID.Parse(textureIdBytes).CardNum;

            if ((Folder.Length > 4) || (Folder.Length < 1)) throw new ArgumentOutOfRangeException("Folder id is out of range. Must be of length (0,4]");
            if (CardNum > 999 || CardNum < 0) throw new ArgumentOutOfRangeException("CardNum is out of range. Must be in [0,999]"); 
        }

        /// <summary>
        /// Returns a new TextureID object generated from a byte array representation of a TextureID.
        /// </summary>
        /// <param name="textureIdBytes">Byte array to convert to new TextureID.</param>
        /// <returns>Returns a new TextureID object generated from a byte array representation of a TextureID.</returns>
        public static TextureID Parse(byte[] textureIdBytes)
        {
            ASCIIEncoding encoder = new ASCIIEncoding();

            return TextureID.Parse(encoder.GetString(textureIdBytes));
        }

        /// <summary>
        /// Returns a new TextureID object generated from a string comrised of both the folder name and card ID.
        /// </summary>
        /// <param name="longString">String should be of the form ffffccc, 
        /// where 'ffff' is a four character long folder name, and 'ccc' is a three digit card ID.</param>
        /// <returns>Returns a new TextureID object generated from a string.</returns>
        public static TextureID Parse(string longString)
        {
            string folder = longString.Substring(0, 4);
            int cardNum = int.Parse(longString.Substring(3, 3));

            return new TextureID(folder, cardNum);
        }

        /// <summary>
        /// Converts this TextureID struct to an array of bytes.
        /// </summary>
        /// <returns>Byte array representation of this TextureID.</returns>
        public byte[] ToBytes()
        {
            ASCIIEncoding encoder = new ASCIIEncoding();
            return encoder.GetBytes(this.ToString());
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != this.GetType())
                return false;

            TextureID other = (TextureID)obj;

            if (!this.Folder.Equals(other.Folder))
                return false;
            if (this.CardNum != other.CardNum)
                return false;

            return true;
        }

        public override string ToString()
        {
            StringBuilder s = new StringBuilder();
            s.Append(Folder.PadLeft(4, '>'));
            s.Append(CardNum.ToString().PadLeft(3, '0'));

            return s.ToString();
        }

        public string GetPath()
        {
            // TODO implement GetPath()
            throw new NotImplementedException();
        }

        public static Boolean operator ==(TextureID a, TextureID b)
        {
            return a.Equals(b);
        }

        public static Boolean operator !=(TextureID a, TextureID b)
        {
            return !a.Equals(b);
        }
    }
}

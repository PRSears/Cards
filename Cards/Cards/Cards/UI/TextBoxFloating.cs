using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Cards.IInteractable;
using Cards.Structs;
using Cards.UI;
using Cards.KeyboardHandling;

namespace Cards.UI
{
    class TextBoxFloating : Button
    {
        public List<string> Text { get; set; }
        public float Padding { get; set; }
        protected string FontName;
        protected SpriteFont ContentsFont;

        public override Rectangle HitBox
        {
           get
            {
               // Calculate the space occupied up by the text
               Rectangle R = new Rectangle((int)this.Position.X, (int)this.Position.Y, 0, 0);
               foreach (string line in Text)
               {
                   Vector2 LineRect = ContentsFont.MeasureString(line);
                   R.X = (LineRect.X > R.X) ? (int)LineRect.X : R.X;
                   R.Y += (int)LineRect.Y;
               }
               return R;
           }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="initialText"></param>
        /// <param name="position"></param>
        /// <param name="game"></param>
        public TextBoxFloating(string[] initialText, Vector2 position, string font, Color color, PokemonCardGame game)
            : base(game)
        {
            Text = new List<string>();
            foreach (string s in initialText)
                Text.Add(s);
            FontName = font;
            FontColor = color;
            Padding = 15;
            Position = position;
            Visible = true;
            ZIndex = GameConstants.RENDER_INTERFACE_LEVEL;
        }

        public override void LoadContent(ContentManager content)
        {
            ContentsFont = currentGame.Content.Load<SpriteFont>(FontName);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // NOTE is it neccessary to recalculate on every draw call? It's possible the font changes size/style.
            float spacing = ContentsFont.MeasureString(Text[0]).Y + Padding;
            for(int i = 0; i < Text.Count; i++)
            {
                spriteBatch.DrawString(ContentsFont, Text[i], new Vector2(Position.X, Position.Y + (spacing * i)), FontColor);
            }
        }

        public override string ToString()
        {
            StringBuilder strOut = new StringBuilder();
            strOut.Append("[");
            strOut.Append(Position.ToString());
            strOut.Append("]");
            foreach (string line in Text)
                strOut.Append(line);

            return strOut.ToString();
        }

        public override bool Equals(object obj)
        {
            if (this.GetType() != obj.GetType())
                return false;

            TextBoxFloating other = (TextBoxFloating)obj;

            if (!this.Text.Equals(other.Text))
                return false;
            if (!this.Position.Equals(other.Position))
                return false;
            if (!this.FontName.Equals(other.FontName))
                return false;

            return true;
        }
    }
}

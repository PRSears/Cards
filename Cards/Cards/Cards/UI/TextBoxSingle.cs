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
    public class TextBoxSingle : Button
    {
        public string Text { get; protected set; }
        protected string InitialText;
        protected SpriteFont ContentsFont;
        
        public TextBoxSingle(string initialText, Vector2 position, PokemonCardGame game) : base(game)
        {
            Text = InitialText = initialText;

            TextureAsset = "TextBoxSingle";
            ContentsFont = game.Content.Load<SpriteFont>("UIFont");
            Position = position;
            Visible = true;
            Active = false;
            ZIndex = GameConstants.RENDER_INTERFACE_LEVEL;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Hovering = Active || Hovering;
            base.Draw(spriteBatch);
            spriteBatch.DrawString(ContentsFont, Text, new Vector2(Position.X + 34, Position.Y + 14), Color.White);
        }

        public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
                return false;

            TextBoxSingle other = (TextBoxSingle)obj;

            if (this.Text.Equals(other.Text))
                return true;
            return false;
        }

        public virtual void EnterText(Keys key)
        {
            //System.Diagnostics.Debug.WriteLine("EnterText[" + key.ToString() + "]");

            if (Text.Equals(InitialText))
                Text = "";

            if (key.Equals(Keys.Back)) // backspace removes characters
            {
                if (Text.Length > 0)
                    Text = Text.Remove(Text.Length - 1);
                else
                    Text = InitialText;
            }
            else
            {
                string newChar = FormatKey(key);

                // Ignore overflow
                if (Text.Length > 40)
                    return;

                Text += (Text.Length > 0) ? newChar.ToLower() : newChar;
            }
            System.Diagnostics.Debug.WriteLine(Text);
        }

        private string FormatKey(Keys key)
        {
            string t = key.ToString();
            
            // Handle special buttons
            if (t.Length > 1)
            {
                if (key.Equals(Keys.Decimal)) return ".";
                if (key.Equals(Keys.OemPeriod)) return ".";
                if (key.Equals(Keys.Space)) return " ";
                if (key.Equals(Keys.Divide)) return "/";
                if (key.Equals(Keys.Add)) return "+";
                if (key.Equals(Keys.Subtract)) return "-";
                if (key.Equals(Keys.Multiply)) return "*";
                if (key.Equals(Keys.OemQuestion)) return "?";

                if (t.Contains("NumPad"))
                {
                    t = t.Replace("NumPad", "");
                    return t;
                }
                if ((t.Length == 2) && t.Contains("D"))
                {
                    t = t.Replace("D", "");
                    return t;
                }
            }
            else
            {
                return t;
            }

            return ""; // unsupported key
        }
    }
}

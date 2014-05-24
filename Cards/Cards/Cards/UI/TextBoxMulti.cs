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
    public class TextBoxMulti : TextBoxSingle
    {
        public string[] Lines { get; protected set; }
        protected float Padding { get; private set; }
        protected string FontName;
        protected Color FontColor;
        protected SpriteFont ContentsFont;

        public TextBoxMulti(string initialText, Vector2 position, PokemonCardGame game)
            : base(initialText, position, game)
        {
            Lines = new string[7];
            Lines[0] = InitialText;

            TextureAsset = "TextBox";
            FontName = "UIFont";
            FontColor = Color.White;
            Padding = 3;
        }

        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);
            ContentsFont = currentGame.Content.Load<SpriteFont>(FontName);
        }

        public override void EnterText(Keys key)
        {
            if (key.Equals(Keys.Enter))
            {
                SendChat(Text);
                Text = InitialText;
                return;
            }
            base.EnterText(key);
            Lines[0] = Text;
        }

        private void SendChat(string text)
        {
            // Do nothing if the default text is still entered (player hasn't actually typed anything).
            if (text == InitialText)
                return;

            for (int i = Lines.Length - 1; i > 1; i--)
            {
                Lines[i] = Lines[i - 1];
            }

            Lines[1] = Filter(text);
            Lines[0] = InitialText;
        }

        private string Filter(string text)
        {
            string t = text.ToLower();
            if (t.Contains("so fucking"))
                return t.Replace("so fucking", "sofa king");
            if (t == "easter egg")
                return "0110001001101100011101010110010101110000\n011010010110110001101100";
            if (t == "this game sucks")
                return "No, you suck.";            

            return text;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Hovering = Active || Hovering;
            if (Visible)
            {
                spriteBatch.Draw(Hovering ? ButtonHoverTexture : ButtonTexture, Position, null, Disabled ? Color.Gray : Color.White,
                    0f, Vector2.Zero, 1f, SpriteEffects.None, ZIndex);

                float spacing = ContentsFont.MeasureString(InitialText).Y + Padding;
                for (int i = 0; i < Lines.Length; i++)
                {
                    if (Lines[i] != null)
                    {
                        spriteBatch.DrawString(ContentsFont, Lines[i],
                            new Vector2(Position.X + 24, Position.Y + Height - 34 - (spacing * i)), 
                            FontColor);
                    }
                }
            }
            Hovering = Active;

        }
    }
}

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

namespace Cards.UI
{
    public class Button : IClickable
    {
        protected string TextureAsset;
        private SpriteFont LabelFont;
        protected Texture2D ButtonTexture;
        protected Texture2D ButtonHoverTexture;
        protected PokemonCardGame currentGame;

        private Vector2 _Position;
        public Vector2 Position
        {
            get
            {
                return _Position;
            }
            set
            {
                _Position = value;
                _PositionBox.X = (int)_Position.X;
                _PositionBox.Y = (int)_Position.Y;

                // Make sure the button is still positioned on the screen.
                Rectangle w = new Rectangle(0, 0, currentGame.Window.ClientBounds.Width, currentGame.Window.ClientBounds.Height);
                if (!((_PositionBox.Left < w.Right) && (_PositionBox.Right > w.Left) && (_PositionBox.Top < w.Bottom) && (_PositionBox.Bottom > w.Top)))
                {
                    // no intersection, reset position
                    _Position = new Vector2(0, 0);
                    _PositionBox.X = (int)_Position.X;
                    _PositionBox.Y = (int)_Position.Y;
                }
            }
        }
        public Color BackgroundColor 
        { 
            get; set; 
        }
        public Color HoverColor
        {
            get;
            set;
        }
        private string _Label;
        public string Label 
        {
            get
            {
                return _Label;
            }
            set
            {
                _Label = (value != null ? value.ToLower() : null);
            }
        }
        public Color FontColor 
        { 
            get; set; 
        }
        private Rectangle _PositionBox;
        public virtual Rectangle HitBox
        {
            get
            {
                return _PositionBox; 
            }
        }
        [System.Obsolete("Use Rectangle HitBox.Width property instead.", false)]
        public int Width
        {
            get 
            {
                if (ButtonTexture == null)
                    return _PositionBox.Width;
                else
                    return ButtonTexture.Width;
            }
        }
        [System.Obsolete("Use Rectangle HitBox.Height property instead.", false)]
        public int Height
        {
            get
            {
                if (ButtonTexture == null)
                    return _PositionBox.Height;
                else
                    return ButtonTexture.Height;
            }
        }
        public float ZIndex
        {
            get;
            set;
        }
        public bool Visible
        {
            get;
            set;
        }
        public bool Hovering
        {
            get;
            protected set;
        }
        public bool Active
        { 
            get; set; 
        }
        public bool Disabled 
        { 
            get; set; 
        }

        protected Button(PokemonCardGame game)
        {
            currentGame = game;
            BackgroundColor = Color.White; // Apply no tint so the button is fully visible by default
        }

        /// <summary>
        /// Constructs a new textured button. 
        /// </summary>
        /// <param name="asset">Path to the texture asset for this button.</param>
        /// <param name="position">Vector to position the button at.</param>
        /// <param name="visible">Whether or not you want the button to initially be visiible.</param>
        /// <param name="game">Game instance.</param>
        public Button(string asset, Vector2 position, bool visible, PokemonCardGame game)
            : this(game)
        {
            TextureAsset = asset;
            Position = position;
            Active = false;
            Visible = visible;
            ZIndex = GameConstants.RENDER_INTERFACE_LEVEL;
        }
        
        /// <summary>
        /// Construcs a new button with optional parameters.
        /// </summary>
        /// <param name="game">Game instance.</param>
        /// <param name="position">Position to place the button inside. If not using default 
        /// textures make sure to specify a width and height of the rectangle.</param>
        /// <param name="label">Text to overlay on the button.</param>
        /// <param name="labelColor">Color of overlaid text.</param>
        /// <param name="colorOverlay">Color to tint the button with. If no texture is used 
        /// this is the background color for the button.</param>
        /// <param name="hoverColor">Color to overlay the button with on mouse hover.</param>
        /// <param name="asset">Path to the texture asset for this button.</param>
        /// <param name="visible">Whether or not you want the button to initially be visiible.</param>
        public Button(PokemonCardGame game, Rectangle? position = null, string label = null, Color? labelColor = null, Color? colorOverlay = null, Color? hoverColor = null, string asset = null, bool visible = true)
            : this(asset, new Vector2(0, 0), visible, game)
        {
            _PositionBox = position ?? new Rectangle(0, 0, 295, 135); //HACK default texture width/height... 
            Position = new Vector2(_PositionBox.X, _PositionBox.Y);
            BackgroundColor = colorOverlay ?? Color.White;
            HoverColor = hoverColor ?? Color.LightSlateGray;
            Label = label;
            FontColor = labelColor ?? Color.Black;
        }

        public virtual void LoadContent(ContentManager content)
        {
            if (TextureAsset != null)
            {
                ButtonTexture = content.Load<Texture2D>(@"ui\" + TextureAsset);
                ButtonHoverTexture = content.Load<Texture2D>(@"ui\" + TextureAsset + @"Hover");

                _PositionBox.Width = ButtonTexture.Width;
                _PositionBox.Height = ButtonTexture.Height;
            }
            else
            {
                // Create dummy textures to render simple coloured box
                ButtonTexture = new Texture2D(currentGame.GraphicsDevice, 1, 1);
                ButtonTexture.SetData(new Color[] { Color.White });
                ButtonHoverTexture = new Texture2D(currentGame.GraphicsDevice, 1, 1);
                ButtonHoverTexture.SetData(new Color[] { HoverColor });
            }
            if (Label != null)
            {
                LabelFont = currentGame.Content.Load<SpriteFont>("ButtonFont");
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (Visible)
            {
                spriteBatch.Draw(
                    (Hovering && !Disabled)? ButtonHoverTexture : ButtonTexture,    // Texture2D texture
                    HitBox,                                                         // Rectangle destinationRectangle
                    null,                                                           // Rectangle? sourceRectangle
                    Disabled ? Color.Gray : BackgroundColor,                        // Color tint color
                    0f,                                                             // float rotation
                    Vector2.Zero,                                                   // Vector2 origin
                    SpriteEffects.None,                                             // SpriteEffects effects
                    ZIndex);                                                        // float layerDepth

                if (LabelFont != null)
                {
                    spriteBatch.DrawString(LabelFont, Label, CenterText(this.HitBox, Label, LabelFont), FontColor);
                }
            }
            Hovering = Active;
        }

        /// <summary>
        /// Check if this button is using a texture.
        /// </summary>
        /// <returns>Returns true if a texture has been specified.</returns>
        protected virtual bool HasTexture()
        {
            if (TextureAsset != null)
                return true;
            return false;
        }

        /// <summary>
        /// Finds the position to draw a string at in order to appear centered in a given rectangle
        /// </summary>
        /// <param name="box">Rectangle to center the text in</param>
        /// <param name="text">String that is being drawn</param>
        /// <param name="font">Font being used to draw the text</param>
        /// <returns></returns>
        private Vector2 CenterText(Rectangle box, string text, SpriteFont font)
        {
            Vector2 centerPos = new Vector2(box.X, box.Y);
            // Make sure the font exists
            if (font != null) // Can't center the text if the font hasn't been loaded
            {
                Vector2 TextSize = LabelFont.MeasureString(Label);
                Rectangle TextArea = new Rectangle(0, 0, (int)TextSize.X, (int)TextSize.Y);
                Vector2 TabelOffset = new Vector2(TextArea.Center.X, TextArea.Center.Y);

                Vector2 HitBoxCenter = new Vector2(HitBox.Center.X, HitBox.Center.Y);

                centerPos = (HitBoxCenter - TabelOffset);
            }

            return centerPos;
        }

        public override bool Equals(object obj)
        {
            if (this.GetType() != obj.GetType())
                return false;

            Button other = (Button)obj;

            if (TextureAsset != null) 
                if (!(this.TextureAsset.Equals(other.TextureAsset)))
                    return false;
            if (Label != null) 
                if (!this.Label.Equals(other.Label))
                    return false;
            if (!(this.BackgroundColor.Equals(other.BackgroundColor)))
                return false;
            if (!(this.HitBox.Equals(other.HitBox)))
                return false;
            if (this.Visible != other.Visible)
                return false;

            return true;
        }

        public override string ToString()
        {
            return ("[ " + TextureAsset + " " + (Label ?? "n/a") + ", " + Position.ToString() + (Visible ? ", visible]" : ", hidden]"));
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        public virtual bool IsMouseOver(MouseState mouse)
        {
            if ((mouse.X > HitBox.Left) && (mouse.X < HitBox.Right) && (mouse.Y > HitBox.Top) && (mouse.Y < HitBox.Bottom))
                return true;
            return false;
        }

        public int CompareTo(IClickable other)
        {
            return this.ZIndex.CompareTo(other.ZIndex);
        }

        public void Hover()
        {
            Hovering = true;
        }

    }
}

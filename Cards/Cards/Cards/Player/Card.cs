using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Cards.Structs;
using Cards.IInteractable;

namespace Cards.Player
{
    public class Card : Comparer<IClickable>, IComparable<IClickable>, IMovable
    {
        //private const string BACKGROUND_ASSET = "000-cardback";
        private bool debug = true;

        private string _assetName;
        private Vector2 _position;
        private Vector2 _prevPosition;
        private bool _active;
        private PokemonCardGame _game;
        private Texture2D AltTexture;
        private int _pokemonStage;
        private IMovable _attachedNext;
        private IMovable _attachedPrev;

        #region Card render-specific properties
        /// <summary>
        /// Texture2D image to draw on the card (primary state)
        /// </summary>
        public Texture2D CardTexture { 
            get; 
            private set; 
        }
        /// <summary>
        /// Gets or sets the name of the card texture asset
        /// </summary>
        public string AssetName
        {
            get { return _assetName; }
            set
            {
                string v = value;
                if (Path.HasExtension(v))
                    v = Path.GetFileNameWithoutExtension(v);
                _assetName = v;
            }
        }
        /// <summary>
        /// Gets or sets the position of the card on the playing area (top left origin)
        /// </summary>
        public Vector2 Position
        {
            get
            {
                return _position;
            }
            set
            {
                try
                {
                    if (value.X < 0)
                        value.X = 0;
                    else if (value.X > (_game.Window.ClientBounds.Width - Width))
                        value.X = _game.Window.ClientBounds.Width - Width;

                    if (value.Y < 0)
                        value.Y = 0;
                    else if (value.Y > (_game.Window.ClientBounds.Height - Height))
                        value.Y = _game.Window.ClientBounds.Height - Height;
                }
                catch (Exception)
                {
                    
                }

                // _prevPosition = _position; // should onle use prev position on zoom, and when 'snapping' to a new location
                _position = value;
            }
        }
        /// <summary>
        /// Get the prior position of the card
        /// </summary>
        public Vector2 PrevPosition
        {
            get { return _prevPosition; }
            set { _prevPosition = value; }
        }
        /// <summary>
        /// Gets or sets the previous ZIndex value of this card
        /// </summary>
        public float PrevZIndex
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the z-Index (depth value) of the card
        /// </summary>
        public float ZIndex { 
            get; 
            set; 
        }
        /// <summary>
        ///  Gets whether the card is face up.
        /// </summary>
        public bool Active
        {
            get { return _active; }
            set { _active = value; }
        }
        /// <summary>
        /// Gets or sets whether the user is hovering 
        /// </summary>
        public bool Hovering
        {
            get;
            private set;
        }
        #endregion
        #region Card movement-specific properties
        /// <summary>
        /// the width of the card texture
        /// </summary>
        public int Width
        {
            get { return (int)(CardTexture.Width * Size.ToSingle()); }
        }
        /// <summary>
        /// the height of the card texture
        /// </summary>
        public int Height
        {
            get { return (int)(CardTexture.Height * Size.ToSingle()); }
        }
        /// <summary>
        /// Stores the zoom level of the card
        /// </summary>
        public CardSize Size 
        { 
            get; 
            set; 
        }
        /// <summary>
        /// Rectangle representing the current visible bounds of the card
        /// </summary>
        public Rectangle HitBox
        {
            get { return (new Rectangle((int)Position.X, (int)Position.Y, (int)(Width), (int)(Height))); } 
        }
        /// <summary>
        /// Stores whether or not the card is currently being dragged
        /// </summary>
        public bool Dragging 
        { 
            get; 
            set; 
        }
        public IMovable AttachedNext { get { return _attachedNext; } private set { _attachedNext = value; } }
        public IMovable AttachedPrev { get { return _attachedPrev; } private set { _attachedPrev = value; } }
        #endregion
        #region Card game-specific properties
        /// <summary>
        /// Gets the name of the card. This is NOT the asset name. Multiple cards may share the same CardName, but AssetName will be unique.
        /// </summary>
        public string CardName
        {
            get;
            private set;
        }
        /// <summary>
        /// Amount of HP remaining
        /// </summary>
        public int Health 
        { 
            get; 
            private set; 
        }
        public bool Dead
        {
            get { return (Health <= 0); }
        }
        /// <summary>
        /// Gets the CardType
        /// </summary>
        public CardType TypeOfCard
        {
            get;
            private set;
        }
        /// <summary>
        /// Evolution level of the card (0-2)
        /// </summary>
        public int PokemonStage
        {
            get { return _pokemonStage; }
            set
            {
                if (value > 2)
                    _pokemonStage = 2;
                else if (value < 0)
                    _pokemonStage = 0;
                else
                    _pokemonStage = value;
            }
        }
        /// <summary>
        /// List of Attacks this card has (if any)
        /// </summary>
        public Action[] Attacks { get; private set; }
        /// <summary>
        /// List of all abilities or other special properties this card has (if any)
        /// </summary>
        public Action Ability { get; private set; }
        /// <summary>
        /// Stores where the card is on the game board
        /// </summary
        public CardPlacement Placement { get; set; }
        /// <summary>
        /// List of all CardStatus effects active on this card
        /// </summary>
        public List<CardStatus> StatusConditions { get; private set; }
        #endregion

        public Card(string assetName, string cardName, Vector2 position, int health, float zIndex, CardType type, int pokemonStage, Action[] attacks, 
                    Action ability, PokemonCardGame game)
        {
            /*
             * This constructor assumes the initial card placement is always in a deck.
             * If this turns out not to be the case, remember to change it...
             */
                        
            _assetName = assetName;
            if (cardName == null) // make up a name from the assetName
            {
                StringBuilder s = new StringBuilder();
                foreach (char c in assetName)
                {
                    if ((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z'))
                        s.Append(c);
                }
                CardName = s.ToString();
            }
            else
            {
                CardName = cardName;
            }

            _position = position;
            _prevPosition = _position; // initial state makes them equal so _prevPosition is never null
            Active = false;
            Health = health;

            TypeOfCard = type;
            PokemonStage = pokemonStage;
            AttachedNext = null; // not going to be any cards attached at object construction
            AttachedPrev = null;
            Attacks = attacks;
            Ability = ability;
            StatusConditions = new List<CardStatus>(); // all cards will init clean

            Dragging = false;
            Size = CardSize.Deck;
            Placement = CardPlacement.Deck;
            ZIndex = zIndex;

            _game = game;
        }

        /// <summary>
        /// LoadContent will load a texture from the content pipeline with the given asset name.
        /// </summary>
        /// <param name="content">Content manager</param>
        /// <param name="assetName">String name of the asset to load</param>
        public void LoadContent(ContentManager content)
        {
            try
            {
                //System.Diagnostics.Debug.WriteLine("Loading " + AssetName);
                CardTexture = content.Load<Texture2D>(@"cards\" + AssetName);
                AltTexture = content.Load<Texture2D>(GameConstants.PATH_CARDBACK);
            }
            catch (ContentLoadException exception)
            {
                System.Diagnostics.Debug.WriteLine(exception.Message);
                throw exception;
            }
        }
        
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Active ? CardTexture : AltTexture, Position, null, Color.White, 0f, Vector2.Zero, Size.ToSingle(), SpriteEffects.None, ZIndex);
            // TODO Draw any necessary damage counters on top. Or maybe just draw the actual number value of damage on the card.
            // TODO Handle any hover action before clearing state 

            if (debug)
            {
                spriteBatch.DrawString(_game.screenManager.Focused.DebugFont, this.ZIndex.ToString(), this.Position, Color.Red);
            }

            Hovering = false;
        }

        public void Damage(int amount)
        {
            /* TODO : Implement proper Damage() method
             *  + Calculate which damage counters to display on the card
             *  + Add them to a "DamageCounter" object list maybe.
             *  
             * LATER :
             * - Change parameter to take a CardHelper.Action object, calculate damage and status conditions from that
             * + ^ Not sure if that's a good idea. Should probably have the Action just call this method.
             */

            Health -= amount;                
        }

        public void Click()
        {
            // TODO have the card perform an action. play card/ go to hand etc.
        }

        public void SetSize()
        {
            if ((Placement == CardPlacement.Deck) || (Placement == CardPlacement.Discard))
                Size = CardSize.Deck;
            else if (Placement == CardPlacement.Bench)
                Size = CardSize.Bench;
            else if (Placement == CardPlacement.Hand)
                Size = CardSize.Hand;
            else if (Placement == CardPlacement.Playing)
                Size = CardSize.Active;
            else if (Placement == CardPlacement.Prize)
                Size = CardSize.Prize;
            else
                Size = CardSize.Deck; // this should never actually get used unless I fucked up somewhere
        }

        public bool IsMouseOver(MouseState mouse)
        {
            if (mouse.X > HitBox.Left && mouse.X < HitBox.Right
                && mouse.Y > HitBox.Top && mouse.Y < HitBox.Bottom)
                return true;
            return false;
        }

        public bool IsIntersecting(IMovable other)
        {
            if (this.HitBox.Bottom < other.HitBox.Top) return false;
            if (this.HitBox.Top > other.HitBox.Bottom) return false;
            if (this.HitBox.Left > other.HitBox.Right) return false;
            if (this.HitBox.Right < other.HitBox.Left) return false;
            
            return true;
        }

        public void AttachToFront(IMovable obj)
        {
            if (AttachedNext == null)
            {
                AttachedNext = obj;
                obj.AttachToBack(this);
            }
            else
                AttachedNext.AttachToFront(obj);
        }

        public void AttachToBack(IMovable obj)
        {
            if (AttachedPrev == null)
            {
                AttachedPrev = obj;
                obj.AttachToFront(this);
            }
            else
                AttachedPrev.AttachToBack(obj);
        }

        public IMovable RemoveFromFront(IMovable obj)
        {
            if (AttachedNext == null)
                return null;
            if (AttachedNext.Equals(obj))
            {
                IMovable next = AttachedNext;
                AttachedNext = null;
                return next;
            }
            else
            {
                return AttachedNext.RemoveFromFront(obj);
            }
        }

        public IMovable RemoveFromBack(IMovable obj)
        {
            if (AttachedPrev == null)
                return null;
            if (AttachedPrev.Equals(obj))
            {
                IMovable prev = AttachedPrev;
                AttachedPrev = null;
                return prev;
            }
            else
            {
                return AttachedPrev.RemoveFromBack(obj);
            }
        }

        public override bool Equals(object obj)
        {
            if (this.GetType() != obj.GetType())
                return false;

            Card objCard = (Card)obj;

            if (!((this.AssetName == objCard.AssetName) && (this.Position == objCard.Position) && (this.ZIndex == objCard.ZIndex) && (this.Active == objCard.Active)))
                return false;

            return true;
        }

        public override string ToString()
        {
            return AssetName + " @" + Position.ToString() + " : [" + Health + "HP, " + " Z-Index " + ZIndex + ", " + (Active ? "Face up]" : "Face down]");
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        public override int Compare(IClickable x, IClickable y)
        {
            return (x.ZIndex.CompareTo(y.ZIndex));
        }

        public int CompareNameTo(Card other)
        {
            return this.CardName.CompareTo(other.CardName);
        }

        public int CompareTo(IClickable other)
        {
            return Compare(this, other);
        }

        public void Hover()
        {
            Hovering = true;
        }
        
    }
}

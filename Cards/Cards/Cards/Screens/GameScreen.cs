using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Cards.Structs;
using Cards.IInteractable;
using Cards.UI;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Cards.KeyboardHandling;

namespace Cards.Screens
{
    public abstract class GameScreen
    {
        public const string LEFTBUTTON = "left";
        public const string RIGHTBUTTON = "right";
        public const string MIDDLEBUTTON = "middle";

        public const string TITLE_MAINMENU = "MAINMENU";
        public const string TITLE_GAMEPLAY = "GAMEPLAYSCREEN";
        public const string TITLE_CLIENT = "CLIENTSCREEN";
        public const string TITLE_GAMEOPTIONS = "OPTIONSMENU";
        public const string TITLE_DECKEDITOR = "DECKEDITOR";
        public const string TITLE_LOBBY = "LOBBYSCREEN";
        public const string TITLE_SERVEROPTIONS = "SERVEROPTIONS";
        
        public string ScreenTitle;
        public int ScreenLevel;
        public string BackgroundAsset { get; set; }

        protected GraphicsDeviceManager graphics;
        protected ContentManager content;
        protected KeyboardHandler keyboardHandler;
        protected PokemonCardGame currentGame;
        protected Texture2D backgroundTexture;
        protected SpriteBatch finalSpriteBatch;
        protected SpriteBatch interfaceSpriteBatch;
        protected MouseState[] mouseState = new MouseState[5];
        protected InputEvent currentInputEvent;
        protected List<IClickable> OnScreenElements; // Unsorted list of everything onscreen (above background)
        protected List<IClickable> ZBuffer;          // List of onscreenelements sorted by depth ( 0 = top )
        public SpriteFont DebugFont;

        private Vector2 DragStartPosition;

        #region debug
        private bool debugging = false;
        private System.Diagnostics.Stopwatch w = new System.Diagnostics.Stopwatch();
        #endregion

        public abstract void Initialize();

        public virtual void LoadContent()
        {
            DebugFont = currentGame.Content.Load<SpriteFont>("DebugFont");
        }

        public abstract void UnloadContent();

        public abstract void OnKeyDown(Keys key);

        public abstract void OnKeyUp(Keys key);

        public virtual void HandleInput()
        {
            UpdateMouseState();
            keyboardHandler.Update();

            if (currentInputEvent == InputEvent.None) // Check to make sure we're not in the middle of an action
                Update();
        }

        public virtual void Update()
        {
            ZBuffer = OnScreenElements;
            if (ZBuffer.Count > 0)
                ZBuffer.Sort();

            foreach (IClickable c in ZBuffer)
            {
                if (c.IsMouseOver(mouseState[0]))
                {
                    c.Hover();

                    if (IsButtonPressed(LEFTBUTTON))
                    {
                        if (c is IMovable) Drag((IMovable)c);
                        else currentInputEvent = InputEvent.LeftClicking + c;
                        break;
                    }
                    if (IsButtonPressed(MIDDLEBUTTON))
                    {
                        currentInputEvent = InputEvent.MiddleClicking + c;
                        break;
                    }
                    if (IsButtonPressed(RIGHTBUTTON))
                    {
                        currentInputEvent = InputEvent.RightClicking + c;
                        break;
                    }

                }
                //System.Diagnostics.Debug.WriteLine("Checking all UI elements.");
            }

        }

        public virtual void Draw()
        {
            interfaceSpriteBatch.Begin();
            interfaceSpriteBatch.Draw(backgroundTexture, Vector2.Zero, Color.White);
            interfaceSpriteBatch.End();

            ZBuffer = OnScreenElements;
            ZBuffer.Sort();

            finalSpriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            foreach (IClickable item in ZBuffer)
                item.Draw(finalSpriteBatch);

            #region debug
            if (Structs.GameConstants.DEBUG_MOUSE == true)
            {
                UpdateMouseState();
                finalSpriteBatch.DrawString(DebugFont, mouseState[0].X + "." + mouseState[0].Y,
                    new Vector2(mouseState[0].X + 15, mouseState[0].Y), Color.Red);
            }
            #endregion

            finalSpriteBatch.End();
        }

        public virtual void Close()
        {
            currentInputEvent = InputEvent.None;
        }

        protected virtual void Drag(IMovable obj)
        {
            // check if the dragging just started
            if (currentInputEvent != InputEvent.Dragging)
                DragStartPosition = obj.Position; // Set DragStartPosition

            // register the dragging event with this object
            currentInputEvent = InputEvent.Dragging + obj;
            // Find the bottom IMovable attached
            IMovable cur = obj;
            int i = 0; // how many levels down we go
            while (cur.AttachedPrev != null)
            {
                cur = obj.AttachedPrev;
                i++;
            }

            // Check if we need to drop
            if (mouseState[0].LeftButton == ButtonState.Released)
            {
                currentInputEvent = InputEvent.None;
                Drop(cur); 
                if (obj.Position.Equals(DragStartPosition)) // if the obj didn't move 
                    currentInputEvent = InputEvent.LeftClicking + obj; // set event to clicking. user took a long time to release the mouse                
                return;
            }

            // Start updating positions now that we're at the lowest object
            do
            {
                Vector2 offset = new Vector2(mouseState[1].X - cur.Position.X, mouseState[1].Y - cur.Position.Y);
                cur.Position = new Vector2(mouseState[0].X - offset.X, mouseState[0].Y - offset.Y);
                cur.ZIndex = GameConstants.RENDER_CEILING + (i--);

                cur = cur.AttachedNext;
            } while (cur != null);
        }

        protected virtual void Drop(IMovable obj)
        {
            // Make sure this is the lowest attached IMovable
            if (obj.AttachedPrev != null)
            {
                //Call Drop() again on whatever is below this
                Drop(obj.AttachedPrev);
            }
            else
            {
                obj.ZIndex = GameConstants.RENDER_FLOOR;

                ZBuffer = OnScreenElements; // Re-sort in case changes have been made since the last Update() call. 
                ZBuffer.Sort(); 
                foreach (IClickable c in ZBuffer)
                {
                    if ((c is IMovable) && !c.Equals(obj)) // Only count elements which can be moved, and aren't the object itself
                    {
                        if (obj.IsIntersecting((IMovable)c))
                        {
                            obj.ZIndex = c.ZIndex - GameConstants.RENDER_CARD_DEPTH;
                            ZBuffer.Add(obj); // Add the object back on so that if Drop() is called again before Update() we have the full list
                            break; // stop when we find the first match
                        }
                    }
                }

                if (obj.AttachedNext != null)
                    Drop(obj.AttachedNext);
            }
        }

        /// <summary>
        /// Checks to see if the given mouse state intersects with the Rectangle 'bounds'
        /// </summary>
        /// <param name="mouseState"></param>
        /// <param name="bounds"></param>
        /// <returns></returns>
        protected bool IsMouseIntersecting(MouseState state, Rectangle bounds)
        {
            if (state.X > bounds.Left && state.X < bounds.Right
                && state.Y > bounds.Top && state.Y < bounds.Bottom)
                return true;
            return false;
        }

        protected bool IsButtonPressed(string button)
        {
            switch (button)
            {
                case LEFTBUTTON:
                    return ((mouseState[0].LeftButton == ButtonState.Pressed) && (mouseState[1].LeftButton == ButtonState.Released));
                case RIGHTBUTTON:
                    return ((mouseState[0].RightButton == ButtonState.Pressed) && (mouseState[1].RightButton == ButtonState.Released));
                case MIDDLEBUTTON:
                    return ((mouseState[0].MiddleButton == ButtonState.Pressed) && (mouseState[1].MiddleButton == ButtonState.Released));
            }

            return false;
        }

        protected bool ButtonRelease(string button)
        {
            switch (button)
            {
                case LEFTBUTTON:
                    return ((mouseState[1].LeftButton == ButtonState.Pressed) && (mouseState[0].LeftButton == ButtonState.Released));
                case RIGHTBUTTON:
                    return ((mouseState[1].RightButton == ButtonState.Pressed) && (mouseState[0].RightButton == ButtonState.Released));
                case MIDDLEBUTTON:
                    return ((mouseState[1].MiddleButton == ButtonState.Pressed) && (mouseState[0].MiddleButton == ButtonState.Released));
            }

            return false;
        }

        protected void UpdateMouseState()
        {
            #region debug
            w.Reset();
            w.Start();
            #endregion

            for (int i = (mouseState.Length - 1); i > 0; i--)
            {
                mouseState[i] = mouseState[i - 1];
            }
            mouseState[0] = Mouse.GetState();

            #region debug
            w.Stop();
            if (debugging)
                System.Diagnostics.Debug.WriteLine("Mouse " + w.Elapsed.TotalMilliseconds.ToString());
            #endregion
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Cards.Player;
using Cards.Structs;
using Cards.IInteractable;
using Cards.UI;
using Cards.KeyboardHandling;

namespace Cards.Screens
{
    [Obsolete("Replaced with 'NowPlayingScreen' for multiplayer server compatibility. Use this only for tests.", false)]
    class GameplayScreen : GameScreen
    {
        private const string BACKGROUND_ASSET = "GameMat";  

        #region debug
        private bool debugging = false;
        System.Diagnostics.Stopwatch s = new Stopwatch();
        #endregion

        #region DeclarePlayers
        NPlayer Player1;
        NPlayer Player2;
        #endregion
        #region Declare UI elements
        public Button drawCardButtonP1;
        public TextBoxMulti chatBox;
        #endregion

        public GameplayScreen(PokemonCardGame game)
        {
            currentGame = game;
            content = new ContentManager(currentGame.Services, "Content");

            ScreenTitle = TITLE_GAMEPLAY;
            ScreenLevel = 2;
        }

        public override void Initialize()
        {
            /* TODO: (low) Implement proper initializer for all the cards/decks
             * Init the background
             * Init two decks (Load an XML file with data on each card from folders "Decks\Player1" and "Decks\Player2")
             * Init controls (buttons, etc)
             * Init Labels (damage counters, energy card symbols, card zones, etc)
             * Init starting gameplayStage
             */

            interfaceSpriteBatch = new SpriteBatch(currentGame.GraphicsDevice);
            finalSpriteBatch = new SpriteBatch(currentGame.GraphicsDevice);

            Player1 = new NPlayer(1, currentGame);
            Player2 = new NPlayer(2, currentGame);

            // Build the players' decks
            DeckBuilder builder = new DeckBuilder(GameConstants.PATH_PLAYER1DECK, currentGame);

            builder.ImportDeck();
            Player1.PlayerDeck = builder.ToDeck();
            Player1.PlayerDeck.Position = new Vector2(1489, 607);
            Player1.PlayerDeck.Shuffle();

            builder.DeckPath = GameConstants.PATH_PLAYER2DECK;
            builder.ImportDeck();
            Player2.PlayerDeck = builder.ToDeck();
            Player2.PlayerDeck.Position = new Vector2(30, 178);
            Player2.PlayerDeck.Shuffle();
            
            // Init the players' hands
            Player1.PlayerHand.Position = new Vector2(15, 650);
            Player2.PlayerHand.Position = new Vector2(850, 30);

            // Init buttons
            drawCardButtonP1 = new Button("DrawButton", new Vector2(1355, 632), true, currentGame);
            chatBox = new TextBoxMulti("Enter chat", new Vector2((int)(GameConstants.SCREEN_WIDTH - (GameConstants.SCREEN_WIDTH * 0.005) - 450),
                (int)(GameConstants.SCREEN_HEIGHT / 2) - 61), currentGame);

            // Populate the OnScreenElements list
            OnScreenElements = new List<IClickable>();
            OnScreenElements.Add(drawCardButtonP1);
            OnScreenElements.Add(chatBox);
            foreach (Card c in Player1.PlayerDeck) OnScreenElements.Add(c);
            foreach (Card c in Player2.PlayerDeck) OnScreenElements.Add(c);

            currentInputEvent = InputEvent.None;
            keyboardHandler = new KeyboardHandler(currentGame);
        }

        public override void LoadContent()
        {
            base.LoadContent();

            try
            {
                backgroundTexture = content.Load<Texture2D>(BACKGROUND_ASSET);
                foreach (IClickable element in OnScreenElements) 
                    element.LoadContent(content);
            }
            catch (ContentLoadException e)
            {            
                // TODO: Write a pop-up screen to warn of incorrect files in the deck instead of just ignoring the error/ crashing
                throw e;
            }
        }

        public override void UnloadContent()
        {
            content.Unload();
        }

        public override void OnKeyDown(Keys key)
        {
            if (keyboardHandler.Typing)
            {
                chatBox.EnterText(key);

                if (key.Equals(Keys.Enter) || key.Equals(Keys.Escape))
                {
                    keyboardHandler.Typing = false;
                    chatBox.Active = keyboardHandler.Typing;
                    return;
                }
            }
            else if (key == Keys.Back)
                    currentGame.screenManager.FocusScreen(GameScreen.TITLE_MAINMENU);
        }

        public override void OnKeyUp(Keys key)
        {
            // 
        }

        public override void HandleInput()
        {
            // Updates mouse and keyboard states and calls Update() if there's been a change
            base.HandleInput();

            // Handle dragging
            if (currentInputEvent == InputEvent.Dragging)
                Drag((IMovable)currentInputEvent.EventObject); // Keep dragging
            
            // Handle left clicks
            if (currentInputEvent == InputEvent.LeftClicking)
            {
                if (currentInputEvent.EventObject is Button)
                {
                    Button b = (Button)currentInputEvent.EventObject;
                    if (b.Equals(drawCardButtonP1))
                        DrawCard(Player1.PlayerDeck, Player1.PlayerHand);
                    if (b.Equals(chatBox))
                    {
                        keyboardHandler.Typing = true;
                        chatBox.Active = true;

                    }
                }
                else if (currentInputEvent.EventObject is Card)
                {
                    Card c = (Card)currentInputEvent.EventObject;
                    c.Click();
                }
                currentInputEvent = InputEvent.None;
            }

            // Handle right clicks
            if (currentInputEvent == InputEvent.RightClicking)
            {
                if (currentInputEvent.EventObject is Card)
                    Flip((Card)currentInputEvent.EventObject);

                currentInputEvent = InputEvent.None;
            }

            // Handle middle clicks
            if (currentInputEvent == InputEvent.MiddleClicking)
            {
                if (currentInputEvent.EventObject is Card)
                {
                    Card c = (Card)currentInputEvent.EventObject;
                    if (c.Size == CardSize.Full)
                    {
                        UnZoom(c);
                    }
                    else
                    {
                        Zoom(c);
                        return;
                    }
                }
                currentInputEvent = InputEvent.None;
            }

            // Handle already zooming. We want to ignore any mouse input unless it's unzooming. 
            if (currentInputEvent == InputEvent.Zooming)
            {
                if (IsButtonPressed(MIDDLEBUTTON)) Update();
                else return;
            }       
        }

        /* // OBSOLETE
        public override void HandleInput()
        {
            UpdateMouseState();

            // If there is a persisting InputEvent, skip the normal update and jump back into handling it
            if (currentInputEvent != InputEvent.None)
            {
                if (currentInputEvent == InputEvent.Dragging)
                {
                    Drag((IMovable)currentInputEvent.EventObject);
                }
                else if (currentInputEvent == InputEvent.Zooming)
                {
                    if (IsButtonPressed(MIDDLEBUTTON))
                        UnZoom((Card)currentInputEvent.EventObject);
                    else
                        System.Threading.Thread.Sleep(GameConstants.UPDATE_WAIT_TIME);
                }
                else if (currentInputEvent == InputEvent.Hovering)
                {
                    Hover(currentInputEvent.EventObject);

                    //Update in low-priority
                    System.Threading.Thread.Sleep(GameConstants.UPDATE_WAIT_TIME);
                    this.Update();
                }
                //else if (currentInputEvent == InputEvent.ShiftingCard)




            }
            else
            {
                if (mouseState[0] == mouseState[1])
                    System.Threading.Thread.Sleep(GameConstants.UPDATE_WAIT_TIME);
                else
                    this.Update();
            }
        }

        public override void Update()
        {
            // reset the ZBuffer list... things got pretty messy when I forgot this.
            ZBuffer = new List<IClickable>();
            ZBuffer.Add(drawCardButtonP1);
            ZBuffer.Add(chatBox);
            foreach (IMovable c in Player1.PlayerDeck)
                ZBuffer.Add(c);
            foreach (IMovable c in Player2.PlayerDeck)
                ZBuffer.Add(c);
            foreach (IMovable c in Player1.PlayerDiscard)
                ZBuffer.Add(c);
            foreach (IMovable c in Player2.PlayerDiscard)
                ZBuffer.Add(c);
            foreach (IMovable c in Player1.PlayerHand)
                ZBuffer.Add(c);
            foreach (IMovable c in Player2.PlayerHand)
                ZBuffer.Add(c);
            foreach (IMovable c in Player1.PlayerPrize)
                ZBuffer.Add(c);
            foreach (IMovable c in Player2.PlayerPrize)
                ZBuffer.Add(c);

            if (ZBuffer.Count > 0)
                ZBuffer.Sort();

            foreach (IClickable c in ZBuffer)
            {
                if (c.IsMouseOver(mouseState[0]))
                {
                    currentInputEvent = InputEvent.Hovering + c;

                    // Left button clicked over an IMovable
                    if (IsButtonPressed(LEFTBUTTON) && (c is IMovable))
                    {
                        Drag((IMovable)c);
                        break;
                    }
                    // Middle click a card
                    if (IsButtonPressed(MIDDLEBUTTON) && (c is Card))
                    {
                        Zoom((Card)c);
                        break;
                    }
                    // Right click a card
                    if (IsButtonPressed(RIGHTBUTTON) && (c is Card))
                    {
                        Flip((Card)c);
                        break;
                    }

                    #region Buttons and Interface checks
                    if (c.Equals(drawCardButtonP1) && IsButtonPressed(LEFTBUTTON))
                    {
                        DrawCard(Player1.PlayerDeck, Player1.PlayerHand);
                        break;
                    }
                    #endregion
                }
            }

        }
         * */

        private void Flip(Card card)
        {
            card.Active = !card.Active;
        }

        private void Zoom(Card card)
        {
            if (card.Size != CardSize.Full)
            {
                card.Size = CardSize.Full;

                card.PrevPosition = card.Position;
                card.Position = new Vector2((currentGame.Window.ClientBounds.Width / 2) - (card.HitBox.Width / 2),
                                            (currentGame.Window.ClientBounds.Height / 2) - (card.HitBox.Height / 2));

                card.PrevZIndex = card.ZIndex;
                card.ZIndex = GameConstants.RENDER_ZOOM_LEVEL;
                currentInputEvent = InputEvent.Zooming + card;
            }
        }

        private void UnZoom(Card card)
        {
            if (card.Size == CardSize.Full)
            {
                card.SetSize();
                card.Position = card.PrevPosition;
                card.ZIndex = card.PrevZIndex;
                //Drop(card);
                currentInputEvent = InputEvent.None;
            }
        }

        private void DrawCard(Deck fromDeck, Hand toHand)
        {
            System.Diagnostics.Debug.WriteLine("DrawCard()");
            toHand.AddCard(fromDeck.DrawCard());
        }

        // TODO: Implement remaining card actions
        // DiscardCard(Card card)
        // 
        // AttachCard(Card card1, Card card2) // <-- add sanity checks to make sure they should be attached
        // DettachCard(Card card1, Card card2) 
        // 
    }
}

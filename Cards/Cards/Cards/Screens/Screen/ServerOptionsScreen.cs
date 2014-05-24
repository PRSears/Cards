using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Cards.IInteractable;
using Cards.KeyboardHandling;
using Cards.Screens.Layouts;
using Cards.Structs;
using Cards.UI;

namespace Cards.Screens
{
    class ServerOptionsScreen : GameScreen
    {
        #region Declare UI elements
        private Button[] TestButtons;
        private FluidLayoutContainer LayoutManager;
        #endregion
        
        public ServerOptionsScreen(PokemonCardGame game)
        {
            currentGame = game;
            content = new ContentManager(currentGame.Services, "Content");
            BackgroundAsset = "MainMenu";
            ScreenTitle = TITLE_SERVEROPTIONS;
            ScreenLevel = 0;

            LayoutManager = new FluidLayoutContainer(new Rectangle(0, 0, GameConstants.SCREEN_WIDTH, GameConstants.SCREEN_HEIGHT),
                                                     Edge.Top, 25);
            TestButtons = new Button[6];
        }

        public override void Initialize()
        {
            interfaceSpriteBatch = new SpriteBatch(currentGame.GraphicsDevice);
            finalSpriteBatch = new SpriteBatch(currentGame.GraphicsDevice);
            OnScreenElements = new List<IClickable>();

            //HACK Generate a bunch of dummy buttons for testing purposes
            for (int i = 0; i < TestButtons.Count(); i++)
            {
                TestButtons[i] = new Button(currentGame, position: new Rectangle(0, 0, 250, 100), colorOverlay: Color.DarkGreen, label: "start server", labelColor: Color.White);
                OnScreenElements.Add(TestButtons[i]);
            }

            // add UI to layout manager
            foreach (Button b in TestButtons)
                LayoutManager.AddElement(b);

            currentInputEvent = InputEvent.None;
            keyboardHandler = new KeyboardHandler(currentGame);
        }

        public override void LoadContent()
        {
            base.LoadContent();

            try
            {
                backgroundTexture = content.Load<Texture2D>(BackgroundAsset);
                foreach (IClickable element in OnScreenElements)
                    element.LoadContent(content);
            }
            catch (ContentLoadException e)
            {
                // TODO pop up warning of missing texture
                throw;
            }
        }

        public override void HandleInput()
        {
            base.HandleInput();

            currentInputEvent = InputEvent.None;
        }

        public override void UnloadContent()
        {
            content.Unload();
        }

        public override void OnKeyDown(Microsoft.Xna.Framework.Input.Keys key)
        {
            // HACK Write keydowns to debug box for debug purposes. Change key handling for proper release.
            System.Diagnostics.Debug.WriteLine(key.ToString());


            if (key.Equals(Keys.Back) && !keyboardHandler.Typing)
                currentGame.screenManager.FocusScreen(GameScreen.TITLE_MAINMENU);
        }

        public override void OnKeyUp(Microsoft.Xna.Framework.Input.Keys key)
        {
            // Do nothing for now.
        }
    }
}

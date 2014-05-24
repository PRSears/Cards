using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Cards.IInteractable;
using Cards.KeyboardHandling;
using Cards.Structs;
using Cards.UI;
using Cards.Screens.Layouts;

namespace Cards.Screens
{
    class MainMenuScreen : GameScreen
    {        
        #region Declare ui elements
        private Button StartServerButton;
        private Button JoinGameButton;
        private Button EditDecksButton;
        private Button OptionsButton;
        private FluidLayoutContainer LayoutManager;
        #endregion

        public MainMenuScreen(PokemonCardGame game)
        {
            currentGame = game;
            content = new ContentManager(currentGame.Services, "Content");
            BackgroundAsset = "MainMenu";
            ScreenTitle = TITLE_MAINMENU;
            ScreenLevel = 0;
            //System.Diagnostics.Debug.WriteLine(currentGame.Window.ClientBounds.Width + ", " + currentGame.Window.ClientBounds.Height);
            LayoutManager = new FluidLayoutContainer(new Rectangle(0, 0, GameConstants.SCREEN_WIDTH, GameConstants.SCREEN_HEIGHT),
                Edge.Left, 25);
        }

        public override void Initialize()
        {
            interfaceSpriteBatch = new SpriteBatch(currentGame.GraphicsDevice);
            finalSpriteBatch = new SpriteBatch(currentGame.GraphicsDevice);

            StartServerButton = new Button(currentGame, asset: "StartServerButton");
            JoinGameButton = new Button(currentGame, asset: "JoinGameButton");
            EditDecksButton = new Button(currentGame, asset: "EditDecksButton");
            OptionsButton = new Button(currentGame, asset: "OptionsButton");

            // Add buttons to layout manager
            LayoutManager.AddElement(StartServerButton);
            LayoutManager.AddElement(JoinGameButton);
            LayoutManager.AddElement(EditDecksButton);
            LayoutManager.AddElement(OptionsButton);
            
            OnScreenElements = new List<IClickable>();
            // Add elements from all layout containers to OnScreenElements
            foreach (IClickable element in LayoutManager.Elements)
                OnScreenElements.Add(element);

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
                throw e;
            }
        }

        public override void UnloadContent()
        {
            content.Unload();
        }

        public override void OnKeyDown(Keys key)
        {
            System.Diagnostics.Debug.WriteLine(key.ToString());
        }

        public override void OnKeyUp(Keys key)
        {
            //
        }

        public override void HandleInput()
        {
            // Updates mouse and keyboard states and calls Update() if there's been a change
            base.HandleInput();

            // Handle left clicks
            if (currentInputEvent == InputEvent.LeftClicking)
            {
                if (currentInputEvent.EventObject is Button)
                {
                    Button b = (Button)currentInputEvent.EventObject;
                    if (b.Equals(StartServerButton))
                        currentGame.screenManager.Open(new ServerOptionsScreen(currentGame));

                    if (b.Equals(JoinGameButton) && !currentGame.screenManager.InScreenStack(GameScreen.TITLE_GAMEPLAY))
                        currentGame.screenManager.Open(new ServerLobbyScreen(currentGame));
                    else
                        currentGame.screenManager.FocusScreen(GameScreen.TITLE_GAMEPLAY);

                    if (b.Equals(EditDecksButton))
                        currentGame.screenManager.Open(new DeckEditorScreen(currentGame));

                    if (b.Equals(OptionsButton))
                        currentGame.screenManager.Open(new GameOptionsScreen(currentGame));

                }
                currentInputEvent = InputEvent.None;
            }

            // Handle middle clicks
            if (currentInputEvent == InputEvent.MiddleClicking)
            {
                currentInputEvent = InputEvent.None; // There's nothing middle clickable in this menu, so ignore it.
            }

            // Handle right clicks
            if (currentInputEvent == InputEvent.RightClicking)
            {
                currentInputEvent = InputEvent.None; // There's nothing right clickable in this menu, so ignore it.
            }
        }

        private void TestStartServer()
        {
            /*
            NetMouseState cm = new NetMouseState(Mouse.GetState());
            NetWrapper testPackage1 = new NetWrapper(Categories.NetMouseState, Originators.Player1, cm);
            System.Diagnostics.Debug.WriteLine(testPackage1.DebugText());

            System.Diagnostics.Debug.WriteLine(NetWrapper.DebugText(testPackage1.ToBytes()));
             */
        }
    }
}

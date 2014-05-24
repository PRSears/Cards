using System;
using System.Collections.Generic;
using System.Net;
using Cards.IInteractable;
using Cards.KeyboardHandling;
using Cards.Net;
using Cards.Player;
using Cards.Structs;
using Cards.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Cards.Screens
{
    class ServerLobbyScreen : GameScreen
    {

        #region UI element declarations
        private Button JoinButton;
        private Button SearchButton;
        private TextBoxSingle SearchBar;
        private TextBoxFloating SearchInfobox;
        private Button[] DeckSelectButtons;
        #endregion

        public ClientIntermediary client;
        private IPAddress ServerIP;

        public ServerLobbyScreen(PokemonCardGame game)
        {
            currentGame = game;
            content = new ContentManager(currentGame.Services, "Content");
            BackgroundAsset = "LobbyMenu";
            ScreenTitle = TITLE_LOBBY;
            ScreenLevel = 1;
        }

        /// <summary>
        /// Constructs a new server lobby screen object assuming a connection to 
        /// the server has been made.
        /// </summary>
        /// <param name="game">The current game instance.</param>
        /// <param name="serverAddress">IP address of the connected server.</param>
        public ServerLobbyScreen(PokemonCardGame game, IPAddress serverAddress)
        {
            currentGame = game;
            ServerIP = serverAddress;
            content = new ContentManager(currentGame.Services, "Content");
            ScreenTitle = TITLE_LOBBY;
            ScreenLevel = 1; // NOTE I don't think screenlevel is being used anywhere ... look into that

            SearchBar.Disabled = true;
            SearchButton.Disabled = true;

            if (SearchForServer())
                JoinButton.Disabled = false;
        }

        public override void Initialize()
        {
            client = new ClientIntermediary(currentGame);
            if (ServerIP == null) // if ServerIP wasn't set in the constructor
                ServerIP = new IPAddress(new byte[]{127,0,0,1}); // init to a default of localhost

            #region Init SpriteBatches
            interfaceSpriteBatch = new SpriteBatch(currentGame.GraphicsDevice);
            finalSpriteBatch = new SpriteBatch(currentGame.GraphicsDevice);
            #endregion

            #region Init UI elements
            JoinButton = new Button("JoinButton", new Vector2(85, 745), true, currentGame);
            JoinButton.Disabled = true;

            SearchButton = new Button("SearchButton", new Vector2(565, 405), true, currentGame);
            SearchBar = new TextBoxSingle("Enter server IP", new Vector2(84, 405),  currentGame);
            SearchInfobox = new TextBoxFloating(new string[]{""}, 
                new Vector2(119, 465), "UIFont", new Color(248,242,144), currentGame);
            
            DeckSelectButtons = new Button[4];
            for (int i = 0; i < 4; i++)
                DeckSelectButtons[i] = new Button("DeckSelectButton", new Vector2(85 + (i * 280), 45), true, currentGame);
            DeckSelectButtons[0].Active = true; // default choice
            #endregion

            // Build the list of all elements being drawn to the screen
            OnScreenElements = new List<IClickable>();

            OnScreenElements.Add(JoinButton);
            OnScreenElements.Add(SearchButton);
            OnScreenElements.Add(SearchBar);
            OnScreenElements.Add(SearchInfobox);
            foreach (Button b in DeckSelectButtons) OnScreenElements.Add(b);


            currentInputEvent = InputEvent.None;
            keyboardHandler = new KeyboardHandler(currentGame);
        }

        public override void LoadContent()
        {
            base.LoadContent(); 

            try
            {
                backgroundTexture = content.Load<Texture2D>(this.BackgroundAsset);
                foreach (IClickable element in OnScreenElements)
                    element.LoadContent(content);
            }
            catch (ContentLoadException e)
            {
                // TODO pop up warning of missing texture
                System.Diagnostics.Debug.WriteLine("Missing texture. ");
                System.Diagnostics.Debug.WriteLine(e.Message);
                throw e;
            }
        }

        public override void UnloadContent()
        {
            content.Unload();
        }

        public override void Close()
        {
            base.Close();
            client.Listening = false;
        }

        public override void OnKeyDown(Keys key)
        {
            System.Diagnostics.Debug.WriteLine("OnKeyDown[" + key.ToString() + "]");

            if (key.Equals(Keys.Enter) || key.Equals(Keys.Escape))
            {
                keyboardHandler.Typing = false;
                SearchBar.Active = keyboardHandler.Typing;
                return;
            }
            if (key.Equals(Keys.Back) && !keyboardHandler.Typing)
                currentGame.screenManager.FocusScreen(GameScreen.TITLE_MAINMENU);

            if (keyboardHandler.Typing) SearchBar.EnterText(key);
        }

        public override void OnKeyUp(Keys key)
        {
            //
        }

        public override void HandleInput()
        {
            base.HandleInput();

            // Handle left clicks
            if (currentInputEvent == InputEvent.LeftClicking)
            {
                if (currentInputEvent.EventObject is Button)
                {
                    Button b = (Button)currentInputEvent.EventObject;

                    if (b.Equals(JoinButton) && (!JoinButton.Disabled))
                        JoinGame();

                    if (b.Equals(SearchButton))
                        SearchForServer();

                    foreach (Button selectButton in DeckSelectButtons) if (b.Equals(selectButton))
                        SelectDeck(selectButton);

                    if (b.Equals(SearchBar))
                        ListenSearchBar();
                }
            }
            // Handle right clicks
            if (currentInputEvent == InputEvent.RightClicking)
            {

            }
            // Handle middle clicks
            if (currentInputEvent == InputEvent.MiddleClicking)
            {

            }

            // reset the input event
            currentInputEvent = InputEvent.None;
        }

        /// <summary>
        /// Sets players type and connects to a remote server.
        /// </summary>
        /// <returns>True if successful.</returns>
        private void JoinGame()
        {
            // If you're joining an already-created game session you'll always be the second player
            currentGame.PlayerType = NPlayer.TYPE_PLAYER2;

            this.Close();
            currentGame.screenManager.Open(new NowPlayingScreen(currentGame));
        }

        /// <summary>
        /// Checks to see if there is a CardGame server running at the address currently in the search bar.
        /// </summary>
        /// <returns>True if a valid server is found.</returns>
        private bool SearchForServer()
        {
            //JoinButton.Disabled = false;
            /*

            client.ConnectToServer(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8088));
            System.Diagnostics.Debug.WriteLine("Connected to server. Listening.");
            return false;
             */

            // TODO use ip provided by the constructor if searchbar doesn't pan out

            IPAddress CheckIP;

            SearchInfobox.Text.Clear(); // Clear the info box
            
            /*
            if (!IPAddress.TryParse(SearchBar.Text, out CheckIP))
            {
                SearchInfobox.Text.Add("Enter a valid IP address");
                JoinButton.Disabled = true;
                return false;
            }
             */

            if (!SearchBar.Disabled) // if the SearchBar is available, its contents override ServerIP
            {
                if (IPAddress.TryParse(SearchBar.Text, out CheckIP)) 
                    ServerIP = CheckIP;
            }

            System.Diagnostics.Debug.WriteLine("Trying to connect to server.");
            SearchInfobox.Text.Add("Searching for server at " + ServerIP.ToString());

            try
            {
                client.ConnectToServer(new IPEndPoint(ServerIP, Cards.Structs.GameConstants.SERVER_PORT));
            }
            catch (ArgumentException e)
            {
                SearchInfobox.Text.Add(e.Message);
            }

            if(client.Connected)
            {
                SearchInfobox.Text.Add("Found a server!");
                JoinButton.Disabled = false;
                return true;
            }

            SearchInfobox.Text.Add("No server was found.");
            JoinButton.Disabled = true;
            return false;
        }

        private void SelectDeck(Button deckButton)
        {
            foreach (Button d in DeckSelectButtons)
                d.Active = false;

            deckButton.Active = true;
        }

        private void ListenSearchBar()
        {
            /*
            NetWrapper packageFromServer;
            if (client.RecievedActions.Count > 0)
            {
                packageFromServer = new NetWrapper(client.RecievedActions.Dequeue());
                System.Diagnostics.Debug.WriteLine("Received data: ");
                System.Diagnostics.Debug.WriteLine(packageFromServer.DebugText());
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("There was nothing in the RecievedActions queue.");
            }
             */

            keyboardHandler.Typing = true;
            SearchBar.Active = keyboardHandler.Typing;
        }
    }
}

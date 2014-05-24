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

using Cards.Structs;
using Cards.IInteractable;
using Cards.UI;
using Cards.KeyboardHandling;
using System.Net;
using System.Net.Sockets;
using Cards.Net;
using Networking;
using Networking.Structs;
using Cards.Screens.Layouts;

namespace Cards.Screens
{
    class NowPlayingScreen : GameScreen
    {
        private ClientIntermediary intermediary;
        private IPEndPoint serverEndpoint;
        
        #region Declare UI elements
        private Button TestButton;
        private FluidLayoutContainer LayoutManager;
        #endregion

        public NowPlayingScreen(PokemonCardGame game)
        {
            currentGame = game;
            content = new ContentManager(currentGame.Services, "Content");
            intermediary = new ClientIntermediary(game);
            // HACK endpoint should be loaded in ini/xml
            serverEndpoint = new IPEndPoint(new IPAddress(GameConstants.SERVER_IP), GameConstants.SERVER_PORT);

            BackgroundAsset = "GameMat";
            ScreenTitle = TITLE_GAMEPLAY;
            ScreenLevel = 2;

            LayoutManager = new FluidLayoutContainer(new Rectangle(0, 0, GameConstants.SCREEN_WIDTH, GameConstants.SCREEN_HEIGHT),
                                                     Edge.Top, 50);
            TestButton = new Button(currentGame, position: new Rectangle(0, 0, 150, 75), colorOverlay: Color.Black, label: "Action", labelColor: Color.White);
        }

        public override void Initialize()
        {
            interfaceSpriteBatch = new SpriteBatch(currentGame.GraphicsDevice);
            finalSpriteBatch = new SpriteBatch(currentGame.GraphicsDevice);
            OnScreenElements = new List<IClickable>();

            OnScreenElements.Add(TestButton);
            LayoutManager.AddElement(TestButton);
            
            currentInputEvent = InputEvent.None;
            keyboardHandler = new KeyboardHandler(currentGame);

            intermediary.ConnectToServer(serverEndpoint);
        }

        public override void LoadContent()
        {
            base.LoadContent();

            backgroundTexture = content.Load<Texture2D>(this.BackgroundAsset);
            foreach (IClickable element in OnScreenElements)
                element.LoadContent(content);
        }

        public override void Update()
        {
            base.Update();
        }

        public override void HandleInput()
        {
            base.HandleInput();

            // Handle left clicks
            if (currentInputEvent == InputEvent.LeftClicking)
            {
                NetMouseState netMouse = new NetMouseState(mouseState[0]);
                intermediary.EnqueueSend(netMouse);
                                
                System.Diagnostics.Debug.WriteLine("Enqueing click.");
                intermediary.Update();
            }
            // Handle right clicks
            if (currentInputEvent == InputEvent.RightClicking)
            {

            }
            // Handle middle clicks
            if (currentInputEvent == InputEvent.MiddleClicking)
            {

            }

            currentInputEvent = InputEvent.None;
        }

        public override void Close()
        {
            base.Close();
            
        }

        public override void UnloadContent()
        {
            content.Unload();
        }

        public override void OnKeyDown(Microsoft.Xna.Framework.Input.Keys key)
        {
            // HACK Write keydowns to debug box for debug purposes. Change key handling for proper release.
            System.Diagnostics.Debug.WriteLine(key.ToString());
        }

        public override void OnKeyUp(Microsoft.Xna.Framework.Input.Keys key)
        {
            // Do nothing for now.
        }
    }
}

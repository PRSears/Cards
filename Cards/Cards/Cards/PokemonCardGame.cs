using System;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using Cards.Player;
using Cards.Screens;
using Cards.Structs;
using Cards.Net;

namespace Cards
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class PokemonCardGame : Microsoft.Xna.Framework.Game
    {
        protected GraphicsDeviceManager graphics;
        public ScreenManager screenManager { get; protected set; }

        public int PlayerType { get; set; }      

        /// <summary>
        /// Debug
        /// </summary>
        private bool debugging = false;
        private System.Diagnostics.Stopwatch s = new Stopwatch();
        
        public PokemonCardGame()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = GameConstants.SCREEN_WIDTH;
            graphics.PreferredBackBufferHeight = GameConstants.SCREEN_HEIGHT;
            this.TargetElapsedTime = TimeSpan.FromSeconds(1.0f / Cards.Structs.GameConstants.TARGET_FRAMERATE);
            Content.RootDirectory = "Content";
            this.IsMouseVisible = true;
            this.PlayerType = NPlayer.TYPE_OBSERVER; // init to observer

            screenManager = new ScreenManager(this);

            this.Exiting += new EventHandler<EventArgs>(PokemonCardGame_Exiting);
        }


        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            screenManager.Initialize();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            screenManager.LoadContent();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            screenManager.UnloadContent();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            s.Reset();
            s.Start();

            screenManager.HandleInput();
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.SteelBlue);

            screenManager.Draw();

            s.Stop();
            if (debugging)
                System.Diagnostics.Debug.WriteLine(s.Elapsed.TotalMilliseconds.ToString());

            base.Draw(gameTime);
        }

        protected void PokemonCardGame_Exiting(object sender, EventArgs args)
        {
            // TODO Make sure everything actually quits.
            foreach (GameScreen screen in screenManager.Screens)
                screen.Close();
        }
    }
}

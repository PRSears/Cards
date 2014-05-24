using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cards.Screens
{
    public class ScreenManager
    {

        public GameScreen Focused { 
            get
            {
                return Screens.Peek();
            }
        }
        public Stack<GameScreen> Screens { get; private set; }
        protected PokemonCardGame currentGame;
        private bool justSwitched;
      
        public ScreenManager(PokemonCardGame game)
        {
            currentGame = game;
            justSwitched = false;

            Screens = new Stack<GameScreen>();

            Screens.Push(new MainMenuScreen(currentGame));
        }

        protected void Push(GameScreen screen)
        {
            if (InScreenStack(screen)) throw new ArgumentException("screen already exists in the stack.");

            Screens.Push(screen);
        }

        /// <summary>
        /// Adds the new screen to the top of the stack. Properly initializes, and loads content
        /// of the new Screen. If the same type of screen already exists in the stack then it gets focused.
        /// </summary>
        /// <param name="newScreen">The new GameScreen object to open.</param>
        public void Open(GameScreen newScreen)
        {
            if (!InScreenStack(newScreen))
            {
                this.Push(newScreen);

                Screens.Peek().Initialize();
                Screens.Peek().LoadContent();
            }
            else
                FocusScreen(newScreen.ScreenTitle);
            // TODO add better/more sanity checks
        }

        public void Initialize()
        {
            foreach (GameScreen g in Screens)
                g.Initialize();
        }

        public void LoadContent()
        {
            foreach (GameScreen g in Screens)
                g.LoadContent();
        }

        public void UnloadContent()
        {
            foreach (GameScreen g in Screens)
                g.UnloadContent();
        }

        // TODO move keyboard handling into screen manager so basic functions aren't duplicated

        public void HandleInput()
        {
            //if (key == Keys.Back)
            //    currentGame.screenManager.FocusScreen(GameScreen.TITLE_MAINMENU);
            if (currentGame.IsActive)
                Focused.HandleInput();
        }

        public void Update()
        {
            //SortScreens();
            Focused.Update();
        }

        public void Draw()
        {
            Focused.Draw();
        }

        public void FocusScreen(string title)
        {

            System.Diagnostics.Debug.WriteLine("Trying to focus " + title);

            Stack<GameScreen> pulled = new Stack<GameScreen>();
            GameScreen selected = null;

            // pop everything off the screen stack to search through it
            while (Screens.Count > 0)
            {
                if (Screens.Peek().ScreenTitle.Equals(title))
                    selected = Screens.Pop();
                else
                    pulled.Push(Screens.Pop());
            }

            // Put all the screens back on
            while (pulled.Count > 0)
                Screens.Push(pulled.Pop());
            // If we found a match up above add it to the top of the stack
            if (selected != null) Screens.Push(selected);
        }

        public bool InScreenStack(GameScreen screen)
        {
            foreach (GameScreen s in Screens)
                if (screen.ScreenTitle == s.ScreenTitle) return true;
            return false;
        }

        public bool InScreenStack(string screenTitle)
        {
            foreach (GameScreen s in Screens)
                if (screenTitle == s.ScreenTitle) return true;
            return false;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Cards.KeyboardHandling
{
    public class KeyboardHandler
    {
        private PokemonCardGame currentGame;
        private Keys[] lastKeysPressed;
        public bool Typing { get; set; }

        public KeyboardHandler(PokemonCardGame game)
        {
            lastKeysPressed = new Keys[0];
            Typing = false;
            currentGame = game;
        }

        public void Update()
        {
            KeyboardState kbState = Keyboard.GetState();
            Keys[] keysPressed = kbState.GetPressedKeys(); // array of all currently pressed keys            

            // Check keysup
            foreach (Keys key in lastKeysPressed)
            {
                if (!keysPressed.Contains(key))
                    currentGame.screenManager.Focused.OnKeyUp(key); // send key release to top screen
            }

            // Check for new keypressed
            foreach (Keys key in keysPressed)
            {
                if (!lastKeysPressed.Contains(key))
                    currentGame.screenManager.Focused.OnKeyDown(key); // send key press to top screen
            }

            lastKeysPressed = keysPressed;
        }
    }
}

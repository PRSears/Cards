using System;

namespace Cards
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            
            using (PokemonCardGame game = new PokemonCardGame())
            {
                game.Run();
            }            
        }
    }
#endif
}


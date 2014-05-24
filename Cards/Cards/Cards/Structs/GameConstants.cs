using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cards.Structs
{
    // HACK these settings should be loaded from an ini / xml file or something
    public static class GameConstants
    {
        #region ZIndex constants
        /// <summary>
        /// Top ZIndex to allow cards to draw at. Cards should be dragged at this level.
        /// </summary>
        public const float RENDER_CEILING = 0.006f;
        /// <summary>
        /// ZIndex zoomed-in cards will use (very top allowed).
        /// </summary>
        public const float RENDER_ZOOM_LEVEL = 0.002F;
        /// <summary>
        /// ZIndex for interface elements to draw on.
        /// </summary>
        public const float RENDER_INTERFACE_LEVEL = 0.003F;
        /// <summary>
        /// Bottom ZIndex. Cards will start here.
        /// </summary>
        public const float RENDER_FLOOR = 0.998f;
        /// <summary>
        /// "Thickness" of one card.
        /// </summary>
        public const float RENDER_CARD_DEPTH = 0.001f;
        #endregion 

        public const int SCREEN_WIDTH = 1600;
        public const int SCREEN_HEIGHT = 900;
        public const float TARGET_FRAMERATE = 30.0f;

        public const string PATH_PLAYER1DECK = @"decks\player1\";
        public const string PATH_PLAYER2DECK = @"decks\player2\";
        public const string PATH_CARDBACK = @"cards\000-cardback";

        #region Server related constants
        public static byte[] SERVER_IP = {127, 0, 0, 1};
        public const int SERVER_PORT = 8088;
        public const int CLIENT1_PORT = 8089;
        public const int CLIENT2_PORT = 8090;
        public const int MIN_PLAYERS = 1; 
        public const int MAX_PLAYERS = 2;
        #endregion 

        #region Debug
        public const bool DEBUG_MOUSE = true;
        #endregion
    }
}

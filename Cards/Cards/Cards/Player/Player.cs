using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cards.Structs;
using Microsoft.Xna.Framework;

namespace Cards.Player
{
    public class NPlayer
    {
        public static string PLAYER_STAGE_DRAW = "DRAW";
        public static string PLAYER_STAGE_TURN = "TURN";
        public static string PLAYER_STAGE_ATTACKING = "ATTACKING";
        public static string PLAYER_STAGE_WAITING = "WAITING";

        public static int TYPE_SERVER = 0;
        public static int TYPE_PLAYER1 = 1;
        public static int TYPE_PLAYER2 = 2;
        public static int TYPE_OBSERVER = 3;

        private PokemonCardGame _game;

        private byte _playerNumber;
        public byte PlayerNumber { get { return _playerNumber; } private set { _playerNumber = value; } }

        private string _playerStage;
        public string PlayerStage
        {
            get
            {
                return _playerStage;
            }
            private set
            {
                if (!((value.Equals(PLAYER_STAGE_ATTACKING)) || (value.Equals(PLAYER_STAGE_DRAW)) || (value.Equals(PLAYER_STAGE_TURN)) || (value.Equals(PLAYER_STAGE_WAITING))))
                    throw new ArgumentException("Invalid value for PlayerStage.");
                else
                    _playerStage = value;
            }
        }

        public Deck PlayerDeck { get; set; }
        public Deck PlayerDiscard { get; set; }
        public Deck PlayerPrize { get; set; }
        public Hand PlayerHand { get; set; }

        public NPlayer(byte playerNumber, PokemonCardGame game)
        {
            _game = game;

            PlayerNumber = playerNumber;
            
            PlayerDeck = new Deck(_game);
            PlayerDiscard = new Deck(_game);
            PlayerPrize = new Deck(_game);
            PlayerHand = new Hand(_game);
        }

        /// <summary>
        /// Advances the player's stage to the next stage of the turn.
        /// </summary>
        public void AdvStage()
        {
            if (PlayerStage.Equals(PLAYER_STAGE_DRAW))
            {
                PlayerStage = PLAYER_STAGE_TURN;
                return;
            }

            if (PlayerStage.Equals(PLAYER_STAGE_TURN))
            {
                PlayerStage = PLAYER_STAGE_ATTACKING;
                return;
            }

            if (PlayerStage.Equals(PLAYER_STAGE_ATTACKING))
            {
                PlayerStage = PLAYER_STAGE_WAITING;
                return;
            }

            if (PlayerStage.Equals(PLAYER_STAGE_WAITING))
            {
                PlayerStage = PLAYER_STAGE_DRAW;
                return;
            }
        }

        #region Overrides
        public override bool Equals(object obj)
        {
            if (!(this.GetType().Equals(obj.GetType())))
                return false;

            NPlayer other = (NPlayer)obj;

            if (!((this.PlayerNumber == other.PlayerNumber) && (this.PlayerStage.Equals(other.PlayerStage))))
                return false;

            return true;
        }

        public override string ToString()
        {
            return string.Format("Player{0}: {1}. {2} Cards in Deck.", PlayerNumber, PlayerStage, PlayerDeck.Count());
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }
        #endregion
    }
}

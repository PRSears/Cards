using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using PokemonXML;
using Cards.Structs;

namespace Cards.Player
{
    class DeckBuilder
    {
        private PokemonCardGame _game;
        private ContentManager content;
                
        private List<XMLCard> XMLCards;

        public string DeckPath { get; set; }

        public DeckBuilder(string path, PokemonCardGame game)
        {
            _game = game;
            content = new ContentManager(_game.Services, "Content");

            XMLCards = new List<XMLCard>();
            DeckPath = path;
        }

        public XMLCard[] ImportDeck()
        {
            XMLCards = new List<XMLCard>();

            try
            {
                foreach (string filename in Directory.GetFiles(content.RootDirectory + @"\" + DeckPath))
                {
                    string assetName = Path.GetFileNameWithoutExtension(filename);
                    if (Path.GetExtension(filename).Equals(".xnb"))
                        XMLCards.Add(content.Load<XMLCard>(DeckPath + assetName));
                }
            }
            catch (ContentLoadException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                throw ex;
            }

            XMLCard[] XMLCardArray = XMLCards.ToArray();
            return XMLCardArray;
        }

        public Deck ToDeck()
        {
            Deck outDeck = new Deck(_game);

            foreach (XMLCard xCard in XMLCards)
            {
                outDeck.AddCard(xCard.AssetName, xCard.CardTitle, xCard.Health, CardType.FromString(xCard.CardKind, xCard.CardColor), 
                                xCard.Stage, Action.FromID(xCard.AttackIDs), Action.FromID(xCard.AbilityID));

            }

            return outDeck;
        }
    }
}

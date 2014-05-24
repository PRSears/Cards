using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Cards.Structs;

namespace Cards.Player
{
    public class Deck : IEnumerable<Card>
    {        
        private PokemonCardGame _game;
        private float _highestZIndex;
        private Stack<Card> deck;

        private Vector2 _position;
        public Vector2 Position 
        {
            get 
            { 
                return _position; 
            }
            set
            {
                _position = value;
                foreach (Card card in deck)
                {
                    card.Position = this.Position;
                }
            }
        }
        
        public Deck(PokemonCardGame game)
        {
            _game = game;
            deck = new Stack<Card>();
            _highestZIndex = GameConstants.RENDER_FLOOR;
            Position = new Vector2(10, 10);
        }

        public Deck(Stack<Card> cards, PokemonCardGame game)
        {
            _game = game;
            deck = cards;
            _highestZIndex = GameConstants.RENDER_FLOOR;
            Position = new Vector2(10, 10);
        }
        
        public void AddCard(string assetName, string cardName, int health, CardType type, int stage, 
                            Action[] attacks, Action ability)
        {
            Card newCard = new Card(assetName, cardName, this.Position, health, _highestZIndex, type, stage, attacks, ability, _game);
            deck.Push(newCard);

            if (_highestZIndex > GameConstants.RENDER_CEILING)
                _highestZIndex -= GameConstants.RENDER_CARD_DEPTH;
        }

        /// <summary>
        /// Adds the card object to the deck. NOTE: this should only be used for the discard piles
        /// </summary>
        /// <param name="card">Card to move into the deck</param>
        public void AddCard(Card card)
        {
            card.ZIndex = _highestZIndex;
            card.Position = this.Position;
            deck.Push(card);

            // update the new highest card position
            if (_highestZIndex > GameConstants.RENDER_CEILING)
                _highestZIndex -= GameConstants.RENDER_CARD_DEPTH;
        }

        /*
        [Obsolete("AddCard( ... 2 parameters) is deprecated, use AddCard(...8 parameters) instead")]
        public void AddCard(string assetName, Vector2 position)
        {
            Card newCard = new Card(assetName, null, position, 10, _highestZIndex, CardType.Colorless, 0, new Action[2], new Action(), _game);
            //newCard.Flip();
            deck.Push(newCard);

            if (_highestZIndex > GameConstants.RENDER_CEILING)
                _highestZIndex -= GameConstants.RENDER_CARD_DEPTH;
        }
         */

        public Card DrawCard()
        {
            if (deck.Count <= 0)
                return null;

            if (_highestZIndex < GameConstants.RENDER_FLOOR)
                _highestZIndex += GameConstants.RENDER_CARD_DEPTH;
            return deck.Pop();
        }

        /// <summary>
        /// Finds the first card with a matching name and pulls it from the deck
        /// </summary>
        /// <param name="card">Card to pull from the deck</param>
        /// <returns>Returns the wanted card object, or null if no match was found.</returns>
        public Card PullCard(Card card)
        {
            return PullCard(card.CardName);
        }

        /// <summary>
        /// Finds the first card with a matching name and pulls it from the deck
        /// </summary>
        /// <param name="cardName">Card name to search for</param>
        /// <returns>Returns the wanted card object, or null if no match was found.</returns>
        public Card PullCard(string cardName)
        {
            Stack<Card> altDeck = new Stack<Card>();
            for (int i = this.deck.Count - 1; i > 0; i--)
            {
                Card cardFromDeck = this.deck.Pop();
                if (cardFromDeck.CardName.Equals(cardName))
                {
                    // dump alt deck back into the main deck before we add cardFromDeck onto the alt stack
                    // shuffle deck
                    // return the popped card
                    foreach (Card cardFromAlt in altDeck)
                        this.deck.Push(cardFromAlt);
                    this.Shuffle();
                    return cardFromDeck;
                }

                altDeck.Push(cardFromDeck);
            }

            // put the cards from altDeck back into the main deck
            foreach (Card cardFromAlt in altDeck)
                deck.Push(cardFromAlt);
            this.Shuffle();

            return null; // did not find a matching card in the deck
        }
        
        public void Shuffle()
        {
            deck = Shuffle(deck);
            _highestZIndex = deck.Peek().ZIndex;
        }

        private Stack<Card> Shuffle(Stack<Card> _deck)
        {
            Stack<Card> newDeck = new Stack<Card>();
            Card[] deckArray = _deck.ToArray();

            Random r = new Random();
            for (int i = deckArray.Length - 1; i > 0; --i)
            {
                int k = r.Next(i + 1);
                Card temp = deckArray[i];
                deckArray[i] = deckArray[k];
                deckArray[k] = temp;
            }

            float tempDeckHighestZIndex = GameConstants.RENDER_FLOOR;
            foreach (Card card in deckArray)
            {
                card.ZIndex = (tempDeckHighestZIndex -= GameConstants.RENDER_CARD_DEPTH);
                newDeck.Push(card);
            }

            return newDeck;
        }

        /*
        public Stack<Card> GetDeck()
        {
            Stack<Card> tempDeck = deck;
            return tempDeck;
        }
         */

        public int Count()
        {
            return deck.Count;
        }

        public override string ToString()
        {
            StringBuilder s = new StringBuilder();

            foreach (Card c in deck)
                s.Append(" IN DECK " + c.ToString());

            return s.ToString(); 
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        public override bool Equals(object obj)
        {            
            if (this.GetType() != obj.GetType())
                return false;

            bool equal = true;
            Deck objDeck = (Deck)obj;

            if (this.Count() != objDeck.Count())
                return false;

            Card[] deck1Array = deck.ToArray();
            Card[] deck2Array = objDeck.ToArray();

            for (int i = 0; i < this.Count(); i++)
            {
                if (!(deck1Array[i].Equals(deck2Array[i])))
                    equal = false;
            }

            return equal;            
        }

        public IEnumerator<Card> GetEnumerator()
        {
            return deck.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}

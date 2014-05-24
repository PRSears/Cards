using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Text;
using Cards.Structs;

namespace Cards.Player
{
    public class Hand : IEnumerable<Card>
    {
        private Game _game;
        private List<Card> _hand;
        private Vector2 _position;

        private const float OFFSET = 50f;

        public Vector2 Position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
                float xOffset = 0;
                foreach (Card card in _hand)
                {
                    card.Position = new Vector2(this.Position.X + xOffset, this.Position.Y);
                    xOffset += OFFSET;
                }
            }
        }

        public Hand(List<Card> cardsInHand, Game game)
        {
            _hand = cardsInHand;
            _game = game;
            Sort();
            Position = new Vector2(10, 300);
            Update();
        }

        public Hand(Game game)
        {
            _hand = new List<Card>();
            _game = game;
            Position = new Vector2(10, 300);
        }
       
        // Cards are going to get instanciated before going into the hand, so no overloads are necessary

        public void AddCard(Card card)
        {
            // Do nothing if the card is null
            if (card == null)
                return;

            _hand.Add(card);
            Sort();            
            Update();
        }

        public Card RemoveCard(Card card)
        {
            foreach (Card c in _hand)
            {
                if (c.Equals(card))
                {
                    _hand.Remove(c);
                    Update();
                    return c;
                }
            }
            return null;
        }

        private void Update()
        {
            float z = GameConstants.RENDER_FLOOR;
            float x = 0f;
            foreach (Card c in _hand)
            {
                c.ZIndex = z;
                c.Active = true;
                c.Placement = CardPlacement.Hand;
                c.SetSize();
                c.Position = new Vector2(this.Position.X + x, this.Position.Y);

                // Update z value making sure it doesn't go above the ceiling
                if (z > GameConstants.RENDER_CEILING)
                    z -= GameConstants.RENDER_CARD_DEPTH;
                x += OFFSET;
            }
        }

        private void Sort()
        {
            QuickSort(_hand, 0, _hand.Count - 1);
        }

        private void QuickSort(List<Card> list, int left, int right)
        {
            if (list.Count <= 1)
                return;

            // select a pivot 
            Random r = new Random();
            int pivot = r.Next(left, right);
            int low = left, high = right;

            while (low <= high)
            {
                while (list[low].CompareNameTo(list[pivot]) < 0) low++;
                while (list[high].CompareNameTo(list[pivot]) > 0) high--;
                                
                if (low <= high)
                {
                    Swap(list, low, high);
                    low++;
                    high--;
                }
            }

            if (left < high) QuickSort(list, left, high);
            if (low < right) QuickSort(list, low, right);
        }

        private void Swap(List<Card> list, int a, int b)
        {
            Card temp = list[a];
            list[a] = list[b];
            list[b] = temp;
        }

        public IEnumerator<Card> GetEnumerator()
        {
            return _hand.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}

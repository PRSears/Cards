using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cards.Player
{
    public class Action
    {
        public Action()
        {

        }

        public static Action FromID(int id)
        {
            return new Action();
        }

        public static Action[] FromID(int[] id)
        {
            return new Action[id.Length];
        }
    }
}

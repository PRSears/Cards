using System;
using System.ComponentModel;

namespace Cards.Structs 
{
    public struct GameStage : IComparable
    {
        private string Stage;
        private int StageNum;

        public static GameStage Player1Draw { get { return new GameStage("player1draw", 1); } }
        public static GameStage Player1Turn { get { return new GameStage("player1turn", 2); } }
        public static GameStage Player1Attack { get { return new GameStage("player1attack", 3); } }
        public static GameStage Player2Draw { get { return new GameStage("player2draw", 4); } }
        public static GameStage Player2Turn { get { return new GameStage("player2turn", 5); } }
        public static GameStage Player2Attack { get { return new GameStage("player2attack", 6); } }
        
        private GameStage(string stage, int stageNum)
        {
            Stage = stage;
            StageNum = stageNum;
        }

        public override int GetHashCode()
        {
            return Stage.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (this.GetType() != obj.GetType())
                return false;

            GameStage oGameStage = (GameStage)obj;

            return this.Stage.Equals(oGameStage.Stage);
        }

        public override string ToString()
        {
            return Stage;
        }

        public static Boolean operator ==(GameStage a, GameStage b)
        {
            return (a.Equals(b));
        }

        public static Boolean operator !=(GameStage a, GameStage b)
        {
            return !(a == b);
        }

        public int CompareTo(object obj)
        {
            if (obj.GetType() != this.GetType())
                throw new ArgumentException("obj is not a GameStage.");

            GameStage objGameStage = (GameStage)obj;

            return this.StageNum.CompareTo(objGameStage.StageNum);
        }
    }
}

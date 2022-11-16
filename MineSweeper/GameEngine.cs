using System;
namespace MineSweeper
{
    public class GameEngine
    {
        public GameState State;

        public GameEngine(GameState state)
        {
            State = state;
        }

        public void Click(Move move)
        {
            State.Board.Field[move.Row, move.Column] = (byte)(State.Board.Field[move.Row, move.Column] | Board.SHOWN);
        }

        public Move ParseClick(string move)
        {
            var ret = ParseMove(move);
            if(ret != null)
            {
                ret.IsClick = true;
            }
            return ret;
        }

        private Move ParseMove(string move)
        {
            if(move.Length != 2)
            {
                return null;
            }

            byte row = (byte)(move[0] - 65);
            byte col = (byte)(move[1] - 65);

            if(row >=0 && row < State.Board.Height &&
                col >= 0 && col < State.Board.Width)
            {
                return new Move
                {
                    Row = row,
                    Column = col
                };
            }

            return null;
        }



    }
}


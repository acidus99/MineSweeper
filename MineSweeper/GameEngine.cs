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

        public void UpdateState(Move move)
        {
            if(!State.Board.IsInBounds(move.Row, move.Column))
            {
                return;
            }

            if(move.IsClick)
            {
                if (State.Board.IsShown(move.Row, move.Column))
                {
                    return;
                }
                RevealTile(move.Row, move.Column);
            } else
            {
                State.Board.ToggleFlag(move.Row, move.Column);
            }
            State.Refresh();
        }

        /// <summary>
        /// Reveal the tile. This happens when the player clicks, and recursively is called to expand contiguous empty tiles
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        private void RevealTile(int row, int column)
        {
            State.Board.MarkAsShown(row, column);

            //if they hit a mine, we can stop
            if(State.Board.IsMine(row, column))
            {
                return;
            }

            int count = State.Board.AdjacentMineCount(row, column);

            if(count == 0)
            {
                //Automatically expand out to reveal more, if those tiles aren't already visible
                for (int rx = -1; rx <= 1; rx++)
                {
                    int peekRow = row + rx;

                    if (State.Board.IsValidRow(peekRow))
                    {
                        for (int cx = -1; cx <= 1; cx++)
                        {
                            int peekColumn = column + cx;

                            if(State.Board.IsValidColumn(peekColumn)
                                && !State.Board.IsShown(peekRow, peekColumn))
                            {
                                RevealTile(peekRow, peekColumn);
                            }
                        }
                    }
                }
            }
        }

        public Move ParseClickTile(string move)
        {
            var ret = ParseMove(move);
            if(ret != null)
            {
                ret.IsClick = true;
            }
            return ret;
        }

        public Move ParsePlaceFlag(string move)
        {
            var ret = ParseMove(move);
            if (ret != null)
            {
                ret.IsClick = false;
            }
            return ret;
        }


        private Move ParseMove(string move)
        {
            if(move.Length != 2)
            {
                return null;
            }
            move = move.ToUpper();

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


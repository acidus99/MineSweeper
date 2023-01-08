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

        public Move ParseClickTile(string move)
        {
            var ret = ParseMove(move);
            if (ret != null)
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

        public void UpdateState(Move move)
        {
            if(move.IsCheat)
            {
                AutoWin();
                return;
            }

            if(!State.Board.IsInBounds(move.Row, move.Column))
            {
                return;
            }

            if (move.IsClick)
            {
                if (State.Board.IsShown(move.Row, move.Column))
                {
                    //clicking an already revealed tile that is ajacent to mines
                    //so do "chording"
                    if (State.Board.HasAdjacentMines(move.Row, move.Column))
                    {
                        RevealUnflaggedTiles(move.Row, move.Column);
                    }
                    return;
                }
                RevealTile(move.Row, move.Column);
            }
            else if(!State.Board.IsShown(move.Row, move.Column))
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

                    //because I'm going to do function calls, and this is recursive, lets do some sanity checking before calling the RevealTile/MarkAsShown
                    //even though the MarkAsShown function does bounds checking

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

        /// <summary>
        /// Implments "chording" where surrounding, unflagged tiles are revealed as if you clicked them
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        private void RevealUnflaggedTiles(int row, int column)
        {
            int adjacentFlags = 0;
            for (int rx = -1; rx <= 1; rx++)
            {
                for(int cx = -1; cx <= 1; cx++)
                {
                    if(State.Board.IsFlag(row + rx, column + cx))
                    {
                        adjacentFlags++;
                    }
                }
            }
            //are all adjacent mines flagged?
            if(State.Board.AdjacentMineCount(row, column) == adjacentFlags)
            {
                for (int rx = -1; rx <= 1; rx++)
                {
                    for (int cx = -1; cx <= 1; cx++)
                    {
                        int peekRow = row + rx;
                        int peekColumn = column + cx;

                        //explicitly ensure peek is in bounds. Otherwise we recurse into areas we shouldn't
                        //since IsShown and IsFlag return flase for out-of-bounds guesses
                        if (State.Board.IsInBounds(peekRow,peekColumn) &&
                            !State.Board.IsShown(peekRow,peekColumn) &&
                            !State.Board.IsFlag(peekRow, peekColumn))
                        {
                            RevealTile(peekRow, peekColumn);
                        }
                   }
                }
            }
        }

        private void AutoWin()
        {
            State.IsCheat = true;
            for (int row = 0; row < State.Board.Height; row++)
            {
                for (int column = 0; column < State.Board.Width; column++)
                {
                    //what's in this tile?
                    if (!State.Board.IsMine(row, column))
                    {
                        State.Board.MarkAsShown(row, column);
                    } else
                    {
                        State.Board.ToggleFlag(row, column);
                    }
                }
            }
            State.Refresh();
        }

        private Move ParseMove(string move)
        {
            //some phone/tabley keyboards will add a space when you tell it not to autocorrect the input
            move = move.Trim();

            if(move.ToLower() == "xyzzy")
            {
                return new Move { IsCheat = true };
            }

            //if we don't have 2 letters, its bad
            if(move.Length != 2 || !Char.IsLetter(move[0]) || !Char.IsLetter(move[1]))
            {
                return null;
            }

            byte row = 0;
            byte col = 0;

            //are both letters the same case? if so, its "[row][column]"
            if ((Char.IsLower(move[0]) && Char.IsLower(move[1])) ||
                (Char.IsUpper(move[0]) && Char.IsUpper(move[1])))
            {
                move = move.ToUpper();
                row = (byte)(move[0] - 65);
                col = (byte)(move[1] - 65);
            }
            else
            {
                //one lower, one upper
                if (Char.IsLower(move[0]))
                {
                    //row, col
                    move = move.ToUpper();
                    row = (byte)(move[0] - 65);
                    col = (byte)(move[1] - 65);
                }
                else
                {
                    //col, row
                    move = move.ToUpper();
                    col = (byte)(move[0] - 65);
                    row = (byte)(move[1] - 65);
                }
            }

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

        public static GameState CreateNewGame(int rows = 9, int columns = 9, int mines = 10)
        {
            //do validation
            if (rows < 2 || rows > 26)
            {
                throw new ArgumentException("Rows must be between 2 and 26.");
            }
            if (columns < 2 || columns > 26)
            {
                throw new ArgumentException("Columns must be between 2 and 26.");
            }
            if (mines < 1 || mines > 255 || (mines >= (rows * columns)))
            {
                throw new ArgumentException("Mines must be between 1 and 256, and cannot be more than the size of the board.");
            }

            return new GameState
            {
                StartTime = DateTime.Now,
                Board = Board.GenerateNewBoard(rows, columns, mines)
            };
        }
    }
}

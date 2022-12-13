using System;

using MineSweeper;

namespace MineSweeper.Cgi
{
    public class GameRenderer
    {
        /// <summary>
        /// Should we use Emoji when drawing the game board glyphs?
        /// </summary>
        public bool UseEmoji = true;

        TextWriter Output;
        GameState State;

        public GameRenderer(TextWriter output)
        {
            Output = output;
        }

        public void DrawState(GameState state)
        {
            State = state;
        
            DrawTitle();
            DrawStatus();
            DrawBoard();
        }

        private void DrawStatus()
        {
            if (State.IsComplete)
            {
                if (State.HasHitMine)
                {
                    Output.WriteLine("## 😧 💥 ☠️ ⚰️ 🪦");
                    Output.WriteLine("## You clicked on a mine! You are dead!");
                }
                else
                {
                    Output.WriteLine("## 🎉🎉 You Win! 🎉🎉 ");
                }
                Output.WriteLine($"Mines Cleared {State.ClearedMines}");
                Output.WriteLine($"{Completion} Time: {Math.Truncate(DateTime.Now.Subtract(State.StartTime).TotalSeconds)} s");
                Output.WriteLine();
                Output.WriteLine($"=> {RouteOptions.StartUrl(State.Board.Height, State.Board.Width, State.TotalMines)} Play another game");
            }
            else
            {
                Output.WriteLine($"Total Mines: {State.TotalMines}.");
                Output.WriteLine($"{Completion} Tiles Remaining: {State.RemainingTiles} Time: {Math.Truncate(DateTime.Now.Subtract(State.StartTime).TotalSeconds)} s");
            }
        }

        private void DrawBoard()
        {
            Output.WriteLine("``` Game board");

            DrawColumnLegend();

            bool gameComplete = State.IsComplete;

            for (int row =0; row < State.Board.Height; row++)
            {
                //draw prefix
                Output.Write($"{LegendCharacter(row)} ");
                for (int column = 0; column < State.Board.Width; column++)
                {
                    //do we show it?
                    if (State.Board.IsFlag(row, column))
                    {
                        Output.Write(FlagGlyph);
                    }
                    else if (!State.Board.IsShown(row, column))
                    {
                        //if the game is over reveal all the unflagged mines
                        if (gameComplete && State.Board.IsMine(row, column))
                        {
                            Output.Write(BombGlyph);
                        }
                        else
                        {
                            Output.Write(UnseenGlyph);
                        }
                    }
                    else
                    { 
                        //is shown!
                        //if its a mine, then we clicked it, so show the boom!
                        if (State.Board.IsMine(row, column))
                        {
                            Output.Write(BoomGlyph);
                        }
                        else
                        {
                            int adjacentMines = State.Board.AdjacentMineCount(row, column);
                            if(adjacentMines == 0)
                            {
                                Output.Write("  ");
                            } else
                            {
                                Output.Write(adjacentMines + " ");
                            }
                        }
                    }
                }

                //draw suffix
                Output.Write($" {LegendCharacter(row)}");

                Output.WriteLine();
            }
            DrawColumnLegend();

            Output.WriteLine("```");
            Output.WriteLine();
            if (!gameComplete)
            {
                Output.WriteLine($"=> {RouteOptions.ClickUrl(State)} Click Tile 🤞");
                Output.WriteLine($"=> {RouteOptions.FlagUrl(State)} Place Flag 🚩");
            }
        }

        private string BombGlyph
            => UseEmoji ? "💣" : "M ";

        private string BoomGlyph
            => UseEmoji ? "💥" : "M ";

        private string FlagGlyph
            => UseEmoji ? "🚩" : "X ";

        private string UnseenGlyph
            => UseEmoji ? "· " : ". ";

        private string Completion
            => string.Format("{0:0.0}%", ((Convert.ToDouble(State.RevealedTiles) / Convert.ToDouble(State.SafeTiles))) * 100.0d);

        private void DrawColumnLegend()
        {
            Output.Write("  ");
            for(int i=0; i < State.Board.Width; i++)
            {
                Output.Write(LegendCharacter(i));
                Output.Write(' ');
            }
            Output.WriteLine();
        }

        private char LegendCharacter(int offset)
            => (char)(97 + offset);

        private void DrawTitle()
        {
            Output.WriteLine("# MineSweeper 💣 🧹 💥");
        }

    }
}


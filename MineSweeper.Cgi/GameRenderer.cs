using System;

using MineSweeper;

namespace MineSweeper.Cgi
{
    public class GameRenderer
    {
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
            if(State.IsComplete)
            {
                if(State.HasHitMine)
                {
                    Output.WriteLine("## 😧 💥 ☠️ ⚰️ 🪦");
                    Output.WriteLine("## You clicked on a mine! You are dead!");
                    Output.WriteLine($"Mines Cleared {State.ClearedMines}");
                } else
                {
                    Output.WriteLine("## 🎉🎉 You Win! 🎉🎉 ");
                }
            } else
            {
                Output.WriteLine($"Flags: {State.TotalFlags} Total Mines: {State.TotalMines}");
            }
        }

        private void DrawBoard()
        {
            Output.WriteLine("``` Game board");

            DrawColumnLegend();

            for(int row =0; row < State.Board.Height; row++)
            {
                //draw prefix
                Output.Write($"{LegendCharacter(row)} ");
                for (int column = 0; column < State.Board.Width; column++)
                {
                    //do we show it?
                    if (State.Board.IsFlag(row, column))
                    {
                        Output.Write("🚩");
                    }
                    else if (!State.Board.IsShown(row, column))
                    {
                        Output.Write("· ");
                    }
                    else
                    { 
                        //is shown!
                        if (State.Board.IsMine(row, column))
                        {
                            Output.Write("M ");
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
            if (!State.IsComplete)
            {
                Output.WriteLine($"=> {RouteOptions.ClickUrl(State)} Click Tile 🤞");
                Output.WriteLine($"=> {RouteOptions.FlagUrl(State)} Place Flag 🚩");
            }
        }

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
            Output.WriteLine("# 💣 🧹 💥 MineSweeper ");
        }

    }
}


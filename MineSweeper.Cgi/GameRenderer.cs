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
            DrawBoard();
            DrawState();

        }

        private void DrawBoard()
        {
            Output.WriteLine("``` Game board");

            DrawColumnLegend();

            for(int y =0; y < State.Board.Height; y++)
            {
                //draw prefix
                Output.Write($"{(char)(65 + y)} ");
                for (int x = 0; x < State.Board.Width; x++)
                {
                    byte cellValue = State.Board.Field[y, x];

                    //is it a mine?
                    if ((cellValue & Board.MINE) == Board.MINE)
                    {
                        Output.Write('X');
                    }
                    else if ((cellValue & Board.FLAG) == Board.FLAG)
                    {
                        Output.Write('F');
                    }
                    else
                    {
                        byte count = (byte)(cellValue & Board.NUMBER_MASK);

                        if (count > 0)
                        {
                            Output.Write((int)count);
                        }
                        else
                        {
                            Output.Write('.');
                        }
                    }
                }

                //draw suffix
                Output.Write($" {(char)(65 + y)}");


                Output.WriteLine();
            }
            DrawColumnLegend();

            Output.WriteLine("```");
            Output.WriteLine();
        }

        private void DrawColumnLegend()
        {
            Output.Write("  ");
            for(int i=0; i < State.Board.Width; i++)
            {
                Output.Write((char)(65 + i));
            }
            Output.WriteLine();
        }

        private void DrawTitle()
        {
            Output.WriteLine("# 💣 🧹 MineSweeper?");
            Output.WriteLine($"=> {RouteOptions.HelpUrl} How to play");
        }

        private void DrawState()
        {
            var data = State.ToData();

            Output.WriteLine("# State");
            Output.WriteLine("```");
            Output.WriteLine(data);
            Output.WriteLine("```");
            Output.WriteLine($"=> ?{data} Data in URL");
        }

    }
}


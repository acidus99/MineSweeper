using System;
using Gemini.Cgi;
using MineSweeper;

namespace MineSweeper.Cgi
{
	public static class RouteHandler
	{

		public static void PlayGame(CgiWrapper cgi)
        {

            cgi.Success();

            GameState state = new GameState
            {
                Board = new Board()
                //Board = new Board(123)
            };
            GameRenderer renderer = new GameRenderer(cgi.Writer);
            renderer.DrawState(state);
            Footer(cgi);
        }

        static void Footer(CgiWrapper cgi)
        {
            cgi.Writer.WriteLine();
            cgi.Writer.WriteLine("--");
            cgi.Writer.WriteLine("=> mailto:acidus@gemi.dev Made with 💣 and ❤️ by Acidus");
        }
    }
}


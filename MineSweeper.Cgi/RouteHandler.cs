using System;
using Gemini.Cgi;
using MineSweeper;

namespace MineSweeper.Cgi
{
	public static class RouteHandler
	{

		public static void PlayGame(CgiWrapper cgi)
        {

            GameState state;

            var data = cgi.GetPathInfoParameters("/play/");
            if(data == null || data.Length != 1)
            {
                state = GameState.CreateNewGame();
                cgi.Redirect(RouteOptions.GetPlayUrl(state));
                return;
            }
            cgi.Success();

            state = GameState.FromData(data[0]);

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


using System;
using Gemini.Cgi;
using MineSweeper;

namespace MineSweeper.Cgi
{
	public static class RouteHandler
	{

		public static void StartGame(CgiWrapper cgi)
        {
            GameState state = GameState.CreateNewGame();
            cgi.Redirect(RouteOptions.PlayUrl(state));
        }

        public static void ClickTile(CgiWrapper cgi)
        {
            if(!cgi.HasQuery)
            {
                cgi.Input("Click Tile: Enter coordinates, row, then column (e.g. \"DE\")");
                return;
            }
            GameState state = GetState(cgi, RouteOptions.ClickRoute);
            if (state == null)
            {
                throw new ApplicationException("Bad state");
            }
            GameEngine engine = new GameEngine(state);
            var move = engine.ParseClickTile(cgi.Query);

            if(move == null)
            {
                throw new ApplicationException("Invalid Move");
            }

            cgi.Success();

            engine.UpdateState(move);

            GameRenderer renderer = new GameRenderer(cgi.Writer);
            renderer.DrawState(engine.State);
            Footer(cgi);
        }

        public static void ToggleFlag(CgiWrapper cgi)
        {
            if (!cgi.HasQuery)
            {
                cgi.Input("Placee Flag: Enter coordinates, row, then column (e.g. \"DE\")");
                return;
            }
            GameState state = GetState(cgi, RouteOptions.FlagRoute);
            if (state == null)
            {
                throw new ApplicationException("Bad state");
            }
            GameEngine engine = new GameEngine(state);
            var move = engine.ParsePlaceFlag(cgi.Query);

            if (move == null)
            {
                throw new ApplicationException("Invalid Move");
            }

            cgi.Success();

            engine.UpdateState(move);

            GameRenderer renderer = new GameRenderer(cgi.Writer);
            renderer.DrawState(engine.State);
            Footer(cgi);
        }


        public static void PlayGame(CgiWrapper cgi)
        {
            GameState state = GetState(cgi, RouteOptions.PlayRoute);
            if(state == null)
            {
                throw new ApplicationException("Bad state");
            }

            cgi.Success();

            GameRenderer renderer = new GameRenderer(cgi.Writer);
            renderer.DrawState(state);
            Footer(cgi);
        }

        private static GameState GetState(CgiWrapper cgi, string routePrefix)
        {
            //this accesses PATH_INFO, which, per the CGI spec, has already been URL decoded...
            //so we won't URL decode it again, since that will convert the "+" which is a valid BASE64 encoded
            //character into whitespace, which breaks Base64 decoding
            if(cgi.PathInfo.Length <= routePrefix.Length)
            {
                return null;
            }

            //var data = cgi.PathInfo.Substring(routePrefix.Length);
            //JetForce has an bug with how it generates PathInfo, so lets hack around it ourselves
            // https://github.com/michael-lazar/jetforce/issues/69
            var data = System.Net.WebUtility.UrlDecode(cgi.RequestUrl.AbsolutePath.Replace(cgi.ScriptName, ""));
            data = data.Substring(routePrefix.Length);
            if (data.Length > 1)
            {
                //trim trailing path slash
                data = data.Substring(0, data.Length - 1);
            }

            return GameState.FromData(data);
        }


        static void Footer(CgiWrapper cgi)
        {
            cgi.Writer.WriteLine();
            cgi.Writer.WriteLine("--");
            cgi.Writer.WriteLine("=> mailto:acidus@gemi.dev Made with 💣 and ❤️ by Acidus");
        }
    }
}


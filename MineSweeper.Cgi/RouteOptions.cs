using System;
using System.Net;
namespace MineSweeper.Cgi
{
	public static class RouteOptions
	{
		private const string BaseCgiPath = "/cgi-bin/mines.cgi";
        
        public const string ClickRoute = "/click/";
        public const string FlagRoute = "/flag/";
        public const string PlayRoute = "/play/";
        public const string StartRoute = "/start";

        public static string RootUrl
            => BaseCgiPath;

        public static string HowToUrl
            => BaseCgiPath + "/how-to.gmi";

        public static string StartUrl(int rows, int columns, int mines)
            => $"{BaseCgiPath}{StartRoute}?{rows},{columns},{mines}";

        public static string PlayUrl(GameState state)
            => $"{BaseCgiPath}{PlayRoute}{WebUtility.UrlEncode(state.ToData())}/";

		public static string ClickUrl(GameState state)
            => $"{BaseCgiPath}{ClickRoute}{WebUtility.UrlEncode(state.ToData())}/";

        public static string FlagUrl(GameState state)
            => $"{BaseCgiPath}{FlagRoute}{WebUtility.UrlEncode(state.ToData())}/";
    }
}
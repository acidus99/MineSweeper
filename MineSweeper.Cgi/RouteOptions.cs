using System;
using System.Net;
namespace MineSweeper.Cgi
{
	public static class RouteOptions
	{
		private const string BaseCgiPath = "/cgi-bin/mines.cgi";

        public const string StartUrl = BaseCgiPath + StartRoute;

        public const string ClickRoute = "/click/";
        public const string PlayRoute = "/play/";
        public const string StartRoute = "/start/";

        public static string PlayUrl(GameState state)
            => $"{BaseCgiPath}{PlayRoute}{WebUtility.UrlEncode(state.ToData())}/";

		public static string ClickUrl(GameState state)
            => $"{BaseCgiPath}{ClickRoute}{WebUtility.UrlEncode(state.ToData())}/";

	}
}
using System;
using System.Net;
namespace MineSweeper.Cgi
{
	public static class RouteOptions
	{
		private const string BaseCgiPath = "/cgi-bin/mines.cgi";
		public const string PlayUrl = BaseCgiPath + "/play/";

		public static string GetPlayUrl(GameState state)
        {
			var raw = state.ToData();
			var enc = WebUtility.UrlEncode(raw);
			return $"{PlayUrl}{enc}/";
        }
			

	}
}
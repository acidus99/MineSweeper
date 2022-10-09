using System;
namespace MineSweeper.Cgi
{
	public static class RouteOptions
	{
		private const string BaseCgiPath = "/cgi-bin/mines.cgi";
		
		public const string HelpUrl = BaseCgiPath + "/help.gmi";
		public const string FaqUrl = BaseCgiPath + "/faq.gmi";
	}
}
using System;

using Gemini.Cgi;

namespace MineSweeper.Cgi
{
    class Program
    {

        static void Main(string[] args)
        {
            var router = new CgiRouter();
            router.SetStaticRoot("static/");
            router.OnRequest("", RouteHandler.PlayGame);
            router.ProcessRequest();
        }

    }
}

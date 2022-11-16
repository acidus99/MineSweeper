﻿using System;

using Gemini.Cgi;

namespace MineSweeper.Cgi
{
    class Program
    {

        static void Main(string[] args)
        {
            var router = new CgiRouter();
            router.SetStaticRoot("static/");
            router.OnRequest(RouteOptions.StartRoute, RouteHandler.StartGame);
            router.OnRequest(RouteOptions.PlayRoute, RouteHandler.PlayGame);
            router.OnRequest(RouteOptions.ClickRoute, RouteHandler.Click);
            router.ProcessRequest();
        }
    }
}

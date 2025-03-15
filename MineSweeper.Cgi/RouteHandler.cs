using System;
using Gemini.Cgi;
using MineSweeper;

namespace MineSweeper.Cgi;

public static class RouteHandler
{
    public static void StartGame(CgiWrapper cgi)
    {
        //default to easy if not specified
        if (!cgi.HasQuery)
        {
            cgi.Input(
                "Enters rows, columns, and mines for game. For example: '8,12,15' for board with 8 rows, 12 columns, and 15 mines");
            return;
        }

        var parsedInts = ParseGameOptions(cgi.Query);
        if (parsedInts == null)
        {
            cgi.Failure("Must specify 3 numbers, in the format '[rows],[columns],[mines]'");
            return;
        }

        try
        {
            var state = GameEngine.CreateNewGame(parsedInts[0], parsedInts[1], parsedInts[2]);
            cgi.Redirect(RouteOptions.PlayUrl(state));
        }
        catch (Exception ex)
        {
            cgi.Failure(ex.Message);
        }
    }

    public static void ClickTile(CgiWrapper cgi)
    {
        if (!cgi.HasQuery)
        {
            cgi.Input("Click Tile: Enter coordinates, row, then column (e.g. \"DE\")");
            return;
        }

        var state = GetState(cgi, RouteOptions.ClickRoute);
        if (state == null) throw new ApplicationException("Bad state");
        var engine = new GameEngine(state);
        var move = engine.ParseClickTile(cgi.Query);

        if (move == null)
        {
            cgi.Failure("Invalid move. Must specify 1 row and 1 column. (e.g. \"DE\")");
            return;
        }

        cgi.Success();

        engine.UpdateState(move);
        RenderGame(cgi, engine.State);
        Footer(cgi);
    }

    public static void ToggleFlag(CgiWrapper cgi)
    {
        if (!cgi.HasQuery)
        {
            cgi.Input("Place Flag: Enter coordinates, row, then column (e.g. \"DE\")");
            return;
        }

        var state = GetState(cgi, RouteOptions.FlagRoute);
        if (state == null) throw new ApplicationException("Bad state");
        var engine = new GameEngine(state);
        var move = engine.ParsePlaceFlag(cgi.Query);

        if (move == null) cgi.Failure("Invalid move. Must specify 1 row and 1 column. (e.g. \"DE\")");

        cgi.Success();

        engine.UpdateState(move);
        RenderGame(cgi, engine.State);
        Footer(cgi);
    }

    public static void PlayGame(CgiWrapper cgi)
    {
        var state = GetState(cgi, RouteOptions.PlayRoute);
        if (state == null) throw new ApplicationException("Bad state");

        cgi.Success();
        RenderGame(cgi, state);
        Footer(cgi);
    }

    private static GameState GetState(CgiWrapper cgi, string routePrefix)
    {
        //this accesses PATH_INFO, which, per the CGI spec, has already been URL decoded...
        //so we won't URL decode it again, since that will convert the "+" which is a valid BASE64 encoded
        //character into whitespace, which breaks Base64 decoding
        if (cgi.PathInfo.Length <= routePrefix.Length) return null;

        var data = cgi.PathInfo.Substring(routePrefix.Length);
        if (data.Length > 1)
            //trim trailing path slash
            data = data.Substring(0, data.Length - 1);

        return GameState.FromData(data);
    }

    private static void RenderGame(CgiWrapper cgi, GameState state)
    {
        var renderer = new GameRenderer(cgi.Writer)
        {
            //if its not a CGI, we are debugging, so don't use emjoi
            UseEmoji = CgiWrapper.IsRunningAsCgi
        };
        renderer.DrawState(state);
    }

    private static int[] ParseGameOptions(string qs)
    {
        var ret = new int[3];

        var nums = qs.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        if (nums.Length != 3) return null;

        for (var i = 0; i < 3; i++)
            if (!int.TryParse(nums[i], out ret[i]))
                return null;
        return ret;
    }

    private static void Footer(CgiWrapper cgi)
    {
        cgi.Writer.WriteLine();
        cgi.Writer.WriteLine("--");
        cgi.Writer.WriteLine($"=> {RouteOptions.RootUrl} Play New Game");
        cgi.Writer.WriteLine($"=> {RouteOptions.HowToUrl} How To Play");
        cgi.Writer.WriteLine("--");
        cgi.Writer.WriteLine("=> mailto:acidus@gemi.dev Made with 💣 and ❤️ by Acidus");
    }
}
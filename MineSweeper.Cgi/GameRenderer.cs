using System;
using MineSweeper;

namespace MineSweeper.Cgi;

public class GameRenderer
{
    private TextWriter _output;
    private GameState _state;

    /// <summary>
    /// Should we use Emoji when drawing the game board glyphs?
    /// </summary>
    public bool UseEmoji = true;

    public GameRenderer(TextWriter output)
    {
        _output = output;
    }

    private string BombGlyph
        => UseEmoji ? "💣" : "M ";

    private string BoomGlyph
        => UseEmoji ? "💥" : "M ";

    private string FlagGlyph
        => UseEmoji ? "🚩" : "X ";

    private string UnseenGlyph
        => UseEmoji ? "· " : ". ";

    private string Completion
        => string.Format("{0:0.0}%",
            Convert.ToDouble(_state.RevealedTiles) / Convert.ToDouble(_state.SafeTiles) * 100.0d);

    public void DrawState(GameState state)
    {
        _state = state;

        DrawTitle();
        DrawStatus();
        DrawBoard();
    }

    private void DrawStatus()
    {
        if (_state.IsComplete)
        {
            if (_state.HasHitMine)
            {
                _output.WriteLine("## 😧 💥 ☠️ ⚰️ 🪦");
                _output.WriteLine("## You clicked on a mine! You are dead!");
            }
            else
            {
                _output.WriteLine("## 🎉🎉 You Win! 🎉🎉 ");
                if (_state.IsCheat) _output.WriteLine("... but at the cost of your integrity");
            }

            _output.WriteLine($"Mines Cleared {_state.ClearedMines}");
            _output.WriteLine(
                $"{Completion} Time: {Math.Truncate(DateTime.Now.Subtract(_state.StartTime).TotalSeconds)} s");
            _output.WriteLine();
            _output.WriteLine(
                $"=> {RouteOptions.StartUrl(_state.Board.Height, _state.Board.Width, _state.TotalMines)} Play another game");
        }
        else
        {
            _output.WriteLine($"Total Mines: {_state.TotalMines}.");
            _output.WriteLine(
                $"{Completion} Tiles Remaining: {_state.RemainingTiles} Time: {Math.Truncate(DateTime.Now.Subtract(_state.StartTime).TotalSeconds)} s");
        }
    }

    private void DrawBoard()
    {
        _output.WriteLine("``` Game board");

        DrawColumnLegend();

        var gameComplete = _state.IsComplete;

        for (var row = 0; row < _state.Board.Height; row++)
        {
            //draw prefix
            _output.Write($"{LegendCharacter(row)} ");
            for (var column = 0; column < _state.Board.Width; column++)
                //do we show it?
                if (_state.Board.IsFlag(row, column))
                {
                    _output.Write(FlagGlyph);
                }
                else if (!_state.Board.IsShown(row, column))
                {
                    //if the game is over reveal all the unflagged mines
                    if (gameComplete && _state.Board.IsMine(row, column))
                        _output.Write(BombGlyph);
                    else
                        _output.Write(UnseenGlyph);
                }
                else
                {
                    //is shown!
                    //if its a mine, then we clicked it, so show the boom!
                    if (_state.Board.IsMine(row, column))
                    {
                        _output.Write(BoomGlyph);
                    }
                    else
                    {
                        var adjacentMines = _state.Board.AdjacentMineCount(row, column);
                        if (adjacentMines == 0)
                            _output.Write("  ");
                        else
                            _output.Write(adjacentMines + " ");
                    }
                }

            //draw suffix
            _output.Write($" {LegendCharacter(row)}");

            _output.WriteLine();
        }

        DrawColumnLegend();

        _output.WriteLine("```");
        _output.WriteLine();
        if (!gameComplete)
        {
            _output.WriteLine($"=> {RouteOptions.ClickUrl(_state)} Click Tile 🤞");
            _output.WriteLine($"=> {RouteOptions.FlagUrl(_state)} Place Flag 🚩");
        }
    }

    private void DrawColumnLegend()
    {
        _output.Write("  ");
        for (var i = 0; i < _state.Board.Width; i++)
        {
            //columns are uppercase!
            _output.Write(LegendCharacter(i, true));
            _output.Write(' ');
        }

        _output.WriteLine();
    }

    private char LegendCharacter(int offset, bool isUpper = false)
    {
        return isUpper ? (char)(65 + offset) : (char)(97 + offset);
    }

    private void DrawTitle()
    {
        _output.WriteLine("# MineSweeper 💣 🧹 💥");
    }
}
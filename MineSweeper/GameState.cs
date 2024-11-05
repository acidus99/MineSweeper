using System;
using MineSweeper.Utils;

namespace MineSweeper;

public class GameState
{
    public Board Board { get; set; }

    public DateTime StartTime { get; set; }

    //How many mines have correctly been flagged?
    public int ClearedMines { get; private set; } = 0;

    //have they hit a mine?
    public bool HasHitMine { get; private set; } = false;

    //Is the game complete?
    public bool IsComplete
        => HasHitMine || RevealedTiles == SafeTiles;

    /// <summary>
    /// How many tiles in this game don't contain a bomb?
    /// </summary>
    public int SafeTiles
        => Board.Area - TotalMines;

    public int RemainingTiles
        => SafeTiles - RevealedTiles;

    /// <summary>
    /// How many titles in the board are visible?
    /// </summary>
    public int RevealedTiles { get; internal set; } = 0;

    /// <summary>
    /// how many flags are deployed?
    /// </summary>
    public int TotalFlags { get; private set; } = 0;

    /// <summary>
    /// how many mines are on the board?
    /// </summary>
    public int TotalMines { get; private set; } = 0;

    public bool IsCheat { get; set; }

    public string ToData()
    {
        var data = new List<byte>(Board.Height * Board.Width);
        data.AddRange(BitConverter.GetBytes(StartTime.Ticks));
        data.Add((byte)'|');
        data.Add(Board.Width);
        data.Add(Board.Height);
        data.Add((byte)'|');
        for (var y = 0; y < Board.Height; y++)
        for (var x = 0; x < Board.Width; x++)
            data.Add(Board.Field[y, x]);
        return UrlSafeBase64Encode(GzipUtils.Compress(data.ToArray()));
    }

    private static string UrlSafeBase64Encode(byte[] data)
    {
        return Convert.ToBase64String(data).Replace('+', '-').Replace("/", "_");
    }

    private static byte[] UrlSafeBase64Decode(string base64)
    {
        return Convert.FromBase64String(base64.Replace('-', '+').Replace("_", "/"));
    }

    public static GameState FromData(string base64Data)
    {
        try
        {
            var decoded = UrlSafeBase64Decode(base64Data);
            var data = GzipUtils.Decompress(decoded);

            if (data[8] != '|' || data[11] != '|') return null;

            var state = new GameState
            {
                StartTime = new DateTime(BitConverter.ToInt64(data, 0)),
                Board = new Board(data[9], data[10])
            };

            var index = 12;
            for (var row = 0; row < state.Board.Height; row++)
            for (var column = 0; column < state.Board.Width; column++)
            {
                state.Board.Field[row, column] = data[index];
                index++;
            }

            state.Refresh();
            return state;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public void Refresh()
    {
        TotalMines = 0;
        RevealedTiles = 0;
        TotalFlags = 0;
        ClearedMines = 0;

        for (var row = 0; row < Board.Height; row++)
        for (var column = 0; column < Board.Width; column++)
        {
            var isMine = false;

            //what's in this tile?
            if (Board.IsMine(row, column))
            {
                isMine = true;
                TotalMines++;
            }

            if (Board.IsShown(row, column))
            {
                RevealedTiles++;
                if (isMine)
                    //we hit a mine!
                    HasHitMine = true;
            }

            if (Board.IsFlag(row, column))
            {
                TotalFlags++;
                if (isMine)
                    //successfully flagged a mine
                    ClearedMines++;
            }
        }
    }
}
using System;
using System.Data.Common;

namespace MineSweeper;

public class Board
{
    private const byte Mine = 64;
    private const byte Flag = 32;
    private const byte Shown = 128;
    private const byte NumberMask = 15;

    public byte[,] Field;

    internal Board(byte width, byte height)
    {
        Width = width;
        Height = height;
        Field = new byte[Height, Width];
    }

    public byte Width { get; private set; }
    public byte Height { get; private set; }

    public int Mines { get; private set; }

    /// <summary>
    /// What is the area of the board?
    /// </summary>
    public int Area => Width * Height;

    //is the tile a mine?
    public bool IsMine(int row, int column)
    {
        return !IsInBounds(row, column) ? false : TileHasProperty(row, column, Mine);
    }

    //is the tile visible?
    public bool IsShown(int row, int column)
    {
        return !IsInBounds(row, column) ? false : TileHasProperty(row, column, Shown);
    }

    //does the tile have a flag on it?
    public bool IsFlag(int row, int column)
    {
        return !IsInBounds(row, column) ? false : TileHasProperty(row, column, Flag);
    }

    public bool IsInBounds(int row, int column)
    {
        return IsValidRow(row) && IsValidColumn(column);
    }

    public bool IsValidRow(int row)
    {
        return row >= 0 && row < Height;
    }

    public bool IsValidColumn(int column)
    {
        return column >= 0 && column < Width;
    }

    public bool HasAdjacentMines(int row, int column)
    {
        return !IsInBounds(row, column) ? false : AdjacentMineCount(row, column) > 0;
    }

    public int AdjacentMineCount(int row, int column)
    {
        if (!IsInBounds(row, column)) return 0;
        return Field[row, column] & NumberMask;
    }

    public void MarkAsShown(int row, int column)
    {
        if (IsInBounds(row, column)) Field[row, column] = (byte)(Field[row, column] | Shown);
    }

    public void ToggleFlag(int row, int column)
    {
        if (IsInBounds(row, column)) Field[row, column] = (byte)(Field[row, column] ^ Flag);
    }

    private bool TileHasProperty(int row, int column, byte property)
    {
        return (Field[row, column] & property) == property;
    }

    /// <summary>
    /// Generates a new board object, randomly populated with with mines
    /// </summary>
    /// <param name="rows"></param>
    /// <param name="cols"></param>
    /// <param name="mines"></param>
    internal static Board GenerateNewBoard(int rows, int cols, int mines)
    {
        var rand = new Random(Convert.ToInt32(DateTime.Now.Ticks & 0xFFFF));
        var board = new Board(Convert.ToByte(cols), Convert.ToByte(rows));

        //init board
        for (var y = 0; y < board.Height; y++)
        for (var x = 0; x < board.Width; x++)
            board.Field[y, x] = 0;

        board.Mines = mines;
        var placed = 0;
        while (placed < board.Mines)
        {
            var y = rand.Next(board.Height);
            var x = rand.Next(board.Width);
            if (board.Field[y, x] == 0)
            {
                board.Field[y, x] = Mine;
                placed++;
            }
        }

        //compute and place adjacency numbers
        for (var y = 0; y < board.Height; y++)
        for (var x = 0; x < board.Width; x++)
            if (board.Field[y, x] == 0)
            {
                byte count = 0;
                for (var my = -1; my < 2; my++)
                for (var mx = -1; mx < 2; mx++)
                    if (board.IsMine(y + my, x + mx))
                        count++;
                board.Field[y, x] = count;
            }

        return board;
    }
}
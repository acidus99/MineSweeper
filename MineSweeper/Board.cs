using System;
using System.Data.Common;

namespace MineSweeper
{
    public class Board
    {
        const byte MINE = 64;
        const byte FLAG = 32;
        const byte SHOWN = 128;
        const byte NUMBER_MASK = 15;

        public byte Width { get; private set; }
        public byte Height { get; private set; }

        public int Mines { get; private set; }

        public byte[,] Field;

        public Board( byte width = 15, byte height = 9)
        { 
            Width = width;
            Height = height;
            Field = new byte[Height, Width];
        }

        public static int GetSeed()
            => Convert.ToInt32(DateTime.Now.Ticks & 0xFFFF);

        public void GenerateNewBoard()
            => GenerateNewBoard(GetSeed());

        public void GenerateNewBoard(int seed)
        {
            Random rand = new Random(seed);

            //init board
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    Field[y, x] = 0;
                }
            }

            Mines = 10;
            int placed = 0;
            while(placed < Mines)
            {
                int y = rand.Next(Height);
                int x = rand.Next(Width);
                if (Field[y,x] == 0)
                {
                    Field[y, x] = MINE;
                    placed++;
                }
            }
            //compute numbers
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (Field[y, x] == 0)
                    {
                        byte count = 0;
                        for (int my = -1; my < 2; my++)
                        {
                            for (int mx = -1; mx < 2; mx++)
                            {
                                if (IsMine(y + my, x + mx))
                                {
                                    count++;
                                }
                            }
                        }
                        Field[y, x] = count;
                    }
                }
            }

        }

        //is the tile a mine?
        public bool IsMine(int row, int column)
            => !IsInBounds(row, column) ? false : TileHasProperty(row, column, MINE);

        //is the tile visible?
        public bool IsShown(int row, int column)
            => !IsInBounds(row, column) ? false : TileHasProperty(row, column, SHOWN);

        //does the tile have a flag on it?
        public bool IsFlag(int row, int column)
            => !IsInBounds(row, column) ? false : TileHasProperty(row, column, FLAG);

        public bool IsInBounds(int row, int column)
            => IsValidRow(row) && IsValidColumn(column);

        public bool IsValidRow(int row)
            => row >= 0 && row < Height;

        public bool IsValidColumn(int column)
            => column>= 0 && column< Width;

        public bool HasAdjacentMines(int row, int column)
            => !IsInBounds(row, column) ? false : (AdjacentMineCount(row, column) > 0);

        public int AdjacentMineCount(int row, int column)
        {
            if(!IsInBounds(row, column))
            {
                return 0;
            }
            return (Field[row, column] & Board.NUMBER_MASK);
        }

        public void MarkAsShown(int row, int column)
        {
            if (IsInBounds(row, column))
            {
                Field[row, column] = (byte)(Field[row, column] | Board.SHOWN);
            }
        }

        public void ToggleFlag(int row, int column)
        {
            if (IsInBounds(row, column))
            {
                Field[row, column] = (byte)(Field[row, column] ^ Board.FLAG);
            }
        }

        private bool TileHasProperty(int row, int column, byte property)
            => (Field[row, column] & property) == property;
    }
}
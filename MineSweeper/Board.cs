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

        /// <summary>
        /// What is the area of the board?
        /// </summary>
        public int Area => Width * Height;

        internal Board( byte width, byte height)
        { 
            Width = width;
            Height = height;
            Field = new byte[Height, Width];
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
            for (int y = 0; y < board.Height; y++)
            {
                for (int x = 0; x < board.Width; x++)
                {
                    board.Field[y, x] = 0;
                }
            }

            board.Mines = mines;
            int placed = 0;
            while (placed < board.Mines)
            {
                int y = rand.Next(board.Height);
                int x = rand.Next(board.Width);
                if (board.Field[y, x] == 0)
                {
                    board.Field[y, x] = MINE;
                    placed++;
                }
            }
            //compute and place adjacency numbers
            for (int y = 0; y < board.Height; y++)
            {
                for (int x = 0; x < board.Width; x++)
                {
                    if (board.Field[y, x] == 0)
                    {
                        byte count = 0;
                        for (int my = -1; my < 2; my++)
                        {
                            for (int mx = -1; mx < 2; mx++)
                            {
                                if (board.IsMine(y + my, x + mx))
                                {
                                    count++;
                                }
                            }
                        }
                        board.Field[y, x] = count;
                    }
                }
            }

            return board;
        }
    }
}
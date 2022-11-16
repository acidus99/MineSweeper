using System;
namespace MineSweeper
{
    public class Board
    {
        public const byte MINE = 64;
        public const byte FLAG = 32;
        public const byte SHOWN = 128;
        public const byte NUMBER_MASK = 15;

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
                                if (CheckMine(y + my, x + mx))
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

        private bool CheckMine(int row, int column)
        {
            if (row < 0 || row >= Height || column < 0 || column >= Width)
            {
                //outside the the board
                return false;
            }
            return (Field[row, column] == MINE);
        }            

    }
}


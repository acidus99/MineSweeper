using System;
namespace MineSweeper
{
    public class Board
    {
        public const byte MINE = 64;
        public const byte FLAG = 32;
        public const byte NUMBER_MASK = 15;

        public int Width { get; private set; }
        public int Height { get; private set; }

        public int Mines { get; private set; }

        public byte[,] Field;

        public int Seed;

        Random rand;

        public Board()
            : this(GetSeed())
        {
        }

        public Board(int seed)
        {
            Seed = seed;
            Width = 15;
            Height = 9;
            rand = new Random(Seed);

            GenerateBoard();
        }

        public static int GetSeed()
            => Convert.ToInt32(DateTime.Now.Ticks & 0xFFFF);

        
        private void GenerateBoard()
        {
            Field = new byte[Height, Width];

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


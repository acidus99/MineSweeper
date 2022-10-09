using System;
namespace MineSweeper
{
    public class GameState
    {
        public Board Board { get; set; }

        public bool IsValid { get; private set; } = true;

        public string ToData()
        {
            var data = new List<byte>((Board.Height * Board.Width) + 4);
            data.Add((byte)'|');
            data.Add(Board.Width);
            data.Add(Board.Height);
            data.Add((byte)'|');
            for(int y = 0; y<Board.Height; y++)
            {
                for(int x=0; x < Board.Width; x++)
                {
                    data.Add(Board.Field[y, x]);
                }
            }
            return Convert.ToBase64String(GzipUtils.Compress(data.ToArray()));
        }

        public static GameState FromData(string base64Data)
        {
            byte[] data = GzipUtils.Decompress(Convert.FromBase64String(base64Data));
            if (data[0] != '|' || data[3] != '|')
            {
                return new GameState
                {
                    IsValid = false
                };
            }
            var board = new Board(data[1], data[2]);
            int index = 4;
            for (int y = 0; y < board.Height; y++)
            {
                for (int x = 0; x < board.Width; x++)
                {
                    board.Field[y, x] = data[index];
                    index++;
                }
            }
            return new GameState
            {
                Board = board
            };
        }

        public static GameState CreateNewGame()
        {
            var board = new Board();
            board.GenerateNewBoard();
            return new GameState
            {
                Board = board
            };
        }

    }
}


using System;
namespace MineSweeper
{
    public class GameState
    {
        public Board Board { get; set; }

        //Is the game state valid?
        public bool IsValid { get; private set; } = true;

        //Is the game complete?
        public bool IsComplete
            => HasHitMine || HasUncoveredAllSafeSquares;

        //have they hit a mine?
        public bool HasHitMine { get; private set; } = false;

        public bool HasUncoveredAllSafeSquares
            => RevealedTiles == (Board.Area - TotalMines); 

        //how many mines are on the board?
        public int TotalMines { get; private set; } = 0;

        //how many flags are deployed?
        public int TotalFlags { get; private set; } = 0;

        //How many mines have correctly been flagged?
        public int ClearedMines { get; private set; } = 0;

        //How many mines left does the player think they have?
        public int MinesLeft
            => TotalFlags - TotalMines;

        //How many titles in the board are visible?
        public int RevealedTiles { get; internal set; } = 0;

        public string ToData()
        {
            var data = new List<byte>((Board.Height * Board.Width) + 4)
            {
                (byte)'|',
                Board.Width,
                Board.Height,
                (byte)'|'
            };
            for (int y = 0; y<Board.Height; y++)
            {
                for(int x=0; x < Board.Width; x++)
                {
                    data.Add(Board.Field[y, x]);
                }
            }
            return UrlSafeBase64Encode(GzipUtils.Compress(data.ToArray()));
        }

        private static string UrlSafeBase64Encode(byte[] data)
            => Convert.ToBase64String(data).Replace('+', '-').Replace("/", "_");

        private static byte[] UrlSafeBase64Decode(string base64)
            => Convert.FromBase64String(base64.Replace('-', '+').Replace("_", "/"));

        public static GameState FromData(string base64Data)
        {
            var decoded = UrlSafeBase64Decode(base64Data);
            byte[] data = GzipUtils.Decompress(decoded);

            if (data[0] != '|' || data[3] != '|')
            {
                return new GameState
                {
                    IsValid = false
                };
            }

            var state = new GameState
            {
                Board = new Board(data[1], data[2])
            };

            int index = 4;
            for (int row = 0; row < state.Board.Height; row++)
            {
                for (int column = 0; column < state.Board.Width; column++)
                {
                    state.Board.Field[row, column] = data[index];
                    index++;
                }
            }

            state.Refresh();

            return state;
        }

        public void Refresh()
        {
            TotalMines = 0;
            RevealedTiles = 0;
            TotalFlags = 0;
            ClearedMines = 0;

            for (int row = 0; row < Board.Height; row++)
            {
                for (int column = 0; column < Board.Width; column++)
                {
                    bool isMine = false;

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
                        {
                            //we hit a mine!
                            HasHitMine = true;
                        }
                    }
                    if (Board.IsFlag(row, column))
                    {
                        TotalFlags++;
                        if (isMine)
                        {
                            //successfully flagged a mine
                            ClearedMines++;
                        }
                    }
                }
            }
        }

        public static GameState CreateNewGame(byte width = 15, byte height = 9)
        {
            var board = new Board(width, height);
            board.GenerateNewBoard();
            return new GameState
            {
                Board = board
            };
        }
    }
}
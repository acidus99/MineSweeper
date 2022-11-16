using System;
namespace MineSweeper
{
    public class Move
    {
        public bool IsClick { get; set; } = true;
        public byte Row { get; set; }
        public byte Column { get; set; }
    }
}


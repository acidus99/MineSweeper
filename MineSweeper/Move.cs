using System;

namespace MineSweeper;

public class Move
{
    //if its not a click, its placing a flag
    public bool IsClick { get; set; } = true;
    public byte Row { get; set; }
    public byte Column { get; set; }

    public bool IsCheat { get; set; } = false;
}
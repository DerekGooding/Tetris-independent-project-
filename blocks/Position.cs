namespace Tetris.blocks;

public struct Position(int height, int width)
{
    public int Height { get; set; } = height;

    public int Width { get; set; } = width;
}
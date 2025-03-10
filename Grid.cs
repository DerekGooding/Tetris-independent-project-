using Tetris.blocks;

namespace Tetris;

public class Grid
{
    public const int Height = 20;
    public const int Width  = 10;
    public static int[,] NewGrid { get; } = new int[Height, Width];
    public static ConsoleColor[,] ColorGrid { get; } = new ConsoleColor[20, 10];

    public Position? StartPoint;
    public int NewPiece;
    public static int Score { get; set; }

    public int this[int w, int h]
    {
        get => NewGrid[w, h];
        set => NewGrid[w, h] = value;
    }

    public Position Start
    {
        get => new(10 / 2, 20);
        set => StartPoint = value;
    }

    public int WhichBlock
    {
        get => Random.Shared.Next(Tetrominos.PiecesPool.Length);
        set => NewPiece = value;
    }

    public static bool IsCellEmpty(int h, int w) => NewGrid[h, w] == 0;

    public static bool IsRowFull(int h)
    {
        for (var w = 0; w < Width; w++)
        {
            if (NewGrid[h, w] == 0)
            {
                return false;
            }
        }
        return true;
    }

    public static bool IsRowEmpty(int h)
    {
        for (var w = 0; w < Width; w++)
        {
            if (NewGrid[h, w] != 0)
            {
                return false;
            }
        }
        return true;
    }

    public static void ClearRow(int h)
    {
        if (IsRowFull(h))
        {
            for (var w = 0; w < Width; w++)
            {
                NewGrid[h, w] = 0;
            }

            Score++;
        }
    }

    public static void ClearBoard()
    {
        for (var h = 0; h < Height; h++)
        {
            for (var w = 0; w < Width; w++)
            {
                NewGrid[h, w] = 0;
            }
        }
    }

    public static void RowDown(int h, int rowNumber)
    {
        for (var w = 0; w < Width; w++)
        {
            NewGrid[h + rowNumber, w] = NewGrid[h, w];
            NewGrid[h, w] = 0;
        }
    }

    public static int ClearFullRows()
    {
        var clear = 0;

        for (var h = Height - 1; h >= 0; h--)
        {
            if (IsRowFull(h))
            {
                ClearRow(h);
                clear++;
            }
            else if (clear > 0)
            {
                RowDown(h, clear);
            }
        }
        return clear;
    }

    public static bool IsGameOver()
    {
        for (var w = 0; w < Width; w++)
        {
            if (NewGrid[0, w] != 0)
            {
                return true;
            }
        }
        return false;
    }
}
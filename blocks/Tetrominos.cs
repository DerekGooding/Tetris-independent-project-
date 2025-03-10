﻿using System;

namespace Tetris.blocks;

public static class Tetrominos
{
    public static Tetromino[] PiecesPool { get; } =
   [
        new("Square",
            [new(0,0), new(1,0), new(0, 1), new(1, 1)],
            ConsoleColor.Yellow),
        new("Long",
            [new(0, 0), new(-1, 0), new(1, 0), new(2, 0)],
            ConsoleColor.Cyan),
        new("T Block",
            [new(0, 0), new(-1, 0), new(1, 0), new(0, 1)],
            ConsoleColor.Magenta),
        new("J Block",
            [new(0, 0), new(-1, 0), new(1, 0), new(1, 1)],
            ConsoleColor.Blue),
        new("L Block",
            [new(0, 0), new(-1, 0), new(1, 0), new(-1, 1)],
            ConsoleColor.DarkYellow),
        new("Blarg",
            [new(0, 0), new(-1, 0), new(1, 0), new(0, 1)],
            ConsoleColor.Green),
    ];

    private static Tetromino _currentPiece;
    private static Position _positionOfPiece;
    private static bool _gameOver;
    public static bool Pause { get; set; }

    public static bool CanMove(int dx, int dy)
    {
        foreach (var p in _currentPiece.Postions)
        {
            var x = _positionOfPiece.X + p.X + dx;
            var y = _positionOfPiece.Y + p.Y + dy;
            if (x < 0 || x >= Grid.Width || y < 0 || y >= Grid.Height || y >= 0 && Grid.NewGrid[y, x] != 0)
                return false;
        }
        return true;
    }

    public static void NewPiece()
    {
        _currentPiece = PiecesPool[Random.Shared.Next(PiecesPool.Length)];
        _positionOfPiece = new(Grid.Width / 2, 0);
        if (!CanMove(0, 0)) _gameOver = true;
    }

    private static void PlacePiece()
    {
        var pieceColor = _currentPiece.Color;                    

        foreach (var p in _currentPiece.Postions)
        {
            var x = _positionOfPiece.X + p.X;
            var y = _positionOfPiece.Y + p.Y;
            if (y >= 0)
            {
                Grid.NewGrid[y, x] = 1;
                Grid.ColorGrid[y, x] = pieceColor;
            }
        }
    }

    public static void MovePiece(int dx, int dy)
    {
        if (CanMove(dx, dy))
        {
            _positionOfPiece.X += dx;
            _positionOfPiece.Y += dy;
        }
        else if (dy > 0)
        {
            PlacePiece();
            Grid.ClearFullRows();
            NewPiece();
        }
    }

    public static bool CanRotate(Position[] rotatedPiece)
    {
        foreach (var (px, py) in rotatedPiece)
        {
            var x = _positionOfPiece.X + px;
            var y = _positionOfPiece.Y + py;
            if (x < 0 || x >= Grid.Width || y < 0 || y >= Grid.Height || y >= 0 && Grid.NewGrid[y, x] != 0)
                return false;
        }
        return true;
    }

    public static void RotatePiece()
    {
        var rotatedPiece = _currentPiece.Postions
            .Select(cell => new Position(-cell.Y, cell.X))
            .ToArray();

        if (CanRotate(rotatedPiece))
            _currentPiece.Postions = rotatedPiece;
    }

    private static void DrawBorder()
    {
        const int startX = 0;
        const int startY = 0;

        ForegroundColor = ConsoleColor.White;

        SetCursorPosition(startX, startY);
        Write("╔" + new string('═', Grid.Width) + "╗");

        for (var i = 0; i < Grid.Height; i++)
        {
            SetCursorPosition(startX, startY + i + 1);
            Write("║");
            SetCursorPosition(startX + Grid.Width + 1, startY + i + 1);
            Write("║");
        }

        SetCursorPosition(startX, startY + Grid.Height + 1);
        Write("╚" + new string('═', Grid.Width) + "╝");

        ResetColor();
    }

    public static void DrawBoard()
    {
        DrawBorder();
        SetCursorPosition(0, 0);

        for (var y = 0; y < Grid.Height; y++)
        {
            SetCursorPosition(1, y + 1);
            for (var x = 0; x < Grid.Width; x++)

            {
                var isPiece = false;
                var color = ConsoleColor.White;

                foreach (var p in _currentPiece.Postions)
                {
                    if (_positionOfPiece.X + p.X == x && _positionOfPiece.Y + p.Y == y)
                    {
                        isPiece = true;
                        color = _currentPiece.Color;
                        break;
                    }
                }

                if (!isPiece && Grid.NewGrid[y, x] != 0)
                    color = Grid.ColorGrid[y, x];

                ForegroundColor = color;
                Write(isPiece || Grid.NewGrid[y, x] != 0 ? "X" : ".");
            }

            WriteLine();
        }
        const int textX = Grid.Width + 4;
        const int textY = 2;
        ResetColor();
        WriteLine();
        WriteLine($"  Score: {Grid.Score}");
        WriteLine("    _._     _,-'\"\"`-._\r\n     (,-.`._,'(       |\\`-/|\r\n         `-.-' \\ )-`( , o o)\r\n             `-    \\`_`\"'-");

        if (Grid.Score > 10)
        {
            SetCursorPosition(textX, textY);
            ForegroundColor = ConsoleColor.Cyan;
            WriteLine("Nice!");
        }

        if (Grid.Score > 20)
        {
            SetCursorPosition(textX, textY + 2);
            ForegroundColor = ConsoleColor.Green;
            WriteLine("Great job!");
        }

        if (Grid.Score > 30)
        {
            SetCursorPosition(textX, textY + 4);
            ForegroundColor = ConsoleColor.Red;
            WriteLine("Excellent!");
        }

        if (Grid.Score > 40)
        {
            SetCursorPosition(textX, textY + 6);
            ForegroundColor = ConsoleColor.Magenta;
            WriteLine("Fantastic!");
        }

        if (Grid.Score > 50)
        {
            SetCursorPosition(textX, textY + 8);
            ForegroundColor = ConsoleColor.Yellow;
            WriteLine("Increrdible!!");
        }

        if (Grid.Score > 60)
        {
            SetCursorPosition(textX, textY + 10);
            ForegroundColor = ConsoleColor.Blue;
            WriteLine("Out of this world!!");
        }

        ResetColor();
    }

    public static void ReadInput()
    {
        while (!_gameOver)
        {
            var key = ReadKey(true).Key;

            if (key == ConsoleKey.LeftArrow) MovePiece(-1, 0);
            if (key == ConsoleKey.RightArrow) MovePiece(1, 0);
            if (key == ConsoleKey.DownArrow) MovePiece(0, 1);
            if (key == ConsoleKey.UpArrow) RotatePiece();
            if (key == ConsoleKey.Spacebar)
            {
                Pause = !Pause;
                Clear();
                WriteLine("           P A U S E ");
                WriteLine();
                WriteLine();
                WriteLine("======== TETRIS CONTROLS ========");
                WriteLine("⬅  Left Arrow      - Move left ");
                WriteLine("➡  Right Arrow     - Move right ");
                WriteLine("⬇  Down Arrow      - Soft drop  ");
                WriteLine("⬆  Up Arrow        - Rotate piece  ");
                WriteLine("Spacebar           - P A U S E");
                WriteLine("=================================");
                WriteLine(". . . . . . .");
                WriteLine();
                WriteLine($"  Score: {Grid.Score}");
                WriteLine("    _._     _,-'\"\"`-._\r\n     (,-.`._,'(       |\\`-/|\r\n         `-.-' \\ )-`( , o o)\r\n             `-    \\`_`\"'-");

                while (true)
                {
                    var resumeKey = ReadKey(true).Key;
                    if (resumeKey == ConsoleKey.Spacebar)
                    {
                        Pause = false;
                        Clear();
                        break;
                    }
                }
            }
        }
    }
}
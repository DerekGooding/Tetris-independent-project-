namespace Tetris.blocks;

public static class Tetrominos
{
    public static (int x, int y)[][] PiecesPool { get; } =
   [
        //making an array of arrays,
        //for the blocks center point is (0,0) based on x, y coordinates
        //so for example in the square piece, (0,0) is the first block
        //(1,0) is describing one block to the right on the x axis
        //(0,1) is describing one block directly above on the y axis
        //(1,1) describes a block one to the right and one up

        [(0,0), (1,0), (0,1), (1,1)], //square

        [(0,0), (-1,0), (1,0), (2,0)], //long

        [(0,0), (-1,0), (1,0), (0,1)], //t piece

        [(0,0), (-1,0), (1,0), (1,1)], //j block

        [(0,0), (-1,0), (1,0), (-1,1)], //l block

        [(0,0), (-1,0), (1, 0), (0, 1)], //The blarg (supposed to make you say "blarg" in anger)
    ];

    public static ConsoleColor[] PieceColors { get; } =
    [
    ConsoleColor.Yellow,
    ConsoleColor.Cyan,
    ConsoleColor.Magenta,
    ConsoleColor.Blue,
    ConsoleColor.DarkYellow,
    ConsoleColor.Green,
    ConsoleColor.Red
    ];

    private static (int x, int y)[] _currentPiece = [];
    private static (int x, int y) _positionOfPiece;
    private const int _boardWidth = 10, _boardHeight = 20;
    private static bool _gameOver;
    private static int _currentPieceIndex;

    public static bool CanMove(int dx, int dy)                     //couldn't figure this out on my own so I
    {                                                       //found this part online.
        foreach (var (px, py) in _currentPiece)              //defines the elements of each dimension of the piece(x or y)
        {                                                   //to see if the piece is in play, if it is inside the gameboard
            var x = _positionOfPiece.x + px + dx;
            var y = _positionOfPiece.y + py + dy;
            if (x < 0 || x >= _boardWidth || y < 0 || y >= _boardHeight || y >= 0 && Grid.NewGrid[y, x] != 0)
                return false;
        }
        return true;
    }

    public static void NewPiece()       //picks a random array from the array
                                        //and positions it at the top and center of the board
                                        //if it hits zero it starts over
    {
        _currentPieceIndex = Random.Shared.Next(PiecesPool.Length);
        _currentPiece = PiecesPool[_currentPieceIndex];
        _positionOfPiece = (_boardWidth / 2, 0);
        if (!CanMove(0, 0)) _gameOver = true;
    }

    private static void PlacePiece()                                                 //couldn't figure this out on my own
                                                                                     //so I found this part online.
    {                                                                           //basically changes pieces from active to set so nex piece can spawn, works by adding the current
        var pieceColor = PieceColors[_currentPieceIndex];                    //pieces position (px/py = relative positions of each cell that makes up piece)
                                                                            //to the game grid [y, x] when it reaches the bottom and marks the cell as occupied (1)

        foreach (var (px, py) in _currentPiece)
        {
            var x = _positionOfPiece.x + px;
            var y = _positionOfPiece.y + py;
            if (y >= 0)
            {
                Grid.NewGrid[y, x] = 1;
                Grid.ColorGrid[y, x] = pieceColor;
            }
        }
    }

    public static void MovePiece(int dx, int dy)        //this just determines if the piece can move or if its set
    {                                                   //if it CanMove, it accepts inputs and adds them to change the position of the piece (dx = horizontal movement of piece, dy vertical movement of piece)
        if (CanMove(dx, dy))                            //if not, it places the piece, checks for full rows, then initiates a new piece
        {
            _positionOfPiece.x += dx;
            _positionOfPiece.y += dy;
        }
        else if (dy > 0)
        {
            PlacePiece();
            Grid.ClearFullRows();
            NewPiece();
        }
    }

    public static bool CanRotate((int x, int y)[] rotatedPiece)
    {
        foreach (var (px, py) in rotatedPiece)
        {
            var x = _positionOfPiece.x + px;
            var y = _positionOfPiece.y + py;
            if (x < 0 || x >= _boardWidth || y < 0 || y >= _boardHeight || y >= 0 && Grid.NewGrid[y, x] != 0)
                return false;
        }
        return true;
    }

    public static void RotatePiece()
    {
        var rotatedPiece = _currentPiece
            .Select(cell => (-cell.y, cell.x))
            .ToArray();

        if (CanRotate(rotatedPiece))
            _currentPiece = rotatedPiece;
    }

    private static void DrawBorder()
    {
        const int startX = 0;
        const int startY = 0;

        ForegroundColor = ConsoleColor.White;

        SetCursorPosition(startX, startY);
        Write("╔" + new string('═', _boardWidth) + "╗");

        for (var i = 0; i < _boardHeight; i++)
        {
            SetCursorPosition(startX, startY + i + 1);
            Write("║");
            SetCursorPosition(startX + _boardWidth + 1, startY + i + 1);
            Write("║");
        }

        SetCursorPosition(startX, startY + _boardHeight + 1);
        Write("╚" + new string('═', _boardWidth) + "╝");

        ResetColor();
    }

    public static void DrawBoard()                  //wrote about half of this, got some help with the part that determines where the piece is.
                                                    //this draws the gameboard and checks if the grid is currently occupied by a piece
    {
        DrawBorder();
        SetCursorPosition(0, 0);

        for (var y = 0; y < _boardHeight; y++)            //uses a bool to check cells for a piece, if it does it registers true it draws an x
        {
            SetCursorPosition(1, y + 1); //otherwise it does . for empty space
            for (var x = 0; x < _boardWidth; x++)

            {
                var isPiece = false;
                var color = ConsoleColor.White;

                foreach (var (px, py) in _currentPiece)
                {
                    if (_positionOfPiece.x + px == x && _positionOfPiece.y + py == y)
                    {
                        isPiece = true;
                        color = PieceColors[_currentPieceIndex];
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
        const int textX = _boardWidth + 4;
        const int textY = 2;
        ResetColor();
        WriteLine();
        WriteLine($"  Score: {Grid.Score}");
        WriteLine("    _._     _,-'\"\"`-._\r\n     (,-.`._,'(       |\\`-/|\r\n         `-.-' \\ )-`( , o o)\r\n             `-    \\`_`\"'-");

        SetCursorPosition(textX, textY); if (Grid.Score > 10)
        {
            ForegroundColor = ConsoleColor.Cyan;
            WriteLine("Nice!");
        }
        ForegroundColor = ConsoleColor.White;
        SetCursorPosition(textX, textY + 2); if (Grid.Score > 20)
        {
            ForegroundColor = ConsoleColor.Green;
            WriteLine("Great job!");
        }

        ForegroundColor = ConsoleColor.White;

        SetCursorPosition(textX, textY + 4); if (Grid.Score > 30)
        {
            ForegroundColor = ConsoleColor.Red;
            WriteLine("Excellent!");
        }
        ForegroundColor = ConsoleColor.White;

        SetCursorPosition(textX, textY + 6); if (Grid.Score > 40)
        {
            ForegroundColor = ConsoleColor.Magenta;
            WriteLine("Fantastic!");
        }
        ForegroundColor = ConsoleColor.White;

        SetCursorPosition(textX, textY + 8); if (Grid.Score > 50)
        {
            ForegroundColor = ConsoleColor.Yellow;
            WriteLine("Increrdible!!");
        }
        ForegroundColor = ConsoleColor.White;

        SetCursorPosition(textX, textY + 10); if (Grid.Score > 60)
        {
            ForegroundColor = ConsoleColor.Blue;
            WriteLine("Out of this world!!");
        }
        ForegroundColor = ConsoleColor.White;
    }

    public static bool Pause { get; set; }

    public static void ReadInput()              //this is for inputs that determine how the piece move
    {                                           //currently i have an up input just so i can play around with it
        while (!_gameOver)                       //later i'll change that
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
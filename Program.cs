using NAudio.Wave;
using Tetris.blocks;

namespace Tetris;

public static class Program
{
    private static WaveOutEvent? _waveOut;
    private static AudioFileReader? _audioFile;

    private static readonly string[] _playlist =
    [
        "7pm.mp3",
        "9pm.mp3",
        "3-32 Museum - Welcome to the Museum!.mp3",
    ];

    public static void Main()
    {
        PlayRandomSong();

        string? input;
        Tetrominos.NewPiece();
        var inputThread = new Thread(Tetrominos.ReadInput);
        inputThread.Start();
        SetCursorPosition(0, 0);
        CursorVisible = false;

        while (true)
        {
            if (Grid.IsGameOver())
            {
                Clear();
                WriteLine("G A M E  O V E R");
                WriteLine($"Score: {Grid.Score}");
                WriteLine();
                WriteLine();
                WriteLine("www.youtube.com/watch?v=sDipbctxGC4");
                WriteLine("        .\r\n       -.\\_.--._.______.-------.___.---------.___\r\n       )`.                                       `-._\r\n      (                                              `---.\r\n      /o                                                  `.\r\n     (                                                      \\\r\n   _.'`.  _                                                  L\r\n   .'/| \"\" \"\"\"\"._                                            |\r\n      |          \\             |                             J\r\n                  \\-._          \\                             L\r\n                  /   `-.        \\                            J\r\n                 /      /`-.      )_                           `\r\n                /    .-'    `    J  \"\"\"\"-----.`-._             |\\            \r\n              .'   .'        L   F            `-. `-.___        \\`.\r\n           ._/   .'          )  )                `-    .'\"\"\"\"`.  \\)\r\n__________((  _.'__       .-'  J              _.-'   .'        `. \\\r\n                   \"\"\"\"\"\"\"((  .'--.__________(   _.-'___________)..|----------------._____\r\n                            \"\"                \"\"\"               ``U'\r\n");
                WriteLine("Brett Mariani 2025");

                input = ReadLine();
                if (input == "yes")
                {
                    Clear();
                }
                else
                {
                    break;
                }
            }

            if (!Tetrominos.Pause)
            {
                Grid.ClearFullRows();
                Tetrominos.DrawBoard();
                Thread.Sleep(Math.Max(50, 400 - (Grid.Score * 10)));
                Tetrominos.MovePiece(0, 1);
            }
            else
            {
                Thread.Sleep(100);
            }
        }
        SetCursorPosition(0, 0);
    }

    private static void PlayRandomSong()
    {
        if (_waveOut != null)
        {
            _waveOut.Dispose();
            _audioFile?.Dispose();
        }
        var musicDirectory = Path.Combine("Assets", "Audio");

        var randomIndex = Random.Shared.Next(_playlist.Length);
        var filePath = Path.Combine(musicDirectory, _playlist[randomIndex]);

        _waveOut = new WaveOutEvent();
        _audioFile = new AudioFileReader(filePath);

        _waveOut.PlaybackStopped += (s, e) => PlayRandomSong();

        _waveOut.Init(_audioFile);
        _waveOut.Play();
    }
}
using System;
using System.Threading;

namespace SnakeGame
{
    class Start
    {
        static void Main()
        {
            //Loop game Infinitley
            for (; ; )
            {
                //Play with defualt Settings, will be set by a menu in later versions
                Play(20,20,10);
            }
        }
        static void Play(int sizeX, int sizeY, int speed)
        {
            int score = 0;
            InitUi(sizeX, sizeY);
            var snake = new Snake(sizeX, sizeY);
            var controlThread = new Thread(() => Controls(snake));
            controlThread.Start();
            //Make new apples until Apple is not inside of snake
            var apple = new Apple(sizeX, sizeY);
            while (snake.IsTouching(apple.X, apple.Y))apple = new Apple(sizeX, sizeY);
            DateTime start = DateTime.Now;
            Thread.Sleep(500);
            while (!snake.Dead)
            {
                if (DateTime.Now - start > TimeSpan.FromMilliseconds(1000 / speed))
                {
                    start = DateTime.Now;
                    snake.Tick();
                }
                if (snake.IsTouching(apple.X, apple.Y))
                {
                    snake.Length++;
                    score += (int)Math.Ceiling((speed ^ 2 * 1000) / Math.Sqrt(sizeX * sizeY));
                    ShowScore(sizeY, score);
                    while (snake.IsTouching(apple.X, apple.Y))
                    {
                        Console.SetCursorPosition(apple.X, apple.Y);
                        Console.Write("O");
                        apple = new Apple(sizeX, sizeY);
                    }
                }
            }
            controlThread.Abort();
            //Highscores(score);
        }
        static void InitUi(int x, int y)
        {
            Console.CursorVisible = false;
            Console.SetWindowSize(x + 2, y + 7);
            Console.SetBufferSize(x + 2, y + 7);
            Console.SetCursorPosition(0, 0);
            for (int i = 0; i < x + 2; i++) Console.Write("#");
            for (int i = 0; i < y; i++)
            {
                Console.Write("#");
                for (int j = 0; j < x; j++)
                {
                    Console.Write(" ");
                }
                Console.Write("#");
            }
            for (int i = 0; i < x + 2; i++) Console.Write("#");
            int spaces = (int)Math.Ceiling(x / 2f) - 2;
            for (int i = 0; i < spaces; i++)
                Console.Write(" ");
            Console.Write("Snake");
            for (int i = 0; i < spaces; i++)
                Console.Write(" ");
            if (x % 2 == 0) Console.Write(' ');
            ShowScore(y, 0);
        }
        static void ShowScore(int y, int score)
        {
            Console.SetCursorPosition(1, y + 3);
            Console.Write("Score: " + score);
        }
        static void Controls(Snake snake)
        {
            for (; ; )
            {
                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.UpArrow:
                        snake.Up();
                        break;
                    case ConsoleKey.DownArrow:
                        snake.Down();
                        break;
                    case ConsoleKey.LeftArrow:
                        snake.Left();
                        break;
                    case ConsoleKey.RightArrow:
                        snake.Right();
                        break;
                }
            }
        }
    }
}

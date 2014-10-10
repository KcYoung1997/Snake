using System;
using System.Threading;

namespace SnakeGame
{
    class Start
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.Unicode;
            //Loop game Infinitley
            for (; ; )
            {
                //int[] options = MainMenu();
                
                //Play with defualt Settings, will be set by a menu in later versions
                Play(20,20,10);
            }
        }
        static int[] MainMenu()
        {
            int menu1 = Cons.Menu("Snake", new[] { "Start Playing", "Tron (2P) - Out of order", "Looping mode", "View Highscores" });
            return new[] {20, 20, 10};
        }
        static void Play(int sizeX, int sizeY, int speed)
        {
            int score = 0;
            //Create snake from snake.cs and set it's bounds so it dies when is leaves the play area
            var snake = new Snake(sizeX, sizeY);
            //Create a new thread to take in and pass controls to the snake, start it.
            var controlThread = new Thread(() => Controls(snake));
            controlThread.Start();
            //Initialize playing area
            InitUi(sizeX, sizeY);
            //Make new apples until Apple is not inside of snake
            var apple = new Apple(sizeX, sizeY);
            while (snake.IsTouching(apple.X, apple.Y))apple = new Apple(sizeX, sizeY);

            DateTime sinceLastTick = DateTime.Now;
            Thread.Sleep(500);
            while (!snake.Dead)
            {
                if (DateTime.Now - sinceLastTick > TimeSpan.FromMilliseconds(1000 / speed))
                {
                    sinceLastTick = DateTime.Now;
                    snake.Tick();
                }
                //If snake isn't touching apple, skip back to start of while loop
                if (!snake.IsTouching(apple.X, apple.Y)) continue;
                //If it is touching apple
                snake.Length++;
                //Add to the score based on a basic algorithm taking into account size of play area and speed
                score += (int)Math.Ceiling((speed ^ 2 * 1000) / Math.Sqrt(sizeX * sizeY));
                //Update the score's display value
                ShowScore(sizeY, score);
                while (snake.IsTouching(apple.X, apple.Y))
                {
                    //Write snake body to Apples previous location
                    Console.SetCursorPosition(apple.X, apple.Y);
                    Console.Write("O");
                    //Spawn new apple
                    apple = new Apple(sizeX, sizeY);
                }
            }
            //Stop controls thread.
            controlThread.Abort();
            //Highscores(score);
        }
        static void InitUi(int x, int y)
        {
            Console.CursorVisible = false;
            Cons.SetWindowSize(x + 2, y + 7);
            Console.SetCursorPosition(0, 0);
            Cons.WriteRowOf('#');
            for (int i = 0; i < y; i++)
            {
                Console.Write("#");
                for (int j = 0; j < x; j++)
                {
                    Console.Write(" ");
                }
                Console.Write("#");
            }
            Cons.WriteRowOf('#');
            Cons.WriteCentered("Snake");
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

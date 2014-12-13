using System;
using System.Threading;
using System.Linq;
using System.Windows.Input;
using System.IO;
using System.Collections.Generic;

namespace SnakeGame
{
    static class Start
    {

        static void Main()
        {
            for (; ; )
            {
                int[] options = MainMenu();
                if (options[3] == 0)
                    Play(options[0], options[1], options[2], false);
                if (options[3] == 1)
                    PlayTron(options[0], options[1], options[2]);
                if (options[3] == 2)
                    Play(options[0], options[1], options[2], true);
            }
        }
        static int[] MainMenu()
        {
            int mode = int.MaxValue;
            while (mode > 2)
            {
                if (mode == 3)
                {
                    if (!File.Exists("Highscores"))
                    {
                        mode = Cons.Menu("Snake", new[] { "Start Playing", "Tron (2P)", "Looping mode", "No Highscores" });
                        continue;
                    }
                    string[] scores;
                    using (var reader = new StreamReader("Highscores"))
                    {
                        var lines = new List<string>(reader.ReadToEnd().Split(new [] { "\r\r\n", "\r\n", "\n" }, StringSplitOptions.None));
                        lines.Remove("");
                        scores = lines.ToArray();
                    }
                }
                mode = Cons.Menu("Snake", new[] { "Start Playing", "Tron (2P)", "Looping mode", "View Highscores" });
            }
            int x = (Cons.Menu("Board X", Enumerable.Range(2, (Console.LargestWindowWidth - 2) / 10 - 1).Select(i => (i*10).ToString()).ToArray()) + 2) * 10;
            int y = (Cons.Menu("Board Y", Enumerable.Range(2, (Console.LargestWindowHeight - 5) / 10 - 1).Select(i => (i*10).ToString()).ToArray()) + 2) * 10;
            int speed = (Cons.Menu("Snake Speed", new[] { "10", "20", "30", "40", "50", "60", "70", "80", "90", "100" }) + 1) * 10;
            return new[] { x, y, speed, mode };

        }
        static void Play(int sizeX, int sizeY, int speed, bool loop)
        {
            int score = 0;
            //Initialize playing area
            InitUi(sizeX, sizeY);
            //Create snake from snake.cs and set it's bounds so it dies when is leaves the play area
            var snake = new Snake(sizeX, sizeY) {Loop = loop};
            //Create a new thread to take in and pass controls to the snake, start it.
            var controlThread = new Thread(() => Controls(snake,
                Key.Up,
                Key.Down,
                Key.Left,
                Key.Right));
            controlThread.SetApartmentState(ApartmentState.STA);
            controlThread.Start();
            //Make new apples until Apple is not inside of snake
            var apple = new Apple(sizeX, sizeY);
            while (snake.IsTouching(apple.X, apple.Y))apple = new Apple(sizeX, sizeY);

            DateTime sinceLastTick = DateTime.Now;
            Thread.Sleep(500);
            int ticks = 0;
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
                score += (int)Math.Ceiling((speed ^ 2 * 1000) / Math.Sqrt(sizeX * sizeY / 2));
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
                ticks += 1;
            }
            //Stop controls thread.
            controlThread.Abort();
            Console.CursorTop = Console.WindowHeight / 2;
            Cons.WriteCentered("Game Over");
            PostStats(new [] {loop ? 1 : 0, score, snake.Length - 4, ticks});
        }
        static void PlayTron(int sizeX, int sizeY, int speed)
        {
            //Initialize playing area
            InitUi(sizeX, sizeY);
            //Create snake from snake.cs and set it's bounds so it dies when is leaves the play area
            var player1 = new Snake(sizeX, sizeY, (sizeX + 2) / 4, (sizeY + 2) / 2, 1);
            var player2 = new Snake(sizeX, sizeY, ((sizeX + 2) / 4) * 3, (sizeY + 2) / 2, 1);
            player1.Length = int.MaxValue;
            player2.Length = int.MaxValue;
            //Create a new thread to take in and pass controls to the snake, start it.
            var player1Control = new Thread(() => Controls(player1,
                Key.W,
                Key.S,
                Key.A,
                Key.D));
            var player2Control = new Thread(() => Controls(player2,
                Key.Up,
                Key.Down,
                Key.Left,
                Key.Right));
            player1Control.SetApartmentState(ApartmentState.STA);
            player2Control.SetApartmentState(ApartmentState.STA);
            player1Control.Start();
            player2Control.Start();
            //--STAT--
            int killedByWall = 0;
            DateTime sinceLastTick = DateTime.Now;
            Thread.Sleep(500);
            while (!player1.Dead && !player2.Dead)
            {
                if (DateTime.Now - sinceLastTick > TimeSpan.FromMilliseconds(3000 / speed))
                {
                    sinceLastTick = DateTime.Now;
                    player1.Tick();
                    player2.Tick();
                }
                if (player1.Dead || player2.Dead)
                {
                    killedByWall = 1;
                    break;
                }
                //If snake 1's head is in player 1 or 2, kill it.
                if (player2.IsTouching(player1.Head.Key, player1.Head.Value)) player1.Dead = true;
                //If snake 2's head is in player 1, kill it.
                if (player1.IsTouching(player2.Head.Key, player2.Head.Value)) player2.Dead = true;
            }
            //Stop controls thread.
            player1Control.Abort();
            player2Control.Abort();
            Console.SetCursorPosition(1, sizeY + 3);
            int player1Wins = player2.Dead && !player1.Dead ? 1 : 0;
            int player2Wins = player1.Dead && !player2.Dead ? 0 : 1;
            Console.Clear();
            Console.CursorTop = Console.WindowHeight/2;
            Cons.WriteCentered("Game Over");
            Cons.WriteCentered( player1.Dead && player2.Dead ?
                                "Both Players Died!" :
                                "Player " + (player1.Dead ? "1" : "2") + " Died");
            Thread.Sleep(2000);
            PostStats(new [] {2, player1Wins, killedByWall});

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
        static void Controls(Snake snake, Key up, Key down, Key left, Key right)
        {
             
            for (; ; )
            {
                if (Keyboard.IsKeyDown(up))
                    snake.Up();
                else if (Keyboard.IsKeyDown(down))
                    snake.Down();
                else if (Keyboard.IsKeyDown(left))
                    snake.Left();
                else if (Keyboard.IsKeyDown(right))
                    snake.Right();
                //Thread.Sleep(5);
            }
        }
        static void PostStats(int[] info)
        {

            if (!File.Exists("Stats"))
                File.WriteAllLines("Stats", (new string[11]).Select(x => "0"));
            string[] scores = File.ReadAllLines("Stats");
            int gameMode = info[0];
            if (gameMode == 0) // Original
            {
                scores[0] = info[1] > int.Parse(scores[0]) ? info[1].ToString() : scores[0]; //High Score
                scores[1] = info[2] > int.Parse(scores[1]) ? info[2].ToString() : scores[1]; //Most apples eaten
                scores[2] = info[3] > int.Parse(scores[2]) ? info[3].ToString() : scores[2]; //Longest Ticks Survived
                scores[3] = (int.Parse(scores[3]) + info[2]).ToString(); //Total apples eaten
            }
            else if (gameMode == 1) // Looping
            {
                scores[4] = info[1] > int.Parse(scores[4]) ? info[1].ToString() : scores[4]; //High Score
                scores[5] = info[2] > int.Parse(scores[5]) ? info[2].ToString() : scores[5]; //Most apples eaten
                scores[6] = (int.Parse(scores[6]) + info[2]).ToString(); //Total apples eaten
            }
            else //Tron
            {
                //7 = p1 wins     8 = p2 wins
                scores[7 + info[1]] = (int.Parse(scores[7 + info[1]]) + 1).ToString();
                //9 = Deaths by Wall    10 = Deaths By Bike
                scores[9 + info[2]] = (int.Parse(scores[9 + info[2]]) + 1).ToString();
            }
            File.WriteAllLines("Stats", scores);
            if (gameMode == 2) return;
            var highscores = new List<int>();
            if (File.Exists("Highscores")) 
                highscores = File.ReadAllLines("Highscores").Select(int.Parse).ToList();
            highscores.Add(info[1]);
            highscores.Sort();
            File.WriteAllLines( "Highscores", highscores.Select(x => x.ToString()) );

        }
    }
}
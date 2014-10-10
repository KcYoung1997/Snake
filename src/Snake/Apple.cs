using System;
namespace SnakeGame
{
    class Apple
    {
        public int X;
        public int Y;
        public Apple(int maxX, int maxY)
        {
            var rnd = new Random();
            X = rnd.Next(1, maxX + 1);
            Y = rnd.Next(1, maxY + 1);
            Console.SetCursorPosition(X, Y);
            Console.WriteLine('A');
        }
    }
}
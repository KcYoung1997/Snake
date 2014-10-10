using System;
using System.Collections.Generic;

namespace SnakeGame
{
    class Snake
    {
        private List<KeyValuePair<int, int>> tail = new List<KeyValuePair<int, int>>();
        private string _direction = "up";
        private string _lastDirection = "up";
        public int Length = 4;
        private int maxX;
        private int maxY;
        public bool Dead = false;
        public Snake(int x, int y)
        {
            maxX = x;
            maxY = y;
            for (int i = 0; i < 4; i++)
            {
                tail.Add(new KeyValuePair<int, int>((x + 2) / 2, (y + 2) / 2 - i));
                Console.SetCursorPosition((x + 2) / 2, (y + 2) / 2 - i);
                Console.Write("O");
            }
        }
        public void Tick()
        {
            while (tail.Count >= Length)
            {
                KeyValuePair<int, int> tailEnd = tail[0];
                Console.SetCursorPosition(tailEnd.Key, tailEnd.Value);
                Console.Write(" ");
                tail.Remove(tailEnd);
            }
            int x = tail[tail.Count - 1].Key;
            int y = tail[tail.Count - 1].Value;
            switch (_direction)
            {
                case "up":
                    y -= 1;
                    break;
                case "down":
                    y += 1;
                    break;
                case "left":
                    x -= 1;
                    break;
                case "right":
                    x += 1;
                    break;
            }
            if (ShouldDie(x, y))
            {
                Dead = true;
                return;
            }
            Console.SetCursorPosition(tail[0].Key, tail[0].Value);
            Console.Write("0");
            tail.Add(new KeyValuePair<int, int>(x, y));
            Console.SetCursorPosition(x, y);
            Console.Write("0");
            Console.SetCursorPosition(tail[tail.Count - 2].Key, tail[tail.Count - 2].Value);
            Console.Write("O");
            _lastDirection = _direction;
        }
        public void Up()
        {
            if (_lastDirection != "down")
            {
                _direction = "up";
            }
        }
        public void Down()
        {
            if (_lastDirection != "up")
            {
                _direction = "down";
            }
        }
        public void Left()
        {
            if (_lastDirection != "right")
            {
                _direction = "left";
            }
        }
        public void Right()
        {
            if (_lastDirection != "left")
            {
                _direction = "right";
            }
        }
        private bool ShouldDie(int x, int y)
        {
            //True if any of the following are true i.e. Collision with itself or leaving the playing area.
            return IsTouching(x, y) ||
                   x > maxX ||
                   x < 1 ||
                   y > maxY ||
                   y < 1;
        }
        public bool IsTouching(int x, int y)
        {
            //True if tail
            return tail.Contains(new KeyValuePair<int, int>(x, y));
        }
    }
}
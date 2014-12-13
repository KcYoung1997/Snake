using System;
using System.Collections.Generic;
using System.Linq;

namespace SnakeGame
{
    class Snake
    {
        private readonly List<KeyValuePair<int, int>> _tail = new List<KeyValuePair<int, int>>();
        private string _direction = "up";
        private string _lastDirection = "up";
        public int Length = 4;
        private readonly int _maxX;
        private readonly int _maxY;
        public bool Loop = false;
        public KeyValuePair<int, int> Head
        {
            get { return _tail.Last(); }
        }
        public bool Dead = false;

        //Pass default centered start position if none supplied
        public Snake(int x, int y) : this(x, y, 4) { }
        public Snake(int x, int y, int len) : this(x, y, (x + 2) / 2, (y + 2) / 2, len) { }
        public Snake(int x, int y, int startX, int startY, int len)
        {
            _maxX = x;
            _maxY = y;
            if (len + startY > y) Length = y - startY;
            else Length = len;
            for (int i = 0; i < len; i++)
            {
                _tail.Add(new KeyValuePair<int, int>(startX, startY - i));
                Console.SetCursorPosition(startX, startY - i);
                Console.Write("O");
            }
        }
        public void Tick()
        {
            while (_tail.Count >= Length)
            {
                KeyValuePair<int, int> tailEnd = _tail[0];
                Console.SetCursorPosition(tailEnd.Key, tailEnd.Value);
                Console.Write(" ");
                _tail.RemoveAt(0);
            }
            int x = _tail[_tail.Count - 1].Key;
            int y = _tail[_tail.Count - 1].Value;
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
                if (!Loop || IsTouching(x, y))
                {
                    Dead = true;
                    return;
                }
                x = x % _maxX;
                y = y % _maxY;
                if (x == 0) x += _maxX;
                if (y == 0) y += _maxY;
            }
            Console.SetCursorPosition(_tail[0].Key, _tail[0].Value);
            Console.Write("0");
            _tail.Add(new KeyValuePair<int, int>(x, y));
            Console.SetCursorPosition(x, y);
            Console.Write("0");
            Console.SetCursorPosition(_tail[_tail.Count - 2].Key, _tail[_tail.Count - 2].Value);
            Console.Write("O");
            _lastDirection = _direction;
        }
        public void Up()
        {
            if (_lastDirection != "down")
                _direction = "up";
        }
        public void Down()
        {
            if (_lastDirection != "up")
                _direction = "down";
        }
        public void Left()
        {
            if (_lastDirection != "right")
                _direction = "left";
        }
        public void Right()
        {
            if (_lastDirection != "left")
                _direction = "right";
        }
        private bool ShouldDie(int x, int y)
        {
            //True if any of the following are true i.e. Collision with itself or leaving the playing area.
            return IsTouching(x, y) ||
                   x > _maxX ||
                   x < 1 ||
                   y > _maxY ||
                   y < 1;
        }
        public bool IsTouching(int x, int y)
        {
            //True if tail
            return _tail.Contains(new KeyValuePair<int, int>(x, y));
        }
    }
}
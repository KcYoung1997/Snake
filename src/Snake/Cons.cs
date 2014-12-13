using System;
using System.IO;
using System.Linq;

namespace SnakeGame
{
    class Cons
    {
        public static void SetWindowSize(int width, int height)
        {
            //Over Max checking
            if (width  > Console.LargestWindowWidth)  width  = Console.LargestWindowWidth;
            if (height > Console.LargestWindowHeight) height = Console.LargestWindowHeight;
            bool worked = false;
            while (!worked)
                try
                {
                    Console.WindowWidth = 1;
                    Console.BufferWidth = width;
                    Console.WindowWidth = width;
                    worked = true;
                }
                catch (IOException) //Under minimum checking
                {
                    width += 1;
                }
            worked = false;
            while (!worked)
                try
                {
                    Console.WindowHeight = 1;
                    Console.BufferHeight = height;
                    Console.WindowHeight = height;
                    worked = true;
                }
                catch (IOException) //Under minimum checking
                {
                    height += 1;
                }
        }
        public static void WriteRowOf(char c)
        {
            Console.CursorLeft = 0;
            for (int i = 0; i < Console.WindowWidth; i++)
                Console.Write(c);
        }
        public static void WriteCentered(string str)
        {
            Console.CursorLeft = 0;
            double gaps = Console.WindowWidth - str.Length;
            Console.Write(new string(' ', (int)Math.Ceiling(gaps/2)));
            Console.Write(str);
            Console.Write(new string(' ', (int)Math.Floor(gaps / 2)));
        }
        public static int Menu(string title, string[] options)
        {
            Console.Clear();
            bool hadCursor = Console.CursorVisible;
            Console.CursorVisible = false;
            //Setup console size, only if needed to increase
            int maxWidth = (from x in options select x.Length).Max() + 6;
            if (title.Length + 4 > maxWidth) maxWidth = title.Length + 4;
            int maxHeight = 6 + (options.Length * 2);
            SetWindowSize(maxWidth, maxHeight);
            //Title
            WriteCentered(new string('#', title.Length + 2));
            WriteCentered("#"+title+"#");
            WriteCentered(new string('#', title.Length + 2));
            Console.CursorTop = 5;
            //Options
            foreach (string option in options)
            {
                WriteCentered(option);
                Console.WriteLine();
            }
            //Selecting option
            int currentOption = 0;
            Cursor('►', options[currentOption].Length, currentOption);
            bool enterPressed = false;
            do
            {
                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.UpArrow:
                        //If at first option, dont move up
                        Cursor(' ', options[currentOption].Length, currentOption);
                        currentOption--;
                        if (currentOption == -1) currentOption = options.Length - 1; 
                        Cursor('►', options[currentOption].Length, currentOption);
                        break;
                    case ConsoleKey.DownArrow:
                        //If at last option, dont move down

                        Cursor(' ', options[currentOption].Length, currentOption);
                        currentOption++;
                        if (currentOption == options.Length) currentOption = 0;
                        Cursor('►', options[currentOption].Length, currentOption);
                        break;
                    case ConsoleKey.Enter:
                        enterPressed = true;
                        break;
                }
            } while (!enterPressed);
            Console.CursorVisible = hadCursor;
            return currentOption;
        }
        private static void Cursor(char c, double width, int height)
        {
            Console.CursorTop = 5 + height * 2;
            Console.CursorLeft = (int)Math.Ceiling((Console.WindowWidth - width) / 2) - 2;
            Console.Write(c);
        }
    }
}

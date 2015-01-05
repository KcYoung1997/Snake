using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace ConsoleExtensions
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct Coord
    {
        internal short X;
        internal short Y;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct SmallRect
    {
        internal short Left;
        internal short Top;
        internal short Right;
        internal short Bottom;
    }

    public class ConsoleWrite
    {
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
            ConsoleSize.SetWindowSize(maxWidth, maxHeight);
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

    public static class ConsoleColors
    {
        //Set every console color to it's default
        static ConsoleColors()
        {
            var csbe = new ConsoleScreenBufferInfoEx();
            csbe.cbSize = Marshal.SizeOf(csbe);                    // 96 = 0x60
            IntPtr hConsoleOutput = GetStdHandle(StdOutputHandle);    // 7
            GetConsoleScreenBufferInfoEx(hConsoleOutput, ref csbe);
            csbe.ColorTable = new[]
            {
                new Colorref(0, 0, 0),
                new Colorref(0, 0, 128),
                new Colorref(0, 128, 0),
                new Colorref(0, 128, 128),
                new Colorref(128, 0, 0),
                new Colorref(128, 0, 128),
                new Colorref(128, 128, 0),
                new Colorref(192, 192, 192),
                new Colorref(128, 128, 128),
                new Colorref(0, 0, 256),
                new Colorref(0, 256, 0),
                new Colorref(0, 256, 256),
                new Colorref(256, 0, 0),
                new Colorref(256, 0, 256),
                new Colorref(256, 256, 0),
                new Colorref(256, 256, 256)
            };
            ++csbe.srWindow.Bottom;
            ++csbe.srWindow.Right;
            SetConsoleScreenBufferInfoEx(hConsoleOutput, ref csbe);
            Console.Clear();
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct Colorref
        {
            internal uint ColorDWORD;
            internal Colorref(Color color)
            {
                ColorDWORD = color.R + (((uint)color.G) << 8) + (((uint)color.B) << 16);
            }
            internal Colorref(uint r, uint g, uint b)
            {
                ColorDWORD = r + (g << 8) + (b << 16);
            }
            internal Color GetColor()
            {
                return Color.FromArgb((int)(0x000000FFU & ColorDWORD),
                    (int)(0x0000FF00U & ColorDWORD) >> 8, (int)(0x00FF0000U & ColorDWORD) >> 16);
            }
            internal void SetColor(Color color)
            {
                ColorDWORD = (uint)color.R + (((uint)color.G) << 8) + (((uint)color.B) << 16);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct ConsoleScreenBufferInfoEx
        {
            internal int cbSize;
            internal Coord dwSize;
            internal Coord dwCursorPosition;
            internal ushort wAttributes;
            internal SmallRect srWindow;
            internal Coord dwMaximumWindowSize;
            internal ushort wPopupAttributes;
            internal bool bFullscreenSupported;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst=16)]
            public Colorref[] ColorTable;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int nStdHandle);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GetConsoleScreenBufferInfoEx(IntPtr hConsoleOutput, ref ConsoleScreenBufferInfoEx csbe);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleScreenBufferInfoEx(IntPtr hConsoleOutput, ref ConsoleScreenBufferInfoEx csbe);

        public static Color[] Colors
        {
            get
            {
                 var csbe = new ConsoleScreenBufferInfoEx();
                csbe.cbSize = Marshal.SizeOf(csbe);                    // 96 = 0x60
                IntPtr hConsoleOutput = GetStdHandle(StdOutputHandle);    // 7
                GetConsoleScreenBufferInfoEx(hConsoleOutput, ref csbe);
                return csbe.ColorTable.Select(x => x.GetColor()).ToArray();
            }
        }
        private static int _colorsUsed;
        private const int StdOutputHandle = -11;
        internal static readonly IntPtr InvalidHandleValue = new IntPtr(-1);

        public static int SetColor(int color, uint r, uint g, uint b)
        {
            var csbe = new ConsoleScreenBufferInfoEx();
            csbe.cbSize = Marshal.SizeOf(csbe);                    // 96 = 0x60
            IntPtr hConsoleOutput = GetStdHandle(StdOutputHandle);    // 7
            if (hConsoleOutput == InvalidHandleValue)
            {
                return Marshal.GetLastWin32Error();
            }
            bool brc = GetConsoleScreenBufferInfoEx(hConsoleOutput, ref csbe);
            if (!brc)
            {
                return Marshal.GetLastWin32Error();
            }
            csbe.ColorTable[color] = new Colorref(r, g, b);
            ++csbe.srWindow.Bottom;
            ++csbe.srWindow.Right;
            if (!SetConsoleScreenBufferInfoEx(hConsoleOutput, ref csbe))
            {
                return Marshal.GetLastWin32Error();
            }
            return 0;
        }

        public static void ForegroundColor(Color color)
        { ForegroundColor(color.R, color.G, color.B); }
        public static void ForegroundColor(uint r, uint g, uint b)
        {
            ConsoleColor consoleColor;

            var index = Array.FindIndex(Colors, x => x == Color.FromArgb((int)r, (int)g, (int)b));
            if (index != -1)
            {
                Enum.TryParse(index.ToString(), out consoleColor);
                Console.ForegroundColor = consoleColor;
                return;
            }

            SetColor(_colorsUsed, r, g, b);

            Enum.TryParse(_colorsUsed.ToString(), out consoleColor);
            Console.ForegroundColor = consoleColor;

            _colorsUsed++;
        }

        public static void BackgroundColor(Color color)
        { BackgroundColor(color.R, color.G, color.B); }
        public static void BackgroundColor(uint r, uint g, uint b)
        {
            ConsoleColor consoleColor;

            var index = Array.FindIndex(Colors, x => x == Color.FromArgb((int)r, (int)g, (int)b));
            if (index != -1)
            {
                Enum.TryParse(index.ToString(), out consoleColor);
                Console.BackgroundColor = consoleColor;
                return;
            }

            SetColor(_colorsUsed, r, g, b);
            Enum.TryParse(_colorsUsed.ToString(), out consoleColor);
            Console.BackgroundColor = consoleColor;
            _colorsUsed++;
        }
    }

    public class ConsoleSize
    {
        public static void SetWindowWidth(int width)
        {
            if (width > Console.LargestWindowWidth) width = Console.LargestWindowWidth;
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
        }
        public static void SetWindowHeight(int height)
        {
            if (height > Console.LargestWindowHeight) height = Console.LargestWindowHeight;
            bool worked = false;
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
        public static void SetWindowSize(int width, int height)
        {
            SetWindowWidth(width);
            SetWindowHeight(height);
        }
    }

    public class ConsoleRead
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct CHAR_INFO
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] charData;
            public short attributes;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool ReadConsoleOutput(IntPtr hConsoleOutput, IntPtr lpBuffer, Coord dwBufferSize, Coord dwBufferCoord, ref SmallRect lpReadRegion);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        public static IEnumerable<string> ReadFromBuffer(int x, int y, int width, int height)
        {
            return ReadFromBuffer((short) x, (short) y, (short) width, (short) height);
        }
        public static IEnumerable<string> ReadFromBuffer(short x, short y, short width, short height)
        {
            IntPtr buffer = Marshal.AllocHGlobal(width * height * Marshal.SizeOf(typeof(CHAR_INFO)));
            if (buffer == null) throw new OutOfMemoryException();

            try
            {
                var coord = new Coord();
                var rc = new SmallRect
                {
                    Left = x,
                    Top = y,
                    Right = (short) (x + width - 1),
                    Bottom = (short) (y + height - 1)
                };

                var size = new Coord {X = width, Y = height};

                const int STD_OUTPUT_HANDLE = -11;
                if (!ReadConsoleOutput(GetStdHandle(STD_OUTPUT_HANDLE), buffer, size, coord, ref rc))
                {
                    // 'Not enough storage is available to process this command' may be raised for buffer size > 64K (see ReadConsoleOutput doc.)
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }

                IntPtr ptr = buffer;
                for (int h = 0; h < height; h++)
                {
                    var sb = new StringBuilder();
                    for (int w = 0; w < width; w++)
                    {
                        var ci = (CHAR_INFO)Marshal.PtrToStructure(ptr, typeof(CHAR_INFO));
                        char[] chars = Console.OutputEncoding.GetChars(ci.charData);
                        sb.Append(chars[0]);
                        ptr += Marshal.SizeOf(typeof(CHAR_INFO));
                    }
                    yield return sb.ToString();
                }
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }

        public static string Read(int x, int y, int length)
        {
            if (x + (y*Console.BufferWidth) + length > Console.BufferWidth*Console.BufferHeight)
                throw new IndexOutOfRangeException("Read ran past the end of the buffer (i.e. went too far)");
            XYvalidity(x, y);

            string output = "";
            while (x + length > Console.BufferWidth)
            {
                output += ReadFromBuffer(x, y, Console.BufferWidth - x, 1).First();
                length -= Console.BufferWidth - x;
                y += 1;
                x = 0;
            }
            if (length != 0)
                output += ReadFromBuffer(x, y, length, 1).First();
            return output;
        }
        public static char ReadChar(int x, int y)
        {
            XYvalidity(x,y);
            return ReadFromBuffer(x, y, 1, 1).First()[0];
        }
        public static string ReadLine(int line)
        {
            XYvalidity(0, line);
            return ReadFromBuffer(0, line, Console.BufferWidth, 1).First();
        }

        public static void XYvalidity(int x, int y)
        {
            if (x < 0 || y < 0 || x >= Console.BufferWidth || y >= Console.BufferHeight)
                throw new IndexOutOfRangeException("Tried to read outside of the buffer: " +
                    (x < 0 ? "X was below 0" :
                    y < 0 ? "Y was below 0" :
                    x >= Console.BufferWidth ? "X was above the buffer's width" :
                    y >= Console.BufferHeight ? "Y was above the buffer's height" : ""));
        }
    }
}

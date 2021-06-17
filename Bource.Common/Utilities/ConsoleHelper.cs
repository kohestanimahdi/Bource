using System;
using System.Linq;
using System.Text;

namespace Bource.Common.Utilities
{
    public static class ConsoleHelper
    {
        private const char _block = '■';
        private const string _back = "\b\b\b\b\b\b\b\b\b\b\b\b\b\b\b\b\b";
        private const string _twirl = "-\\|/";

        public static void WriteProgressBar(int percent, bool update = false)
        {
            if (update)
                Console.Write(_back);
            Console.Write("[");
            var p = (int)((percent / 10f) + .5f);
            for (var i = 0; i < 10; ++i)
            {
                if (i >= p)
                    Console.Write(' ');
                else
                    Console.Write(_block);
            }
            Console.Write("] {0,3:##0}%", percent);
        }

        public static void WriteProgress(int progress, bool update = false)
        {
            if (update)
                Console.Write("\b");
            Console.Write(_twirl[progress % _twirl.Length]);
        }

        private static int tableWidth = 118;

        private static void PrintLine()
            => System.Console.WriteLine(new string('-', tableWidth));

        private static void PrintRow(params string[] columns)
        {
            int width = (tableWidth - columns.Length) / columns.Length;
            StringBuilder row = new("|");

            foreach (string column in columns)
                row.Append($"{AlignCentre(column, width)}|");

            System.Console.WriteLine(row.ToString());
        }

        private static string AlignCentre(string text, int width)
        {
            text = text.Length > width ? text.Substring(0, width - 3) + "..." : text;

            if (string.IsNullOrEmpty(text))
            {
                return new string(' ', width);
            }
            else
            {
                return text.PadRight(width - (width - text.Length) / 2).PadLeft(width);
            }
        }

        public static void PrintTable(int column, params string[] items)
        {
            if (items.Length % column != 0)
                throw new ArgumentException("");

            int p = 0;
            for (int i = 0; i < items.Length; i += column)
            {
                PrintRow(items.Skip(p).Take(column).ToArray());
                PrintLine();
                p += column;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2024.Utils
{
    internal static class MatrixUtils
    {
        private static readonly string COLOR_MAGENTA = Console.IsOutputRedirected ? "" : "\x1b[95m";
        private static readonly string COLOR_NORMAL = Console.IsOutputRedirected ? "" : "\x1b[39m";

        public static StringBuilder MatrixToStringBuilder<T>(T[,] matrix) {

            StringBuilder sb = new();
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    sb.Append(matrix[i, j]);
                }
                sb.Append('\n');
            }

            return sb;

        }
        public static string MatrixToString<T>(T[,] matrix)
        {
            return MatrixToStringBuilder(matrix).ToString();
        }

        public static char[,] StringToCharMatrix(string input) {
            return StringRowsToMatrix(input.Split("\r\n"));
        }
        public static char[,] StringRowsToMatrix(string[] rows)
        {
            char[,] matrix = new char[rows.Length, rows[0].Length];

            int i = 0;
            foreach (string row in rows) {

                for (int j = 0; j < row.Length; j++)
                {
                    matrix[i, j] = row[j];
                }
                i++;
            }

            return matrix;
        }

        public static void Print<T>(T[,] matrix, IList<char>? colouredChars = null) {

            string str = MatrixToString(matrix);
            if(colouredChars is not null) {
                foreach (char c in colouredChars) { 
                    str = str.Replace(c.ToString(), COLOR_MAGENTA+c+COLOR_NORMAL);
                }
            }
            Console.WriteLine(str);
        }


        public static void Animate<T>(T[,] matrix, int millisecondsSleep = 500, IList<char>? colouredChars = null)
        {

            Console.Clear();
            Print(matrix, colouredChars);
            Thread.Sleep(millisecondsSleep);

        }

        public static void Animate<T>(T[,] matrix, (int X, int Y) currentPostion, char subject, int millisecondsSleep = 500)
        {

            char[,] tmp = new char[matrix.GetLength(0), matrix.GetLength(1)];
            Array.Copy(matrix, tmp, matrix.Length);
            tmp[currentPostion.X, currentPostion.Y] = subject;
            Animate(tmp, millisecondsSleep, [subject]);

        }


    }
}

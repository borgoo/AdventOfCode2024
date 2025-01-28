using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2024.Utils
{
    internal static class MatrixUtils
    {
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

        public static void Print<T>(T[,] matrix) {

            Console.WriteLine(MatrixToString(matrix));
        }


        public static void Animate<T>(T[,] matrix, int millisecondsSleep = 500)
        {

            Console.Clear();
            Print(matrix);
            Thread.Sleep(millisecondsSleep);

        }


    }
}

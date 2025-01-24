using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2024.Day4
{
    internal class Day4 : Day
    {
        protected override long SolveA(string input)
        {
            return FindXMAS(input);
        }

        protected override long SolveB(string input)
        {
            string[] mask = new string[] { 
                "M S",
                " A ",
                "M S"
            };

            SubMatrix subMatrix = new(mask);
            return FindSubMatrix(input, subMatrix);
        }

        private class SubMatrix(string[] mat)
        {
            public readonly char Empty = ' ';
            public string[] Mat { get; private set; } = mat;

            public void Rotate() {

                List<string> tmp = [];

                for (int i = 0; i < Mat.Length; i++)
                {
                    StringBuilder sb = new();
                    for (int j = Mat.Length - 1; j >= 0; j--)
                    {
                        sb.Append(Mat[j][i]);
                    }
                    tmp.Add(sb.ToString());

                }

                string[] result = tmp.ToArray();

                Mat = result;
                
            }


        }

        private static int FindXMAS(string input)
        {

            int ans = 0;

            string[] mat = input.Split('\n');


            for (int i = 0; i < mat.Length; i++)
            {

                for (int j = 0; j < mat[i].Length; j++)
                {

                    if (mat[i][j] != 'X') continue;

                    try
                    {
                        if (mat[i][j + 1] == 'M' && mat[i][j + 2] == 'A' && mat[i][j + 3] == 'S') { ans++; }

                    }
                    catch (IndexOutOfRangeException) { }
                    try
                    {
                        if (mat[i][j - 1] == 'M' && mat[i][j - 2] == 'A' && mat[i][j - 3] == 'S') { ans++; }

                    }
                    catch (IndexOutOfRangeException) { }
                    try
                    {
                        if (mat[i + 1][j + 1] == 'M' && mat[i + 2][j + 2] == 'A' && mat[i + 3][j + 3] == 'S') { ans++; }

                    }
                    catch (IndexOutOfRangeException) { }
                    try
                    {
                        if (mat[i + 1][j] == 'M' && mat[i + 2][j] == 'A' && mat[i + 3][j] == 'S') { ans++; }

                    }
                    catch (IndexOutOfRangeException) { }
                    try
                    {
                        if (mat[i - 1][j] == 'M' && mat[i - 2][j] == 'A' && mat[i - 3][j] == 'S') { ans++; }

                    }
                    catch (IndexOutOfRangeException) { }
                    try
                    {
                        if (mat[i - 1][j - 1] == 'M' && mat[i - 2][j - 2] == 'A' && mat[i - 3][j - 3] == 'S') { ans++; }

                    }
                    catch (IndexOutOfRangeException) { }
                    try
                    {
                        if (mat[i - 1][j + 1] == 'M' && mat[i - 2][j + 2] == 'A' && mat[i - 3][j + 3] == 'S') { ans++; }

                    }
                    catch (IndexOutOfRangeException) { }
                    try
                    {
                        if (mat[i + 1][j - 1] == 'M' && mat[i + 2][j - 2] == 'A' && mat[i + 3][j - 3] == 'S') { ans++; }

                    }
                    catch (IndexOutOfRangeException) { }




                }
            }




            return ans;


        }

        private static int FindSubMatrix(string input, SubMatrix subMatrix) {

            int ans = 0;

            string[] mat = input.Split('\n');


            for (int i = 0; i < mat.Length; i++)
            {

                for (int j = 0; j < mat[i].Length; j++) {

                    for (int k = 0; k < 4; k++) {

                        ans += Check(mat, subMatrix, i, j);
                        subMatrix.Rotate();
                    }
                    subMatrix.Rotate();
                  

                }

            }

            return ans;    
        }

        private static int Check(string[] mat, SubMatrix subMatrix, int i, int j) {


            for (int k = 0; k < subMatrix.Mat.Length; k++) {

                for (int w = 0; w < subMatrix.Mat[k].Length; w++) {

                    if (subMatrix.Mat[k][w] == subMatrix.Empty) { continue; }

                    try
                    {
                        if ( subMatrix.Mat[k][w] != mat[i + k][j + w] )
                        {
                            return 0;
                        }

                    }
                    catch (IndexOutOfRangeException) {
                        return 0;
                    }
                                       
                }

            }

            return 1;
           

        }
    }
}

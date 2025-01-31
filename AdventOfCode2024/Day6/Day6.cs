using AdventOfCode2024.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2024.Day6
{
    internal class LoopException : Exception
    {

    }

    internal class Day6 : Day
    {
        const char DEFAULT_GUARD_ORIENTATION = '^';

        private static readonly Dictionary<char, (int Dx, int Dy)> increments = new()
        {
            { '^', (-1, 0) },
            { '>', (0, 1) },
            { '<', (0, -1) },
            { 'v', (1, 0) },
        };

        private static readonly Dictionary<char, char> rotations = new()
        {
            { '^', '>' },
            { '>', 'v' },
            { '<', '^' },
            { 'v', '<' },
        };

 


        protected override object SolveA(string input)
        {
            char[,] matrix = FormatInput(input);
            var (X, Y, Orientations) = GetGuardStartingCondition(matrix);
            var set = MoveTheGuard(matrix, (X,Y), Orientations);
            return set.Count;
        }

        protected override object SolveB(string input)
        {
            char[,] matrix = FormatInput(input);
            var (X, Y, Orientations) = GetGuardStartingCondition(matrix);
            var set = MoveTheGuard(matrix, (X, Y), Orientations);
            int ans = 0;

            set.Remove((X,Y));
            (int X, int Y)[] possibleObstaclePositions = [.. set];

            for (int i = 0; i < possibleObstaclePositions.Length; i++) {

             
                if (i != 0) {
                    matrix[possibleObstaclePositions[i-1].X, possibleObstaclePositions[i - 1].Y] = '.';
                }

                matrix[possibleObstaclePositions[i].X, possibleObstaclePositions[i].Y] = '#';


                try
                {
                    MoveTheGuard(matrix, (X, Y), Orientations);
                }
                catch (LoopException)
                {
                    ans++;
                }

            }


            return ans;
        }

        private static char[,] FormatInput(string input) {
            string[] rows = input.Split(new string("\r\n"));
            char[,] matrix = new char[rows.Length, rows[0].Length];

            for (int i = 0; i < rows.Length; i++) { 
                
                for(int j = 0; j < rows[i].Length; j++) { matrix[i, j] = (char)rows[i][j]; }
            }

            return matrix;

        }

        private static (int X, int Y, char Orientation) GetGuardStartingCondition(char[,] matrix) {

            char guardOrientation = DEFAULT_GUARD_ORIENTATION;

            for (int i = 0; i < matrix.GetLength(0); i++) {

                for (int j = 0; j < matrix.GetLength(1); j++) {

                    if (matrix[i, j] == guardOrientation) return (X: i, Y: j, guardOrientation);
                    
                }
            }

            throw new NotFoundException();


            
        }

        private static HashSet<(int X, int Y)> MoveTheGuard(char[,] matrix, (int X, int Y) guardPosition, char guardOrientation)
        {

            HashSet<(int X, int Y)> set = [];
            HashSet<(int X, int Y, char Orientation)> history = [];


            set.Add(guardPosition);
            history.Add( (guardPosition.X, guardPosition.Y, guardOrientation) );


            while (true)
            {

                (int X, int Y) guardNextPosition = (guardPosition.X + increments[guardOrientation].Dx, guardPosition.Y + increments[guardOrientation].Dy);
                char val;
                try
                {
                    val = matrix[guardNextPosition.X,guardNextPosition.Y];
                }
                catch (IndexOutOfRangeException)
                {
                    break;
                }

                if (val != '#')
                {
                    guardPosition = guardNextPosition;
                    set.Add(guardPosition);
                }
                else
                {
                    guardOrientation = rotations[guardOrientation];
                }
                
                if (history.Count > 1 && history.Contains((guardPosition.X, guardPosition.Y, guardOrientation))) throw new LoopException();
                history.Add((guardPosition.X, guardPosition.Y, guardOrientation));

                

            }


            return set;
        }

    }
}

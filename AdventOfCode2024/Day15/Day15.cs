using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2024.Day15
{
    internal class Day15 : Day
    {

        const char ROBOT_CHAR = '@';
        const char OBSTACLE_CHAR = 'O';
        const char WALL_CHAR = '#';
        const char EMPTY_SPACE_CHAR = '.';

        private static readonly (int MulX, int MulY) gpsMul = (100, 1);

        private static readonly Dictionary<char, (int Dx, int Dy)> increments = new()
        {
            { '^', (-1,0) },
            { '>', (0,1) },
            { '<', (0,-1) },
            { 'v', (1,0) },
        };

        protected override long SolveA(string input)
        {
            var (matrix, moves, robotPosition, obstacles, walls) = HandleInput(input);
            ApplyMoves(matrix, moves, robotPosition, obstacles, walls);

            long gpsSum = 0;
            foreach (var obstacle in obstacles) {

                gpsSum += CalcGPS(obstacle);

            }
            return gpsSum;
        }

        protected override long SolveB(string input)
        {
            return -1;
        }

        private static (char[,] matrix, string moves, (int X, int Y) robotPosition, HashSet<(int X, int Y)> obstacles, HashSet<(int X, int Y)> walls) HandleInput(string input)
        {

            HashSet<(int X, int Y)> obstacles = [];
            HashSet<(int X, int Y)> walls = [];
            (int X, int Y)? robotPosition = null;

            int tmp = input.LastIndexOf('#');
            string map = input[..(tmp + 1)];
            string moves = input[(tmp + 1)..].Replace("\r\n", String.Empty);

            string[] rows = map.Split("\r\n");
            char[,] matrix = new char[rows.Length, rows[0].Length];


            for (int x = 0; x < rows.Length; x++)
            {
                string row = rows[x];
                for (int y = 0; y < row.Length; y++)
                {
                    char c = row[y];
                    matrix[x, y] = c;
                    if (c == ROBOT_CHAR) robotPosition = (x, y);
                    else if (c == OBSTACLE_CHAR) obstacles.Add((x,y));
                    else if (c == WALL_CHAR) walls.Add((x,y));

                }
            }

            _ = robotPosition ?? throw new NullReferenceException();


            return (matrix, moves, robotPosition.Value, obstacles, walls);


        }

        private static long CalcGPS((int X, int Y) pos)
        {

            return (long)(pos.X * gpsMul.MulX) + (long)(pos.Y * gpsMul.MulY);
        }

        private static void ApplyMoves(char[,] matrix, string moves, (int X, int Y) robotStartingPosition, HashSet<(int X, int Y)> obstacles, HashSet<(int X, int Y)> walls) {

            HashSet<(int X, int Y, char dir)> notPermittedMoves = [];

            (int X, int Y) robotCurrentPosition = robotStartingPosition;
            foreach (char move in moves) {

                (int X, int Y) nextPosition = (robotCurrentPosition.X + increments[move].Dx, robotCurrentPosition.Y + increments[move].Dy);

                if (walls.Contains(nextPosition)) continue;
                if (!obstacles.Contains(nextPosition)) {
                    matrix[robotCurrentPosition.X, robotCurrentPosition.Y] = EMPTY_SPACE_CHAR;
                    robotCurrentPosition = nextPosition;
                    matrix[robotCurrentPosition.X, robotCurrentPosition.Y] = ROBOT_CHAR;
                    continue;
                };

                //TRY TO PUSH
                if (notPermittedMoves.Contains((nextPosition.X, nextPosition.Y, move))) continue;
                //CHECK THERE IS A FREE POSITION

         
                (int X, int Y) tmpPostion = (nextPosition.X, nextPosition.Y);
                do {

                    tmpPostion = (tmpPostion.X + increments[move].Dx, tmpPostion.Y + increments[move].Dy);

                } while (matrix[tmpPostion.X, tmpPostion.Y] == OBSTACLE_CHAR);

                if (matrix[tmpPostion.X, tmpPostion.Y] == WALL_CHAR)
                {
                    notPermittedMoves.Add((nextPosition.X, nextPosition.Y, move));
                    continue;
                }

                obstacles.Remove((nextPosition.X, nextPosition.Y));
                obstacles.Add((tmpPostion.X, tmpPostion.Y));
                matrix[tmpPostion.X, tmpPostion.Y] = OBSTACLE_CHAR;
                matrix[nextPosition.X, nextPosition.Y] = ROBOT_CHAR;
                matrix[robotCurrentPosition.X, robotCurrentPosition.Y] = EMPTY_SPACE_CHAR;

                robotCurrentPosition = nextPosition;
                notPermittedMoves.Clear();

            }
        }

        



    }
}

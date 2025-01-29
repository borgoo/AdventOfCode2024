

using AdventOfCode2024.Utils;
using Serilog;
using System.Diagnostics;
using System.Text;

namespace AdventOfCode2024.Day16
{
    internal class Day16 : Day
    {
        private static bool _debugActive = false; 

        const int ROTATION_COST = 1000;
        const int MOVING_COST = 1;
        const char WALL_CHAR = '#';
        const char DEER_CHAR = 'S';
        const char END_CHAR = 'E';
        const char DEER_STARTING_DIRECTION = '>';

        private static readonly Dictionary<char, char> rotations = new(){

            {'^','>'},
            {'>','v'},
            {'v','<'},
            {'<','^'}

        };

        private static readonly Dictionary<char, (int Dx, int Dy)> increments = new() {
            {'^', (-1,0)},
            {'>',(0,1)},
            {'v',(1,0)},
            {'<',(0,-1)}
        };

        protected override long SolveA(string input)
        {
            long? min = null;
            var (matrix, deerStartingPosition, endPosition) = HandleInput(input);

            Dictionary<((int X, int Y), char) , long> bestCost = [];
         
            foreach (char direction in rotations.Keys)
            {
                int startingCost = CalculateRotationCost(DEER_STARTING_DIRECTION, direction);
                bestCost.Add((deerStartingPosition, direction), startingCost);
                Dfs(matrix, bestCost, [], startingCost, deerStartingPosition, endPosition, direction, ref min);
            }

            return min ?? throw new NullReferenceException("No valid path found.");

        }

        protected override long SolveB(string input)
        {
            return -1;
        }

        private static (char[,] matrix, (int X, int Y) deerStartingPosition, (int X, int Y) endPosition) HandleInput(string input)
        {
            string[] rows = input.Split("\r\n");

            char[,] matrix = new char[rows.Length, rows[0].Length];

            (int X, int Y)? deerStartingPosition = null;
            (int X, int Y)? endPosition = null;
            int i = 0;
            foreach (string row in rows)
            {

                for (int j = 0; j < row.Length; j++)
                {
                    matrix[i, j] = row[j];

                    if (row[j] == DEER_CHAR)
                    {
                        deerStartingPosition = (i, j);
                    }
                    else if (row[j] == END_CHAR) {
                        endPosition = (i, j);
                    }
                }
                i++;
            }

            _ = deerStartingPosition ?? throw new NullReferenceException();
            _ = endPosition ?? throw new NullReferenceException();

            return (matrix, deerStartingPosition.Value, endPosition.Value);
        }


        private static void Dfs(char[,] matrix, Dictionary<((int X, int Y),char), long> bestCost, HashSet<(int X, int Y)> seen, long currentCost, (int X, int Y) deerPosition, (int X, int Y) endPosition, char currentDirection, ref long? min ) {

            if (_debugActive) Debug(matrix, deerPosition, currentDirection);

            if (min.HasValue && min.Value <= currentCost)
            {
                return;
            }

            if (deerPosition == endPosition) {

                min = currentCost;
                return;

            }         

            seen.Add(deerPosition);

            foreach (char direction in rotations.Keys) {

                (int X, int Y) adiacentPosition = (deerPosition.X + increments[direction].Dx, deerPosition.Y + increments[direction].Dy);
                if (matrix[adiacentPosition.X, adiacentPosition.Y] == WALL_CHAR) continue;
                if (seen.Contains(adiacentPosition)) continue;

                int tmpCost = CalculateCostToReachAdiacentPosition(deerPosition, adiacentPosition, currentDirection, direction);
                long prevOfCurrentCost = (long)tmpCost + currentCost;
                if (min.HasValue && min.Value <= prevOfCurrentCost) continue;


                if (bestCost.ContainsKey((adiacentPosition, direction))) {

                    if (bestCost[(adiacentPosition, direction)] <= prevOfCurrentCost) continue;
                    else bestCost[(adiacentPosition, direction)] = prevOfCurrentCost;
                }
                else {
                    bestCost.Add((adiacentPosition, direction), prevOfCurrentCost);
                }

                HashSet<(int X, int Y)> currentPathSeen = new(seen);
                Dfs(matrix, bestCost, currentPathSeen, prevOfCurrentCost, adiacentPosition, endPosition, direction, ref min);
            }

            return;
        }

        private static int CalculateRotationCost(char currentOrientation, char destPositionOrientation) {

            const int CLOCKWISE_TO_COUNTERCLOCKWISE = 1;
            int numOfRotationsClockWise = 0;

            while (currentOrientation != destPositionOrientation)
            {

                currentOrientation = rotations[currentOrientation];
                numOfRotationsClockWise++;

            }

            if (numOfRotationsClockWise > 2) numOfRotationsClockWise = CLOCKWISE_TO_COUNTERCLOCKWISE;

            return (numOfRotationsClockWise * ROTATION_COST);
        }

        private static int CalculateCostToReachAdiacentPosition((int X, int Y) currentPos, (int X, int Y) destPos, char currentOrientation, char destPositionOrientation) {

            int movingCost = 1 * MOVING_COST;
            int rotationCost = CalculateRotationCost(currentOrientation, destPositionOrientation);           

            return movingCost + rotationCost;

        }

        private static void Debug(char[,] matrix, (int X, int Y) deerPosition, char direction)
        {
            char[,] tmp = new char[matrix.GetLength(0), matrix.GetLength(1)];
            Array.Copy(matrix, tmp, matrix.Length);
            tmp[deerPosition.X, deerPosition.Y] = direction;
            MatrixUtils.Animate(tmp, 300, [.. rotations.Keys]);

        }
    }
}

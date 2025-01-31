using AdventOfCode2024.Exceptions;
using AdventOfCode2024.Utils;

namespace AdventOfCode2024.Day15
{
    internal class Day15 : Day
    {
        private static readonly bool _debugActive = false;

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

        protected override object SolveA(string input)
        {
            var (matrix, moves, robotPosition, obstacles, walls) = HandleInput(input);
            ApplyMoves(matrix, moves, robotPosition, obstacles, walls);

            long gpsSum = 0;
            foreach (var obstacle in obstacles)
            {

                gpsSum += CalcGPS(obstacle);

            }
            return gpsSum;
        }

        protected override object SolveB(string input)
        {
            OrderedDictionary<char, string> mapping = new()
            {
                { EMPTY_SPACE_CHAR, ".." },
                { ROBOT_CHAR, "@." },
                { WALL_CHAR, "##" },
                { OBSTACLE_CHAR, "[]" }

            };

            var (robotStartingPosition, matrix, moves) = OtherApproach.HandleInput(input, mapping);

            OtherApproach.ApplyMoves(matrix, moves, robotStartingPosition);
            long gpsSum = OtherApproach.GPSSumFromMatrix(matrix);

            return gpsSum;
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
                    else if (c == OBSTACLE_CHAR) obstacles.Add((x, y));
                    else if (c == WALL_CHAR) walls.Add((x, y));

                }
            }

            _ = robotPosition ?? throw new NullReferenceException();


            return (matrix, moves, robotPosition.Value, obstacles, walls);


        }

        private static long CalcGPS((int X, int Y) pos)
        {

            return (long)(pos.X * gpsMul.MulX) + (long)(pos.Y * gpsMul.MulY);
        }

        private static void ApplyMoves(char[,] matrix, string moves, (int X, int Y) robotStartingPosition, HashSet<(int X, int Y)> obstacles, HashSet<(int X, int Y)> walls)
        {

            HashSet<(int X, int Y, char dir)> notPermittedMoves = [];

            (int X, int Y) robotCurrentPosition = robotStartingPosition;
            foreach (char move in moves)
            {

                (int X, int Y) nextPosition = (robotCurrentPosition.X + increments[move].Dx, robotCurrentPosition.Y + increments[move].Dy);

                if (walls.Contains(nextPosition)) continue;
                if (!obstacles.Contains(nextPosition))
                {
                    matrix[robotCurrentPosition.X, robotCurrentPosition.Y] = EMPTY_SPACE_CHAR;
                    robotCurrentPosition = nextPosition;
                    matrix[robotCurrentPosition.X, robotCurrentPosition.Y] = ROBOT_CHAR;
                    continue;
                };

                if (notPermittedMoves.Contains((nextPosition.X, nextPosition.Y, move))) continue;


                (int X, int Y) tmpPostion = (nextPosition.X, nextPosition.Y);
                do
                {

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

        private static class OtherApproach {

            const char OBSTACLE_OPEN = '[';
            const char OBSTACLE_CLOSE = ']';
            private class Obstacle((int X, int Y) left, (int X, int Y) right) {

                public (int X, int Y) Left { get; set; } = left;
                public (int X, int Y) Right { get; set; } = right;
            }

            private class ImmovableException : Exception { 
           
            }


            public static ((int X, int Y) robotStartingPosition, char[,] matrix, string moves) HandleInput(string input, OrderedDictionary<char, string> mapping)
            {

                (int X, int Y)? robotPosition = null;

                int end = input.LastIndexOf('#');
                string map = input[..(end + 1)];
                string moves = input[(end + 1)..].Replace("\r\n", String.Empty);

                string[] rows = map.Split("\r\n");

                char[,] matrix = new char[rows.Length, rows[0].Length];
                string[] tmp = new string[rows.Length];

                for (int i = 0; i < rows.Length; i++)
                {

                    string row = rows[i];
                    tmp[i] = row;
                    foreach (var pair in mapping)
                    {
                        tmp[i] = tmp[i].Replace(pair.Key.ToString(), pair.Value);
                    }
                    int robotJ = tmp[i].IndexOf(ROBOT_CHAR);
                    if (robotJ != -1) robotPosition = (i, robotJ);


                }

                _ = robotPosition ?? throw new NotFoundException();


                return (robotPosition.Value, MatrixUtils.StringRowsToMatrix(tmp), moves);

            }

            public static void ApplyMoves(char[,] matrix, string moves, (int X, int Y) robotStartingPosition) {

                HashSet<((int X, int Y), char dir)> notPermittedMoves = [];
                (int X, int Y) currentRobotPosition = robotStartingPosition;

                foreach (char move in moves) {

                    if (_debugActive) Debug(matrix);

                    (int X, int Y) moveDestination = (currentRobotPosition.X + increments[move].Dx, currentRobotPosition.Y + increments[move].Dy);
                    char moveDestinationChar = matrix[moveDestination.X, moveDestination.Y];

                    if (notPermittedMoves.Contains((moveDestination, move))) continue;

                    switch (moveDestinationChar) {
                        case EMPTY_SPACE_CHAR:
                            matrix[currentRobotPosition.X, currentRobotPosition.Y] = EMPTY_SPACE_CHAR;
                            matrix[moveDestination.X, moveDestination.Y] = ROBOT_CHAR;
                            currentRobotPosition = moveDestination;
                            break;
                        case WALL_CHAR:
                            break; ;
                        case OBSTACLE_OPEN:
                        case OBSTACLE_CLOSE:

                            Dictionary<(int X, int Y), char>? operations = [];
                            HashSet<(int X, int Y)> seen = [];
                            try
                            {
                                PushObstacles(matrix, move, moveDestination, moveDestinationChar, seen, operations);
                            }
                            catch (ImmovableException)
                            {
                                Rollback(matrix, notPermittedMoves, move, moveDestination, operations);
                                continue;                            
                            }

                            Commit(matrix, operations);
                            matrix[currentRobotPosition.X, currentRobotPosition.Y] = EMPTY_SPACE_CHAR;
                            matrix[moveDestination.X, moveDestination.Y] = ROBOT_CHAR;
                            currentRobotPosition = moveDestination;
                            notPermittedMoves.Clear();
                            break;


                        default:
                            throw new NotImplementedException();
                    }

       

                }


            }

            private static void PushObstacles(char[,] matrix, char move, (int X, int Y) currentPosition, char currentPostionChar, HashSet<(int X, int Y)> seen, Dictionary<(int X, int Y), char> operations) {

                if (seen.Contains(currentPosition)) return;
                if (currentPostionChar == EMPTY_SPACE_CHAR) return;
                if(currentPostionChar == WALL_CHAR) throw new ImmovableException();

                (int X, int Y) siblingPosition = CalculateObstacleSiblingPostion(matrix, currentPosition);
                char siblingChar = matrix[siblingPosition.X, siblingPosition.Y];

                (int X, int Y) destPosition = (currentPosition.X + increments[move].Dx, currentPosition.Y + increments[move].Dy);
                char destPostionChar = matrix[destPosition.X, destPosition.Y];
                (int X, int Y) siblingDestPostion = (siblingPosition.X + increments[move].Dx, siblingPosition.Y + increments[move].Dy);
                char siblingDestPositionChar = matrix[siblingDestPostion.X, siblingDestPostion.Y];
                seen.Add(siblingPosition);

                if (!operations.ContainsKey(currentPosition)) {
                    operations.Add(currentPosition, EMPTY_SPACE_CHAR);            
                }  
                if (!operations.ContainsKey(siblingPosition)) {
                    operations.Add(siblingPosition, EMPTY_SPACE_CHAR);            
                }

                if (!operations.ContainsKey(destPosition))
                {
                    operations.Add(destPosition, currentPostionChar);
                }
                else {
                    if (operations[destPosition] == EMPTY_SPACE_CHAR) {
                        operations[destPosition] = currentPostionChar;
                    }
                }
                if (!operations.ContainsKey(siblingDestPostion))
                {
                    operations.Add(siblingDestPostion, siblingChar);
                }
                else {
                    if (operations[siblingDestPostion] == EMPTY_SPACE_CHAR)
                    {
                        operations[siblingDestPostion] = siblingChar;
                    }
                }

                PushObstacles(matrix, move, destPosition, destPostionChar, seen, operations );
                PushObstacles(matrix, move, siblingDestPostion, siblingDestPositionChar, seen, operations );

            }

            private static void Commit(char[,] matrix, Dictionary<(int X, int Y), char>? operations) {

                if (operations is null) return;
                foreach (var operation in operations)
                {
                    matrix[operation.Key.X, operation.Key.Y] = operation.Value;
                }


            }

            private static (int X, int Y) CalculateObstacleSiblingPostion(char[,] matrix, (int X, int Y) obstaclePostion) {

                char siblingPositionKey = matrix[obstaclePostion.X, obstaclePostion.Y] == OBSTACLE_OPEN ? '>' : '<';
                (int X, int Y) siblingPosition = (obstaclePostion.X + increments[siblingPositionKey].Dx, obstaclePostion.Y + increments[siblingPositionKey].Dy);

                return siblingPosition;
            }

            private static void Rollback(
                char[,]  matrix,
                HashSet<((int X, int Y), char dir)> notPermittedMoves,
                char move,
                (int X, int Y) originPosition,
                Dictionary<(int X, int Y), char> operations
            ) {

                operations = null;
                (int X, int Y) siblingPosition = CalculateObstacleSiblingPostion(matrix, originPosition);
                notPermittedMoves.Add((originPosition, move));
                notPermittedMoves.Add((siblingPosition, move));

            }

            public static long GPSSumFromMatrix(char[,] matrix) {

                long sum = 0;
                for (int i = 0; i < matrix.GetLength(0); i++) {

                    for (int j = 0; j < matrix.GetLength(1); j++) {

                        if (matrix[i,j] == OBSTACLE_OPEN) sum += CalcGPS((i,j));
                    }
                }

                return sum;
            
            }


            private static void Debug(char[,] matrix) {
                MatrixUtils.Animate(matrix, 350);
            }






        }



    }
}

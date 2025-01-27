
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
          
            const int elementHeight = 1;
            const int elementWidth = 1;
            char[,] robotMask = new char[elementHeight, elementWidth] {
                {  ROBOT_CHAR}
            };
            char[,] obstacleMask = new char[elementHeight, elementWidth] {
                { OBSTACLE_CHAR }
            }; 
            char[,] wallMask = new char[elementHeight, elementWidth] {
                { WALL_CHAR }
            }; 
            char[,] emptySpaceMask = new char[elementHeight, elementWidth] {
                { EMPTY_SPACE_CHAR }
            };

            if (robotMask.GetLength(0) != obstacleMask.GetLength(0) || obstacleMask.GetLength(0) != wallMask.GetLength(0) || wallMask.GetLength(0) != emptySpaceMask.GetLength(0)) throw new Exception("All masks must have the same dimensions.");
            if (robotMask.GetLength(1) != obstacleMask.GetLength(1) || obstacleMask.GetLength(1) != wallMask.GetLength(1) || wallMask.GetLength(1) != emptySpaceMask.GetLength(1)) throw new Exception("All masks must have the same dimensions.");

            var (matrix, moves, robotPosition, obstacles, walls) = HandleInput(input, robotMask, obstacleMask, wallMask, emptySpaceMask, elementHeight, elementWidth) ;
            ApplyMoves(matrix, moves, robotPosition, obstacles, walls);

            long gpsSum = 0;
            foreach (var obstacle in obstacles) {

                gpsSum += CalcGPS(obstacle);

            }
            return gpsSum;
        }

        protected override long SolveB(string input)
        {

            const int elementHeight = 1;
            const int elementWidth = 2;
            char[,] robotMask = new char[elementHeight, elementWidth] {
                {  ROBOT_CHAR, EMPTY_SPACE_CHAR}
            };
            char[,] obstacleMask = new char[elementHeight, elementWidth] {
                { OBSTACLE_CHAR, OBSTACLE_CHAR }
            };
            char[,] wallMask = new char[elementHeight, elementWidth] {
                { WALL_CHAR, WALL_CHAR}
            };
            char[,] emptySpaceMask = new char[elementHeight, elementWidth] {
                { EMPTY_SPACE_CHAR, EMPTY_SPACE_CHAR }
            };

            if (robotMask.GetLength(0) != obstacleMask.GetLength(0) || obstacleMask.GetLength(0) != wallMask.GetLength(0) || wallMask.GetLength(0) != emptySpaceMask.GetLength(0)) throw new Exception("All masks must have the same dimensions.");
            if (robotMask.GetLength(1) != obstacleMask.GetLength(1) || obstacleMask.GetLength(1) != wallMask.GetLength(1) || wallMask.GetLength(1) != emptySpaceMask.GetLength(1)) throw new Exception("All masks must have the same dimensions.");

            var (matrix, moves, robotPosition, obstacles, walls) = HandleInput(input, robotMask, obstacleMask, wallMask, emptySpaceMask, elementHeight, elementWidth);
            ApplyMoves(matrix, moves, robotPosition, obstacles, walls);

            long gpsSum = 0;
            foreach (var obstacle in obstacles)
            {

                gpsSum += CalcGPS(obstacle);

            }
            return gpsSum;
        }

        private static void MapInputIntoResultingMatrix(
            char[,] matrix,
            int resultCurrentI,
            int resultCurrentJ,
            ref (int X, int Y)? robotPosition,
            char c,
            char[,] mask,
            int elementHeight,
            int elementWidth,
            HashSet<(int X, int Y)> obstacles,
            HashSet<(int X, int Y)> walls
        ) {


            for (int k = 0; k < elementHeight; k++) {

                for (int w = 0; w < elementWidth; w++)
                {
                    int destinationI = resultCurrentI + k;
                    int destinationJ = resultCurrentJ + w;
                    (int X, int Y) destinationCoordinates = (destinationI, destinationJ);
                    char resultVal = mask[k, w];
                    matrix[destinationI, destinationJ] = resultVal;

                    switch (resultVal) {

                        case ROBOT_CHAR:
                            robotPosition = robotPosition.HasValue ? throw new Exception("Multiple robot positions.") : destinationCoordinates;
                            break;
                        case OBSTACLE_CHAR:
                            obstacles.Add(destinationCoordinates);
                            break;
                        case WALL_CHAR:
                            walls.Add(destinationCoordinates);
                            break;      
                        case EMPTY_SPACE_CHAR:
                            continue;
                        default:
                            throw new NotImplementedException();
                    }                    
                }

            }            
        }

        private static (char[,] matrix, string moves, (int X, int Y) robotPosition, HashSet<(int X, int Y)> obstacles, HashSet<(int X, int Y)> walls) HandleInput(string input, char[,] robotMask, char[,] obstacleMask, char[,] wallMask, char[,] emptySpaceMask, int elementHeight, int elementWidth)
        {

            HashSet<(int X, int Y)> obstacles = [];
            HashSet<(int X, int Y)> walls = [];
            (int X, int Y)? robotPosition = null;

            int tmp = input.LastIndexOf('#');
            string map = input[..(tmp + 1)];
            string moves = input[(tmp + 1)..].Replace("\r\n", String.Empty);

            string[] rows = map.Split("\r\n");

      
            int resultInputMatrixRows = rows.Length * elementHeight;
            int resultInputMatrixCols = rows[0].Length * elementWidth;
            char[,] matrix = new char[resultInputMatrixRows, resultInputMatrixCols];

            for (int i = 0, resultCurrentI = 0; i < rows.Length; i++, resultCurrentI+=elementHeight) {

                for (int j = 0, resultCurrentJ = 0; j < rows[0].Length; j++, resultCurrentJ+=elementWidth) {
                    
                    char c = rows[i][j];

                    char[,] mask = c switch
                    {
                        OBSTACLE_CHAR => obstacleMask,
                        WALL_CHAR => wallMask,
                        EMPTY_SPACE_CHAR => emptySpaceMask,
                        ROBOT_CHAR => robotMask,
                        _ => throw new NotImplementedException(),
                    };

                    MapInputIntoResultingMatrix(matrix, resultCurrentI, resultCurrentJ, ref robotPosition, c, mask, elementHeight, elementWidth, obstacles, walls);
                }
            }

            _ = robotPosition ?? throw new NullReferenceException();

            return (matrix, moves, robotPosition.Value, obstacles, walls);


        }

        private static long CalcGPS((int X, int Y) pos, int resultingObstaclesWidth = 1)
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

                if (notPermittedMoves.Contains((nextPosition.X, nextPosition.Y, move))) continue;

         
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

        private static void Debug(char[,] matrix) {

            for (int i = 0; i < matrix.GetLength(0); i++) // GetLength(0) = numero righe
            {
                for (int j = 0; j < matrix.GetLength(1); j++) // GetLength(1) = numero colonne
                {
                    Console.Write(matrix[i, j]);
                }
                Console.WriteLine(); // A capo dopo ogni riga
            }

        }





    }
}

using AdventOfCode2024.Utils;
using System.Text.RegularExpressions;


namespace AdventOfCode2024.Day14
{
    internal partial class Day14 : Day
    {
        private readonly LoadingBar _loading = new();
        private readonly bool _debugActive = true;

        protected override object SolveA(string input)
        {
            var inputs = HandleInput(input);

            int m = inputs.M; 
            int n = inputs.N;
            int time = inputs.T;

            int sectionsW = (m / 2 - 1);
            int sectionsH = (n / 2 - 1);

            MatrixSection[] sections = [
                new MatrixSection((0,0), sectionsW, sectionsH ),
                new MatrixSection(((m / 2 + 1), 0), sectionsW, sectionsH ),
                new MatrixSection((0,(n / 2 + 1)), sectionsW, sectionsH ),
                new MatrixSection(((m / 2 + 1), (n / 2 + 1)), sectionsW, sectionsH ),
                
            ];
            
            foreach (var robotPosition in inputs.RobotPositions) {

                (int X, int Y) robotDest = TimeMachine(robotPosition, m, n, time);
                foreach (var section in sections) {

                    if (section.Contains(robotDest)) { 
                        break; 
                    }
                }
            }

            int ans = 0;
            foreach (var section in sections)
            {
                if (ans == 0 && section.Count != 0) ans = 1;
                ans *= section.Count;
            }

            return ans;
        }

        
        protected override object SolveB(string input)
        {
            var inputs = HandleInput(input);

            int m = inputs.M;
            int n = inputs.N;
            int maxTime = 10000;
            int startingTime = 1;

            (int Time, int MaxClusterValue)? ans = null;

            for (int time = startingTime; time < maxTime; time++) {
                
                HashSet<(int X, int Y)> result = [];

                foreach (var robotPosition in inputs.RobotPositions)
                {
                    (int X, int Y) robotDest = TimeMachine(robotPosition, m, n, time);
                    result.Add(robotDest);
                }

                int clusterSize = FindMaxClusterSize(result);
                if (ans is null || ans.Value.MaxClusterValue < clusterSize) ans = (time, clusterSize);
                if (_debugActive) _loading.Show(time, maxTime, startingTime);
            }

            return ans?.Time ?? throw new NullReferenceException();
        }

        const string VALUES_PATTERN = @"-?\d+,-?\d+";

        [GeneratedRegex(VALUES_PATTERN)]
        private static partial Regex ValuesRegex();

        private class RobotPosition(int X, int Y, int Xs, int Ys) {
            public (int X, int Y) P { get; private set; } = (X, Y);
            public (int Xs, int Ys) V { get; private set; } = (Xs, Ys);
        }

       
        private class MatrixSection( (int X, int Y) Start, int W, int H) {

            private readonly (int X, int Y) Start = Start;
            private readonly int W = W;
            private readonly int H = H;

            public int Count { get; private set; } = 0;

            public bool Contains( (int X, int Y) point ) {

                if (point.X < Start.X) return false;
                if (point.X > Start.X + W) return false;
                if (point.Y < Start.Y) return false;
                if (point.Y > Start.Y + H) return false;

                Count++;
                return true;

                
            }

        }

        private static readonly (int Dx, int Dy)[] clusterDirections =
        [
           (-1, -1),
            (-1,0),
            (-1,1),
            (0,-1),
            (0,1),
            (1,-1),
            (1,0),
            (1,1)
        ];

        private static (int M, int N, int T, IList<RobotPosition> RobotPositions) HandleInput(string input)
        {

            string constructors = input.Substring(0, input.IndexOf("\r\n"));

            string[] tmp = constructors.Split(',');
            int m = Convert.ToInt32(tmp[0].Substring(2, tmp[0].Length - 2));
            int n = Convert.ToInt32(tmp[1].Substring(2, tmp[1].Length - 2));
            int t = Convert.ToInt32(tmp[2].Substring(2, tmp[2].Length - 2));

            List<RobotPosition> robotPositions = [];

            MatchCollection matches = ValuesRegex().Matches(input);


            for (int i = 0; i < matches.Count - 1; i += 2)
            {

                var pos = matches[i].Value.Split(',').Select(e => Convert.ToInt32(e));
                var v = matches[i + 1].Value.Split(',').Select(e => Convert.ToInt32(e));
                RobotPosition robotPosition = new RobotPosition(pos.ElementAt(0), pos.ElementAt(1), v.ElementAt(0), v.ElementAt(1));
                robotPositions.Add(robotPosition);
            }

            return (m, n, t, robotPositions);
        }


        private static (int X, int Y) TimeMachine(RobotPosition robotPosition, int m, int n, int time)
        {

            int pxDest = (robotPosition.P.X + (robotPosition.V.Xs * time)) % m;
            int pyDest = (robotPosition.P.Y + (robotPosition.V.Ys * time)) % n;

            if (pxDest < 0) pxDest = m + pxDest;
            if (pyDest < 0) pyDest = n + pyDest;

            return (pxDest, pyDest);

        }


        private static int FindMaxClusterSize(HashSet<(int X, int Y)> coordinates) {

            int maxClusterSize = 0; 

            HashSet<(int X, int Y)> alreadySeen = [];

            foreach ((int X, int Y) startingNode in coordinates) {

                if (alreadySeen.Contains(startingNode)) continue;

                int currentClusterSize = CurrentClusterSize(alreadySeen, coordinates, startingNode);
                maxClusterSize = Math.Max(maxClusterSize, currentClusterSize);           

            }

            return maxClusterSize;

        
        }

        private static int CurrentClusterSize(HashSet<(int X, int Y)> alreadySeen, HashSet<(int X, int Y)> coordinates, (int X, int Y) startingNode) {

            int currentClusterSize = 0;
            Queue<(int X, int Y)> nodes = [];
            nodes.Enqueue(startingNode);
            currentClusterSize++;

            while (nodes.Count > 0)
            {

                (int X, int Y) currentNode = nodes.Dequeue();
                alreadySeen.Add(currentNode);

                foreach (var (Dx, Dy) in clusterDirections)
                {
                    (int X, int Y) sibling = (currentNode.X + Dx, currentNode.Y + Dy);
                    if (alreadySeen.Contains(sibling) || !coordinates.Contains(sibling)) continue;
                    nodes.Enqueue(sibling);
                    currentClusterSize++;
                }

            }

            return currentClusterSize;

        }
        
       

        
    
    }
}

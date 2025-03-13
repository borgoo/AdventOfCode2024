using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2024.Day12
{
    internal class Day12 : Day
    {
        private static readonly bool _debugActive = false;

        protected override object SolveA(string input)
        {
            return CalculatePrices(input).Price;
        }

        protected override object SolveB(string input)
        {
            return CalculatePrices(input).DiscountedPrice;
        }


        private static class Directions
        {

            public static readonly (int Dx, int Dy) Left = (Dx: 0, Dy: -1);
            public static readonly (int Dx, int Dy) Right = (Dx: 0, Dy: 1);
            public static readonly (int Dx, int Dy) Up = (Dx: -1, Dy: 0);
            public static readonly (int Dx, int Dy) Down = (Dx: 1, Dy: 0);

        }


        private class Edge
        {
            public (int X, int Y) Start { get; private set; }
            public (int X, int Y) End { get; private set; }

            public Edge((int X, int Y) start, (int X, int Y) end)
            {
                Start = start;
                End = end;
            }

            public override bool Equals(object? obj)
            {
                if (obj is null) return false;

                if (obj is Edge other)
                {
                    return Start == other.Start && End == other.End;
                }
                return false;
            }
            public override int GetHashCode()
            {
                return HashCode.Combine(Start, End);
            }

        }

        private static void UpdateEdges(HashSet<Edge> edges, (int X, int Y) addedPoint)
        {

            Edge top = new((X: addedPoint.X, Y: addedPoint.Y), (X: addedPoint.X, Y: addedPoint.Y + 1));
            Edge bottom = new((X: addedPoint.X + 1, Y: addedPoint.Y), (X: addedPoint.X + 1, Y: addedPoint.Y + 1));
            Edge right = new((X: addedPoint.X, Y: addedPoint.Y + 1), (X: addedPoint.X + 1, Y: addedPoint.Y + 1));
            Edge left = new((X: addedPoint.X, Y: addedPoint.Y), (X: addedPoint.X + 1, Y: addedPoint.Y));

            if (!edges.Add(top)) edges.Remove(top);
            if (!edges.Add(bottom)) edges.Remove(bottom);
            if (!edges.Add(right)) edges.Remove(right);
            if (!edges.Add(left)) edges.Remove(left);
        }

        private static int CountCorners(HashSet<Edge> edges)
        {

            int count = 0;
            HashSet<(Edge Horizontal, Edge Vertical)> seen = new();


            foreach (Edge currentEdge in edges)
            {

                bool isHorizontal = currentEdge.Start.X == currentEdge.End.X;
                if (!isHorizontal) continue;

                Edge swCorner = new((X: currentEdge.Start.X - 1, Y: currentEdge.Start.Y), currentEdge.Start);
                if (edges.Contains(swCorner))
                {

                    (Edge Horizontal, Edge Vertical) cornerBuilder = new(currentEdge, swCorner);
                    (Edge Horizontal, Edge Vertical) twin = new(new Edge((X: currentEdge.Start.X - 1, Y: currentEdge.Start.Y), currentEdge.Start), swCorner);

                    if (!seen.Contains(cornerBuilder) && !seen.Contains(twin))
                    {
                        count++;
                        seen.Add(cornerBuilder);
                        seen.Add(twin);
                    }


                }

                Edge nwCorner = new(currentEdge.Start, (X: currentEdge.Start.X + 1, Y: currentEdge.Start.Y));
                if (edges.Contains(nwCorner))
                {

                    (Edge Horizontal, Edge Vertical) cornerBuilder = new(currentEdge, nwCorner);
                    (Edge Horizontal, Edge Vertical) twin = new(new Edge((X: currentEdge.Start.X, Y: currentEdge.Start.X - 1), currentEdge.Start), nwCorner);

                    if (!seen.Contains(cornerBuilder) && !seen.Contains(twin))
                    {
                        count++;
                        seen.Add(cornerBuilder);
                        seen.Add(twin);
                    }

                }

                Edge seCorner = new((X: currentEdge.End.X - 1, Y: currentEdge.End.Y), currentEdge.End);
                if (edges.Contains(seCorner))
                {

                    (Edge Horizontal, Edge Vertical) cornerBuilder = new(currentEdge, seCorner);
                    (Edge Horizontal, Edge Vertical) twin = new(new Edge(currentEdge.End, (X: currentEdge.End.X, Y: currentEdge.End.Y + 1)), seCorner);

                    if (!seen.Contains(cornerBuilder) && !seen.Contains(twin))
                    {
                        count++;
                        seen.Add(cornerBuilder);
                        seen.Add(twin);
                    }

                }

                Edge neCorner = new(currentEdge.End, (X: currentEdge.End.X + 1, Y: currentEdge.End.Y));
                if (edges.Contains(neCorner))
                {

                    (Edge Horizontal, Edge Vertical) cornerBuilder = new(currentEdge, neCorner);
                    (Edge Horizontal, Edge Vertical) twin = new(new Edge(currentEdge.End, (X: currentEdge.End.X, Y: currentEdge.End.Y + 1)), neCorner);

                    if (!seen.Contains(cornerBuilder) && !seen.Contains(twin))
                    {
                        count++;
                        seen.Add(cornerBuilder);
                        seen.Add(twin);
                    }

                }

            }

            return count;
        }

        private static (int DeltaPrice, int DeltaDiscountedPrice) Bfs(HashSet<(int X, int Y)> seen, string[] rows, int m, int n, (int X, int Y) startingPoint)
        {

            int price = 0;
            int discountedPrice = 0;
            if (seen.Contains(startingPoint)) return (DeltaPrice: price, DeltaDiscountedPrice: discountedPrice);

            HashSet<(int X, int Y)> currentGardenPoints = new();
            HashSet<Edge> edges = new();

            char currVal = rows[startingPoint.X][startingPoint.Y];
            Queue<(int X, int Y)> nodes = new();
            nodes.Enqueue(startingPoint);
            currentGardenPoints.Add(startingPoint);
            UpdateEdges(edges, startingPoint);
            seen.Add(startingPoint);

            if(_debugActive) Console.Write($"{rows[startingPoint.X][startingPoint.Y]}");

            while (nodes.Count > 0)
            {
                int numOfParents = nodes.Count;
                for (int i = 0; i < numOfParents; i++)
                {

                    (int X, int Y) currPoint = nodes.Dequeue();

                    if (currPoint.X > 0)
                    {
                        (int X, int Y) neighbor = (X: currPoint.X + Directions.Up.Dx, Y: currPoint.Y + Directions.Up.Dy);
                        char neighborVal = rows[neighbor.X][neighbor.Y];

                        if (!seen.Contains(neighbor) && currVal == neighborVal)
                        {
                            seen.Add(neighbor);
                            currentGardenPoints.Add(neighbor);
                            nodes.Enqueue(neighbor);
                            UpdateEdges(edges, neighbor);

                        }
                    }

                    if (currPoint.X < m - 1)
                    {
                        (int X, int Y) neighbor = (X: currPoint.X + Directions.Down.Dx, Y: currPoint.Y + Directions.Down.Dy);
                        char neighborVal = rows[neighbor.X][neighbor.Y];

                        if (!seen.Contains(neighbor) && currVal == neighborVal)
                        {
                            seen.Add(neighbor);
                            currentGardenPoints.Add(neighbor);
                            nodes.Enqueue(neighbor);
                            UpdateEdges(edges, neighbor);

                        }
                    }

                    if (currPoint.Y > 0)
                    {
                        (int X, int Y) neighbor = (X: currPoint.X + Directions.Left.Dx, Y: currPoint.Y + Directions.Left.Dy);
                        char neighborVal = rows[neighbor.X][neighbor.Y];

                        if (!seen.Contains(neighbor) && currVal == neighborVal)
                        {
                            seen.Add(neighbor);
                            currentGardenPoints.Add(neighbor);
                            nodes.Enqueue(neighbor);
                            UpdateEdges(edges, neighbor);

                        }
                    }

                    if (currPoint.Y < n - 1)
                    {
                        (int X, int Y) neighbor = (X: currPoint.X + Directions.Right.Dx, Y: currPoint.Y + Directions.Right.Dy);
                        char neighborVal = rows[neighbor.X][neighbor.Y];

                        if (!seen.Contains(neighbor) && currVal == neighborVal)
                        {
                            seen.Add(neighbor);
                            currentGardenPoints.Add(neighbor);
                            nodes.Enqueue(neighbor);
                            UpdateEdges(edges, neighbor);

                        }
                    }

                }
            }

            int perimeter = edges.Count;
            int numOfCorners = CountCorners(edges);
            int numOfEdges = numOfCorners % 2 != 0 ? numOfCorners + 1 : numOfCorners;

            price = perimeter * currentGardenPoints.Count;
            discountedPrice = numOfEdges * currentGardenPoints.Count;
           if(_debugActive) Console.WriteLine($" (discountedPrice: {discountedPrice} - perimeter {perimeter} area: {currentGardenPoints.Count}, edges: {numOfEdges})");

            return (DeltaPrice: price, DeltaDiscountedPrice: discountedPrice);


        }

        private static (int Price, int DiscountedPrice) CalculatePrices(string input)
        {

            int price = 0;
            int discountedPrice = 0;
            string[] rows = input.Split("\r\n");
            int m = rows.Length;
            int n = rows[0].Length;

            HashSet<(int X, int Y)> seen = new();


            for (int x = 0; x < m; x++)
            {
                for (int y = 0; y < n; y++)
                {

                    (int DeltaPrice, int DeltaDiscountedPrice) gardenPrice = Bfs(seen, rows, m, n, (X: x, Y: y));
                    price += gardenPrice.DeltaPrice;
                    discountedPrice += gardenPrice.DeltaDiscountedPrice;
                }
            }




            return (Price: price, DiscountedPrice: discountedPrice);
        }



    }
}

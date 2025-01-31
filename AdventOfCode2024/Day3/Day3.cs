using System.Text.RegularExpressions;

namespace AdventOfCode2024.Day3
{
    internal class Day3 : Day
    {
        protected override object SolveA(string input)
        {
            return CalculateMul(input);
        }

        protected override object SolveB(string input)
        {
            return CalculateMul(input, true);
        }

        const string MUL_REGEX = @"mul\((\d{1,3}),(\d{1,3})\)";
        const string DO_REGEX = @"do()";
        const string DONT_REGEX = @"don't()";

        private static int CalculateMul(string txt, bool deactivable = false) {

            Queue<(int Start, int End)> deactiveRanges = new();
            Queue<int> dontPositions = new();
           
            if (deactivable) {

                MatchCollection matches = Regex.Matches(txt, DONT_REGEX);
                foreach (Match match in matches)
                {
                    int pos = match.Index;
                    dontPositions.Enqueue(pos);
                }

                matches = Regex.Matches(txt, DO_REGEX);
                foreach (Match match in matches)
                {
                    int posDo = match.Index;
                    if (dontPositions.Count > 0 && posDo > dontPositions.Peek()) {
                        deactiveRanges.Enqueue((Start: dontPositions.Dequeue(), End: posDo));
                        while (dontPositions.Count > 0 && dontPositions.Peek() < posDo) { 
                            dontPositions.Dequeue();
                        }
                    }
                }
                if (matches.Count < 1) {
                    int deactiveRangeStart = dontPositions.Count > 0 ? dontPositions.Dequeue() : txt.Length;
                    deactiveRanges.Enqueue((Start: deactiveRangeStart, End: txt.Length ));
                }

            }

            int ans = 0;

            MatchCollection regexMatches = Regex.Matches(txt, MUL_REGEX);
            foreach (Match match in regexMatches)
            {

                int mulPosition = match.Index;

                while (deactiveRanges.Count > 0 && deactiveRanges.Peek().End < mulPosition) {
                    deactiveRanges.Dequeue();
                }

                if (deactiveRanges.Count > 0 && deactiveRanges.Peek().Start < mulPosition) continue;

                int a = Convert.ToInt32(match.Groups[1].Value);
                int b = Convert.ToInt32(match.Groups[2].Value);
                ans += a * b;
            }



            return ans;
        }


    }
}

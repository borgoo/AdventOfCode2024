using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode2024.Day13
{
    internal partial class Day13 : Day
    {
        const int BUTTON_A_COST = 3;
        const int BUTTON_B_COST = 1;
        const long SOLVE_B_SHIFTING = 10000000000000;

        protected override object SolveA(string input)
        {
            long ans = 0;
            var clawMachines = HandleInput(input);

            foreach (var clawMachine in clawMachines) {
                ans += MinCost(clawMachine) ?? 0;            
            }

            return ans;
        }

        protected override object SolveB(string input)
        {
            long ans = 0;
            var clawMachines = HandleInput(input);

            foreach (var clawMachine in clawMachines)
            {
                clawMachine.ShiftPrize(SOLVE_B_SHIFTING);
                ans += MinCost(clawMachine) ?? 0;
            }

            return ans;
        }

        const string BUTTON_BEHAVIOR_PATTERN = @"(?<=\+)\d+";
        const string COORDINATES_PATTERN = @"(?<=\=)\d+";

        [GeneratedRegex(BUTTON_BEHAVIOR_PATTERN)]
        private static partial Regex ButtonBehaviorRegex();    
        
        [GeneratedRegex(COORDINATES_PATTERN)]
        private static partial Regex CoordinatesPattern();

        private class ClawMachine(int aDx, int aDy, int bDx, int bDy, long prizeX, long prizeY) {

            public (int Dx, int Dy) ButtonA { get; private set; } = (aDx, aDy);
            public (int Dx, int Dy) ButtonB { get; private set; } = (bDx, bDy);
            public (long X, long Y) Prize { get; private set; } = (prizeX, prizeY);

            public void ShiftPrize(long shiftValueX, long shiftValueY) {
                Prize = (Prize.X + shiftValueX, Prize.Y + shiftValueY);
            }      
            
            public void ShiftPrize(long shiftValue) {
                ShiftPrize(shiftValue, shiftValue);
            }
        }

        private static IList<ClawMachine> HandleInput(string input) {

            MatchCollection buttonBehaviorMatches = ButtonBehaviorRegex().Matches(input);
            MatchCollection prizesMatches = CoordinatesPattern().Matches(input);

            List<ClawMachine> res = [];
            int j = 0;
            int i = 0;

            while (j < prizesMatches.Count) {

                int aDx = Convert.ToInt32(buttonBehaviorMatches[i].Value); i++;
                int aDy = Convert.ToInt32(buttonBehaviorMatches[i].Value); i++;
                int bDx = Convert.ToInt32(buttonBehaviorMatches[i].Value); i++;
                int bDy = Convert.ToInt32(buttonBehaviorMatches[i].Value); i++;
                int prizeX = Convert.ToInt32(prizesMatches[j].Value); j++;
                int prizeY = Convert.ToInt32(prizesMatches[j].Value); j++;

                res.Add(new(aDx, aDy, bDx, bDy, prizeX, prizeY));
            }

            return res;

        }

        /// <summary>
        /// From eq.system ADx*X+BDx*Y = PrizeX , ADy*X+BDy*Y = PrizeY
        /// </summary>
        /// <param name="clawMachine"></param>
        /// <param name="buttonACost"></param>
        /// <param name="buttonBCost"></param>
        /// <returns></returns>
        private static long? MinCost(ClawMachine clawMachine, int buttonACost = BUTTON_A_COST, int buttonBCost = BUTTON_B_COST) {

            long num = (clawMachine.Prize.Y * clawMachine.ButtonB.Dx) - (clawMachine.Prize.X * clawMachine.ButtonB.Dy);
            long den = (clawMachine.ButtonB.Dx * clawMachine.ButtonA.Dy) - (clawMachine.ButtonB.Dy * clawMachine.ButtonA.Dx);

            decimal numOfTimeToPressA = (decimal)num / (decimal)den;
            if (numOfTimeToPressA % 1 != 0) return null;

            num = clawMachine.Prize.X - (clawMachine.ButtonA.Dx * (long)numOfTimeToPressA);
            den = clawMachine.ButtonB.Dx;
            decimal numOfTimeToPressB = (decimal)num / (decimal)den;
            if (numOfTimeToPressB % 1 != 0) return null;

            return Convert.ToInt64((long)numOfTimeToPressA * buttonACost + (long)numOfTimeToPressB * buttonBCost);

        }

    }
}

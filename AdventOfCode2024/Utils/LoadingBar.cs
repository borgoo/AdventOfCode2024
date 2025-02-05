using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2024.Utils
{
    internal class LoadingBar
    {
        const char LOADING_CHAR = '=';
        const string EMPTY = "[                    ]";
        private static readonly int _steps = EMPTY.Count(c => c == ' ');
        private static readonly int _naturalPadding = EMPTY.IndexOf('[')+1;
        private static readonly int _valPosition = EMPTY.IndexOf(']')+1;
        private static readonly int _eachStepPercentage = 100/_steps;

        public bool Enabled = true;

        private int? OldNormalizedPecentage { get; set; } = null;


        public void Show(int currentVal, int endLoopVal, int startingLoopVal = 0, bool lessAndNotEqual = true) {

            if (!Enabled) return;

            int done = currentVal - startingLoopVal;
            int toBeDone = lessAndNotEqual ? -1 : 0;
            toBeDone += (endLoopVal - startingLoopVal);

            Show(done, toBeDone);


        }
        public void Show(int done, int toBeDone) {

            if (!Enabled) return;
            Show(done * 100 / toBeDone);
            
        }

        public void Show(int effectivePercentage)
        {
            if (!Enabled) return;

            if (OldNormalizedPecentage is null)
            {
                OldNormalizedPecentage = 0;
                Console.Write($"{EMPTY}  0%");
                return;
            }

            int numOfSteps = effectivePercentage / _eachStepPercentage;
            int normalizedPercentage = numOfSteps * _eachStepPercentage;

            int progressInPercentage = normalizedPercentage - OldNormalizedPecentage.Value;

            if (progressInPercentage == 0) return;
            int numOfStepsToAdd = progressInPercentage / _eachStepPercentage;
            int numOfStepsAlreadyAdded = OldNormalizedPecentage.Value / _eachStepPercentage;



            int calcCursorPostion = 0 + _naturalPadding + numOfStepsAlreadyAdded;
            Console.SetCursorPosition(calcCursorPostion, Console.GetCursorPosition().Top);
            for (int i = 0; i < numOfStepsToAdd; i++) Console.Write(LOADING_CHAR);


            if (normalizedPercentage < 10) Console.SetCursorPosition(_valPosition + 2, Console.GetCursorPosition().Top);
            else if (normalizedPercentage < 100) Console.SetCursorPosition(_valPosition + 1, Console.GetCursorPosition().Top);
            else Console.SetCursorPosition(_valPosition, Console.GetCursorPosition().Top);
            Console.Write(normalizedPercentage);

            if (normalizedPercentage == 100) Console.WriteLine();


            OldNormalizedPecentage = normalizedPercentage;
            return;
        }

        public void Terminate(bool appendNewLine = true) {

            if (!Enabled) return;

            Show(100);
        }
      
    }
}

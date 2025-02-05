using AdventOfCode2024.Utils;
using Serilog;
using System.Diagnostics;
using System.Text.Json;

namespace AdventOfCode2024
{

    internal abstract class Day
    {
        private readonly bool TIMER_ACTIVE = true;
        protected readonly LoadingBar _loadingBar = new();
        protected readonly WaitingBar _waitingBar = new();

        

        protected abstract object SolveA(string input);
        protected abstract object SolveB(string input);


        public void Run() {

            SayHi();

            List<string> inputs = ReadInputFiles();
            Solution[] expectedResults = ReadTestFile();
            Stopwatch stopwatch = new();
            for (int i = 0; i < inputs.Count; i++) {
                Log.Debug($"== input {i + 1} ==");
                try
                {
                    stopwatch.Start();
                    object resA = SolveA(inputs[i]);
                    stopwatch.Stop();
                    Assert("SolA", resA, expectedResults[i].SolutionA);
                    if (TIMER_ACTIVE) PrintTime(stopwatch);
                }
                catch (NotImplementedException) {
                    NotImplemented("SolA");
                }

                try
                {
                    stopwatch.Restart();
                    object resB = SolveB(inputs[i]);
                    stopwatch.Stop();
                    Assert("SolB", resB, expectedResults[i].SolutionB);
                    if (TIMER_ACTIVE) PrintTime(stopwatch);
                } catch (NotImplementedException)
                {
                    NotImplemented("SolB");
                }

              
            }

            SayGoodBye();
        }

        public class Input
        {
            public string[] Body { get; set; } = [];
        }
        public class Test
        {
            public Solution[] Body { get; set; } = [];
        }
        public class Solution { 
            public object SolutionA { get; set; }
            public object SolutionB{ get; set; }
        }

        protected string GetClassName() {

            Type classType = this.GetType();
            return classType.Name;
        }

        protected string GetSubDirName() {
            return GetClassName();
        }

        protected List<string> ReadInputFiles() {
            List<string> result = new();
            int i = 1;
            string fileName = Path.Combine(GetSubDirName(), $"{GetClassName()}.input.{i}.txt");
            while (File.Exists(fileName)) {
                string body = File.ReadAllText(fileName);
                result.Add(body);
                i++;
                fileName = Path.Combine(GetSubDirName(), $"{GetClassName()}.input.{i}.txt");
            }

            return result;

        }
        protected Solution[] ReadTestFile()
        {
            string fileName = Path.Combine(GetSubDirName(), $"{GetClassName()}.test.json");
            string json = String.Empty;
            if (File.Exists(fileName)) json = File.ReadAllText(fileName);
            Test? test = JsonSerializer.Deserialize<Test>(json);
            return test?.Body ?? [];

        }

        protected void SayHi() { 

            string className = GetClassName();
            Log.Debug($"[{className}]");
        }

        protected static void SayGoodBye() {
            Log.Debug("\n");
        }



        protected static void Assert(string str, object result, object expectedResult) {

            _ = result ?? throw new NullReferenceException("Null result not allowed.");
            _ = expectedResult ?? throw new NullReferenceException("Null expected result not allowed.");

            if (result.ToString() != expectedResult.ToString()) {
                Log.Debug($"{str}: [{TurnRed("FAIL")}]");
                Log.Debug($"  Expected: {expectedResult}\n  Result: {result}");
            }
            else {
                Log.Debug($"{str}: {TurnGreen("ok")}");
            }
                              
        
        }

        private static string TurnRed(string txt) {

            return $"\u001b[31m{txt}\u001b[0m";
        
        }        
        
        private static string TurnGreen(string txt) {

            return $"\x1b[38;5;154m{txt}\x1b[0m";
        
        }

        private static void NotImplemented(string str) {
            Log.Debug($"{str} not implemented yet.");
        }

        private static void PrintTime(Stopwatch stopwatch) {

            string unit;
            double time = stopwatch.Elapsed.TotalMicroseconds;
            if (time < 1000) {
                unit = "μs";
            }
            else if (time > 1000 && time < 1000000) {
                time = time / 1000;
                unit = "ms";
            }
            else {
                time = time / 1000000;
                unit = "s";
            }

            Log.Debug($"  Time: {time:0.0} {unit}");

        }
    }
}

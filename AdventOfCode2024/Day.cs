﻿using AdventOfCode2024.Exceptions;
using AdventOfCode2024.Utils;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace AdventOfCode2024
{

    internal abstract class Day
    {
        private readonly bool TIMER_ACTIVE = true;
        protected readonly LoadingBar _loadingBar = new();
        protected readonly WaitingBar _waitingBar = new();

        protected bool SolBIsRunning { get; private set; } = false; 

        

        protected abstract object SolveA(string input);
        protected abstract object SolveB(string input);


        public void Run() {

            SayHi();

            List<string> inputs = ReadInputFiles();
            Solution[] expectedResults = ReadTestFile();
            Stopwatch stopwatch = new();
            for (int i = 0; i < inputs.Count; i++) {
                Console.WriteLine($"== input {i + 1} ==");
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
                catch (IsChristmasTimeException)
                {
                    MerryChristmas("SolA");
                }

                try
                {
                    stopwatch.Restart();
                    SolBIsRunning = true;
                    object resB = SolveB(inputs[i]);
                    SolBIsRunning = false;
                    stopwatch.Stop();
                    Assert("SolB", resB, expectedResults[i].SolutionB);
                    if (TIMER_ACTIVE) PrintTime(stopwatch);
                }
                catch (NotImplementedException)
                {
                    NotImplemented("SolB");
                }
                catch (IsChristmasTimeException) {
                    MerryChristmas("SolB");
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
            public object? SolutionA { get; set; }
            public object? SolutionB{ get; set; }
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
            Console.WriteLine($"[{className}]");
        }

        protected static void SayGoodBye() {
            Console.WriteLine("\n");
        }



        protected static void Assert(string str, object? result, object? expectedResult) {

            result ??= "null";
            expectedResult ??= "null";

            if (result.ToString() != expectedResult.ToString()) {
                Console.WriteLine($"{str}: [{TurnRed("FAIL")}]");
                Console.WriteLine($"  Expected: {expectedResult}\n  Result: {result}");
            }
            else {
                Console.WriteLine($"{str}: {TurnGreen("ok")}");
            }
                              
        
        }

        private static string TurnRed(string txt) {

            return $"\u001b[31m{txt}\u001b[0m";
        
        }        
        
        private static string TurnGreen(string txt) {

            return $"\x1b[38;5;154m{txt}\x1b[0m";
        
        }

        private static void NotImplemented(string str) {
            Console.WriteLine($"{str} not implemented yet.");
        }

        private static void MerryChristmas(string str) {

            str = str+": IS CHRISTMAS TIIIIIME!";
            StringBuilder sb = new();
            for (int i = 0; i < str.Length; i++)
            {

                if (i % 2 == 0) sb.Append(TurnGreen(str[i].ToString()));
                else sb.Append(TurnRed(str[i].ToString()));
            }

            Console.WriteLine(sb.ToString());
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

            Console.WriteLine($"  Time: {time:0.0} {unit}");

        }
    }
}

using AdventOfCode2024.Exceptions;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace AdventOfCode2024.Day17
{
    internal class Day17 : Day
    {
        protected override object SolveA(string input)
        {
            var (regA, regB, regC, program) = HandleInput(input);

            string ans = new Program(program, regA, regB, regC).Run();

            return ans;
        }

        private static int c = 0; 
        protected override object SolveB(string input)
        {
            if (c < 2) { c++; throw new NotImplementedException(); }
            var (regA, regB, regC, program) = HandleInput(input);

            string ans = String.Empty;

            long regAVal = 0;
            while (ans != program) {
                ans = new Program(program, regAVal, regB, regC).Run();
            }

            return regAVal;
        }

        const string PATTERN_REG_A = @"Register A:\s*(\d+)";
        const string PATTERN_REG_B = @"Register B:\s*(\d+)";
        const string PATTERN_REG_C = @"Register C:\s*(\d+)";
        const string PATTERN_PROG = @"Program:\s*([\d,]+)";
        private static (long regA, long regB, long regC, string program) HandleInput(string input) {

            Match matchA = Regex.Match(input, PATTERN_REG_A);
            Match matchB = Regex.Match(input, PATTERN_REG_B);
            Match matchC = Regex.Match(input, PATTERN_REG_C);
            Match matchProgram = Regex.Match(input, PATTERN_PROG);

            if (!matchA.Success) throw new NotFoundException("Reg A init value");
            long regA = Convert.ToInt64(matchA.Groups[1].Value);
            if (!matchB.Success) throw new NotFoundException("Reg B init value");
            long regB = Convert.ToInt64(matchB.Groups[1].Value);
            if (!matchC.Success) throw new NotFoundException("Reg C init value");
            long regC = Convert.ToInt64(matchC.Groups[1].Value);
            if (!matchProgram.Success) throw new NotFoundException("Program init value");
            string program = matchProgram.Groups[1].Value;

            return (regA, regB, regC, program);
        }
     

        public class Program(string programStr, long regA = 0, long regB = 0, long regC = 0) {

            private static readonly HashSet<int> _canBeLaunchedWithoutOperand = [4];
            private readonly int[] ProgramCode = programStr is null ? throw new NullReferenceException() : programStr.Split(',').Select(e => e[0] - '0').ToArray();

            private long RegA { get; set; } = regA;
            private long RegB { get; set; } = regB;
            private long RegC { get; set; } = regC;

            private IList<int> Output { get; set; } = [];
            private int InstructionPointer { get; set; } = 0;

 

            private long GetComboOperand(int operand)
            {

                return operand switch
                {
                    0 or 1 or 2 or 3 => operand,
                    4 => RegA,
                    5 => RegB,
                    6 => RegC,
                    _ => throw new NotImplementedException(),
                };
            }

            private static int GetLiteralOperand(int operand) => operand;

            private long CommonPow(int operand) {

                double numerator = RegA;
                double denominator = Math.Pow(2, GetComboOperand(operand));
                return (long)(numerator / denominator);
            }

            private void Adv(int operand) {
                RegA = CommonPow(operand);
            }

            private void Bxl(int operand) {

                RegB = RegB ^ GetLiteralOperand(operand);
            }

            private void Bst(int operand) {
                
                const int mod = 8;

                RegB = GetComboOperand(operand) % mod;
            }

            private void Jnz(int operand) {

                if (RegA == 0)
                {
                    InstructionPointer++;
                    return;
                }


                InstructionPointer = GetLiteralOperand(operand);
            }

            private void Bxc() {

                long result = RegB ^ RegC;
                RegB = result;
            }

            private void Out(int operand) {

                const int mod = 8;
                int res = (int) (GetComboOperand(operand) % mod);
                Output.Add(res);
            }

            private void Bdv(int operand) {

                RegB = CommonPow(operand);

            }

            private void Cdv(int operand) {

                RegC = CommonPow(operand);
            }

            private void ExecOp(int opCode, int? operand) {

                switch (opCode) {

                    case 4:
                        Bxc();
                        break;
                    case 0:
                        Adv(operand ?? throw new NullReferenceException()); 
                        break;
                    case 1:
                        Bxl(operand ?? throw new NullReferenceException());
                        break;
                    case 2:
                        Bst(operand ?? throw new NullReferenceException());
                        break;
                    case 3:
                        Jnz(operand ?? throw new NullReferenceException());
                        return;
    
                    case 5:
                        Out(operand ?? throw new NullReferenceException());
                        break;
                    case 6:
                        Bdv(operand ?? throw new NullReferenceException());
                        break;
                    case 7:
                        Cdv(operand ?? throw new NullReferenceException());
                        break;
                    default:
                        throw new NotImplementedException();
                }

                InstructionPointer+=2;
            }


            public string Run() {

                while (InstructionPointer < ProgramCode.Length)
                {
                    int opCode = ProgramCode[InstructionPointer];
                    int operandPointer = InstructionPointer + 1;

                    int? operand = null;

                    if(operandPointer < ProgramCode.Length) operand = ProgramCode[operandPointer];
                    else if(!_canBeLaunchedWithoutOperand.Contains(opCode)) {
                        break;
                    }                 

                    ExecOp(opCode, operand);

                }

                return GetOutputAsString();
            }

            private string GetOutputAsString() {
                return string.Join(',', Output);
            }
        }



    }
}

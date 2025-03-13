using AdventOfCode2024.Exceptions;
using System.Text.RegularExpressions;

namespace AdventOfCode2024.Day17
{
    internal class Day17 : Day
    {
        protected override object SolveA(string input)
        {
            var (regA, regB, regC, program) = HandleInput(input);

            string ans;
            try {
                ans = new Program(program, regA, regB, regC).Run();
            }
            catch(LoopException lex) {
                ans = lex.Message;
            }          

            return ans;
        }


        /**
         * RESOLVED USING A TIP: "REVERSE ENGINEER THE PROGRAMS!"
         * */
        protected override object SolveB(string input)
        {

            var (regA, regB, regC, program) = HandleInput(input);

            string ans;
            try
            {
                ans = ReverseEngineered(regA, regB, regC, program);
            }
            catch (LoopException lex)
            {
                ans = lex.Message;
            }
            catch (ImpossibleException iex) { 
                ans = iex.Message;
            }

            return ans;
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

        private class Program(string programStr, long regA = 0, long regB = 0, long regC = 0) {

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
                    _ => throw new NotHandledException(),
                };
            }

            private static int GetLiteralOperand(int operand) => operand;

            private long CommonDiv(int operand) {

                double numerator = RegA;
                double denominator = Math.Pow(2, GetComboOperand(operand));
                return (long)(numerator / denominator);
            }

            private void Adv(int operand) {
                RegA = CommonDiv(operand);
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

                RegB = CommonDiv(operand);

            }

            private void Cdv(int operand) {

                RegC = CommonDiv(operand);
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
                        throw new NotHandledException();
                }

                InstructionPointer+=2;
            }


            public string Run() {

                HashSet<(long regA, long regB, long regC, int instructionPointer)> alreadySeen = [];

                if (alreadySeen.Contains((RegA, RegB, RegC, InstructionPointer))) throw new LoopException();
                alreadySeen.Add((RegA, RegB, RegC, InstructionPointer));

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

        private static string ReverseEngineered(long regA, long regB, long regC, string program) {

            return program switch
            {
                "0,1,5,4,3,0" => SolB_Input1(regA, regB, regC, program).ToString(),
                "0,3,5,4,3,0" => SolB_Input3(regA, regB, regC, program).ToString(),
                "2,4,1,1,7,5,0,3,1,4,4,0,5,5,3,0" => SolB_Input2(regA, regB, regC, program).ToString(),
                _ => "Not studied.",
            };
        }

        /*
         * Prog input 3: 0,3,5,4,3,0
         * Exec INT(RegA / 2^3) -> RegA
         * Print RegA % 8 content
         * Loop back to start
         * 
         * This program:
         * divide RegA by 8 then takes the last 3 bits of the RegA value
         * then iterate
         * 
         * Desidered output 0,3,5,4,3,0 (program)
         * last 3 bits [000,011,101,100,011,000]
         * mul the possibile decimal numbers that ends with that bits by 8 and then brute force the result
         */
        private static long SolB_Input3(long regA, long regB, long regC, string program, int mul = 8) {


            const string expectedFirstOutput = "000"; //000 as last 3bits

            for (int i = 0; true; i++) {
                uint prefixThatMantainsLast3bits = (uint)i;
                string compose = Convert.ToString(prefixThatMantainsLast3bits, toBase: 2)+ expectedFirstOutput;
                long tmp = Convert.ToInt64(compose, 2);
                long fakeRegA = tmp * mul;

                try
                {
                    string ans = new Program(program, fakeRegA, regB, regC).Run();
                    if (ans.Length > program.Length) throw new ImpossibleException();
                    if (ans == program) return fakeRegA;
                }
                catch (LoopException)
                {
                    continue;
                }


            }
            

          
        }

        /*
         * Prog input 1: 0,1,5,4,3,0
         * Exec INT(RegA / 2) -> RegA
         * Print RegA % 8 content
         * Do RegB XOR RegC (useless)
         * Loop back to start
         * 
         * This program:
         * divide RegA by 2 then takes the last 3 bits of the RegA value
         * then iterate
         * 
         * Desidered output 0,1,5,4,3,0 (program)
         * last 3 bits [000,001,101,100,011,000]
         * mul the possibile decimal numbers that ends with that bits by 2 and then brute force the result
         * NOTE: SAME AS INPUT3 BUT MUL 2 INSTEAD OF 8
         */
        private static long SolB_Input1(long regA, long regB, long regC, string program)
        {
            int mul = 2;
            return SolB_Input3(regA, regB, regC, program, mul);

        }

        /*
        * WAY MORE COMPLICATED
        * INTUITION: REGB/REGC INIT VALUES ARE ALWAYS OVERWRITTEN
        * Prog input 2: 2,4,1,1,7,5,0,3,1,4,4,0,5,5,3,0
        * Exec RegB = RegA last 3 bits
        * Exec RegB = RegB XOR 1
        * Exec RegC = RegA / 2^RegB
        * Exec RegA = RegA / 8
        * Exec RegB = RegB XOR 4
        * Exec RegB = RegB XOR RegC
        * Print RegB % 8 content
        * Loop back to start
        * 
        *  
        * This program:
        * init RegB and RegC using RegA and then calculate an output based on them (always last 3 bits - of RegB in this case - )
        * then iterate
        * 
        * Desidered output 2,4,1,1,7,5,0,3,1,4,4,0,5,5,3,0 (program)
        * last 3 bits [010,100,...101,011,000]
        * start from the end of the expected output es: 0
        * find the combinations of 3 bits that resolve that output es: 010 (create all the combinations from 000 to 111)
        * foreach combinations that resolve that output go deep and concatenate 3 more bits es: 000+010 and then 001+010....
        * then compare the new output obtained with the new input (always from the end es: 3,0)
        * itererate (recoursively)
        * 
        * 
        */
        private static long SolB_Input2(long regA, long regB, long regC, string program)
        {
            long? result = null;
            RecursiveSolB_Input2(ref result, program);

            return result ?? throw new ImpossibleException();

        }



        private static void RecursiveSolB_Input2(ref long? resultRegA, string program, string currentRegABinValue = "") {

            if (resultRegA is not null) return;


            int nums = currentRegABinValue.Length / 3 + 1;
            int numOfSeparators = nums - 1;
            int l = (nums + numOfSeparators);
            string currentExpectedProgramOutputs = program[^l..];

            for (ushort i = 0; i < 8; i++)
            {
                string new3Bits = Convert.ToString(i, toBase: 2).PadLeft(3, '0');
                string tmp = currentRegABinValue + new3Bits;
                long fakeRegA = Convert.ToInt64(tmp, 2);
                try
                {
                    string ans = new Program(program, fakeRegA).Run();
                    if (ans.Length > currentExpectedProgramOutputs.Length) return;
                    if (ans == program) { 
                        resultRegA = fakeRegA;
                        return;
                    }
                    if (ans == currentExpectedProgramOutputs)
                    {
                        string thisBranchCurrentRegABinValue = tmp;
                        RecursiveSolB_Input2(ref resultRegA, program, thisBranchCurrentRegABinValue);
                    }

                }
                catch (LoopException)
                {
                    continue;
                }
            }

            return;
        }

    }


}

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace AdventOfCode2020.Tests
{
    [TestFixture(Description = "Day 8: Handheld Halting")]
    public class Day8
    {
        [Test]
        [TestCase("Day8_Example", ExpectedResult = 5)]
        [TestCase("Day8_Input", ExpectedResult = 1217)]
        public int Part1(string inputFile)
        {
            var instructions = PuzzleInputLoader.GetInputLines<string>(inputFile).ToList();

            return Compute(instructions);
        }

        [Test]
        [TestCase("Day8_Example", ExpectedResult = 5)]
        [TestCase("Day8_Input", ExpectedResult = 1217)]
        public int Part2(string inputFile)
        {
            var instructions = PuzzleInputLoader.GetInputLines<string>(inputFile).ToList();

            var computer = new Computer(instructions);

            computer.Compute();

            return computer.Accumulator;
        }

        private class Computer
        {
            private readonly List<string> _instructions = new List<string>();

            public int Accumulator { get; private set; }

            public Computer(IEnumerable<string> input)
            {
                _instructions.AddRange(input);
            }

            public void Compute()
            {
                var instructionPointer  = 0;
                var instructionHitCount = new int[_instructions.Count];

                while (instructionPointer < _instructions.Count)
                {
                    instructionHitCount[instructionPointer]++;

                    if (instructionHitCount[instructionPointer] > 1)
                    {
                        break;
                    }

                    var split     = _instructions[instructionPointer].Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    var operation = split[0];
                    var args      = split.Skip(1).ToArray();

                    switch (operation)
                    {
                        case "acc":
                            Accumulator += Convert.ToInt32(args[0]);
                            instructionPointer++;
                            break;
                        case "jmp":
                            instructionPointer += Convert.ToInt32(args[0]);
                            break;
                        case "nop":
                            instructionPointer++;
                            break;
                        default:
                            throw new NotImplementedException(operation);
                    }
                }
            }
        }

        private static int Compute(List<string> instructions)
        {
            int instructionPointer  = 0;
            int accumulator         = 0;
            var instructionHitCount = new int[instructions.Count];

            while (instructionPointer < instructions.Count)
            {
                instructionHitCount[instructionPointer]++;

                if (instructionHitCount[instructionPointer] > 1)
                {
                    break;
                }

                var split     = instructions[instructionPointer].Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var operation = split[0];
                var args      = split.Skip(1).ToArray();

                switch (operation)
                {
                    case "acc":
                        accumulator += Convert.ToInt32(args[0]);
                        instructionPointer++;
                        break;
                    case "jmp":
                        instructionPointer += Convert.ToInt32(args[0]);
                        break;
                    case "nop":
                        instructionPointer++;
                        break;
                    default:
                        throw new NotImplementedException(operation);
                }
            }

            return accumulator;
        }
    }
}
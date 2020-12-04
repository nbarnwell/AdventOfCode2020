using System;
using System.Linq;
using NUnit.Framework;

namespace AdventOfCode2020.Tests
{
    public class Day3
    {
        [Test]
        [TestCase("Day3_Example", ExpectedResult = 7)]
        [TestCase("Day3_Input", ExpectedResult = 286)]
        public int Part1(string inputFile)
        {
            var input = PuzzleInputLoader.GetInput<string>(inputFile).ToArray();

            return Enumerable.Range(0, input.Count())
                      .Select(row => new {X = (row * 3) % input[row].Length, Y = row})
                      .Count(x => input[x.Y][x.X] == '#');
        }

        [Test]
        [TestCase("Day3_Example", 1, 1, ExpectedResult = 2)]
        [TestCase("Day3_Example", 3, 1, ExpectedResult = 7)]
        [TestCase("Day3_Example", 5, 1, ExpectedResult = 3)]
        [TestCase("Day3_Example", 7, 1, ExpectedResult = 4)]
        [TestCase("Day3_Example", 1, 2, ExpectedResult = 2)]
        public long Part2(string inputFile, int advanceX, int advanceY)
        {
            var input = PuzzleInputLoader.GetInput<string>(inputFile).ToArray();

            return CountTreeHits(advanceX, advanceY, input);
        }

        [Test]
        [TestCase("Day3_Example", ExpectedResult = 336)]
        [TestCase("Day3_Input", ExpectedResult = 3638606400)]
        public long Part2_Solution(string inputFile)
        {
            var input = PuzzleInputLoader.GetInput<string>(inputFile).ToArray();

            var slopes = new[]
            {
                new {AdvanceX = 1, AdvanceY = 1},
                new {AdvanceX = 3, AdvanceY = 1},
                new {AdvanceX = 5, AdvanceY = 1},
                new {AdvanceX = 7, AdvanceY = 1},
                new {AdvanceX = 1, AdvanceY = 2}
            };

            return slopes.Select(s => CountTreeHits(s.AdvanceX, s.AdvanceY, input))
                         .LogToConsole()
                         .Aggregate((a, b) =>
                         {
                             var result = a * b; 
                             Console.WriteLine($"{a}x{b}={result}");
                             return result;
                         });
        }

        private static long CountTreeHits(int advanceX, int advanceY, string[] input)
        {
            return Enumerable.Range(0, input.Count())
                             .Where(row => row % advanceY == 0)
                             .Select((row, index) => new {X = (index * advanceX) % input[row].Length, Y = row})
                             .Count(x => input[x.Y][x.X] == '#');
        }
    }
}
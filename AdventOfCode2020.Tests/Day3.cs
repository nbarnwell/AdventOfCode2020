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
    }
}
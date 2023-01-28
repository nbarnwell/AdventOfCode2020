using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace AdventOfCode2020.Tests
{
    public class Day6
    {
        [Test]
        [TestCase("Day6_Example", ExpectedResult = 11)]
        [TestCase("Day6_Input", ExpectedResult = 6532)]
        public int Part1(string inputFile)
        {
            var input     = PuzzleInputLoader.GetInputWhole(inputFile);
            var questions = new[]
            {
                'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'
            };

            return GetGroupedResponses(input).Select(group => string.Join("", group))
                                             .Select(response => questions.Where(response.Contains).Count())
                                             .Sum();
        } 

        [Test]
        [TestCase("Day6_Example", ExpectedResult = 6)]
        [TestCase("Day6_Input", ExpectedResult = 3427)]
        public int Part2(string inputFile)
        {
            var input     = PuzzleInputLoader.GetInputWhole(inputFile);
            var questions = new[]
            {
                'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'
            };

            return GetGroupedResponses(input).Select(group => questions.Count(q => group.All(g => g.Contains(q))))
                                             .Sum();
        } 
        
        private IEnumerable<IEnumerable<string>> GetGroupedResponses(string input)
        {
            return Regex.Split(input, @"\r\n\r\n", RegexOptions.Multiline)
                        .Select(x => Regex.Split(x, @"\r\n", RegexOptions.Multiline));
        }
    }
}
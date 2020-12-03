using System;
using System.Collections.Generic;
using NUnit.Framework;
using System.IO;
using System.Linq;

namespace AdventOfCode2020.Tests
{
    public class PuzzleInputLoader 
    {
        public IEnumerable<T> GetInput<T>(string name)
        {
            var filename = $"{name}.txt";

            return File.ReadLines(filename)
                       .Select(x => Convert.ChangeType(x, typeof(T)))
                       .Cast<T>();
        }
    }

    public class Day1
    {
        [Test]
        public void Part1_Example()
        {
            var input =
                new PuzzleInputLoader().GetInput<int>("Day1_Part1_Example")
                                       .ToArray();

            var result = FixExpenseReport(input);

            Assert.AreEqual(514579, result, "Result");
        }

        [Test]
        public void Part1_Answer()
        {
            var input = 
                new PuzzleInputLoader().GetInput<int>("Day1_Part1_Answer")
                                       .ToArray();

            var result = FixExpenseReport(input);

            Assert.AreEqual(485739, result, "Result");
        }

        private static int FixExpenseReport(int[] input)
        {
            return
                input.SelectMany((x, i) => input.Skip(i + 1).Select(x2 => new {Value1 = x, Value2 = x2}))
                     .Where(x => x.Value1 + x.Value2 == 2020)
                     .Select(x => x.Value1 * x.Value2)
                     .SingleOrDefault();
        }
    }
}
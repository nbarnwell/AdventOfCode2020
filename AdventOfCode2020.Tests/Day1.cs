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
            int result = 0;
            for (int i = 0; i < input.Length; i++)
            {
                int value1 = input[i];
                var subset = input.Skip(i + 1).ToArray();
                //for (int j = i; j < input.Length; j++)
                for (int j = 0; j < subset.Length; j++)
                {
                    var value2 = subset[j];

                    if (value1 + value2 == 2020)
                    {
                        result = value1 * value2;

                        break;
                    }
                }

                if (result > 0)
                {
                    break;
                }
            }

            return result;
        }
    }
}
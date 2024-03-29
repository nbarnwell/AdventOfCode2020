using NUnit.Framework;
using System.Linq;

namespace AdventOfCode2020.Tests
{
    public class Day1
    {
        [Test]
        public void Part1_Example()
        {
            var input =
                PuzzleInputLoader.GetInputLines<int>("Day1_Example")
                                       .ToArray();

            var result = FixExpenseReport(input);

            Assert.AreEqual(514579, result, "Result");
        }

        [Test]
        public void Part1_Solution()
        {
            var input = 
                PuzzleInputLoader.GetInputLines<int>("Day1_Input")
                                       .ToArray();

            var result = FixExpenseReport(input);

            Assert.AreEqual(485739, result, "Result");
        }

        [Test]
        public void Part2_Solution()
        {
            var input = 
                PuzzleInputLoader.GetInputLines<int>("Day1_Input")
                                       .ToArray();

            var result = FixExpenseReportAdvanced(input);

            Assert.AreEqual(161109702, result, "Result");
        }
		
        private static int FixExpenseReport(int[] input)
        {
            // Trick is to get all combinations by listing each number with all the numbers following it
            return
                input.SelectMany((x, i) => input.Skip(i + 1).Select(x2 => new {Value1 = x, Value2 = x2}))
                     .Where(x => x.Value1 + x.Value2 == 2020)
                     .Select(x => x.Value1 * x.Value2)
                     .SingleOrDefault();
        }
		
        private static int FixExpenseReportAdvanced(int[] input)
        {
            // Same as above, but combinations of 3 numbers that add up to 2020
            return
                input.SelectMany(
                         (x1, i1) =>
                             input.Skip(i1 + 1).SelectMany(
                                      (x2, i2) =>
                                          input.Skip(i2 + 1).Select(x3 => new[] {x1, x2, x3}))
                                  .Where(x => x.Sum() == 2020)
                                  .Select(x => x.Aggregate((a1, a2) => a1 * a2)))
                     .SingleOrDefault();
        }
    }
}
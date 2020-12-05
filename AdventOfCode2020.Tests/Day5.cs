using System;
using System.Linq;
using NUnit.Framework;

namespace AdventOfCode2020.Tests
{
    public class Day5
    {
        private class Range
        {
            public int Lower { get; }

            public int Upper { get; }

            public Range(int lower, int upper)
            {
                Lower = lower;
                Upper = upper;
            }

            public Range TakeLowerHalf()
            {
                var range = new Range(Lower, Upper - ((Upper - Lower) / 2) - 1);

                return range;
            }

            public Range TakeUpperHalf()
            {
                var range = new Range(Upper - ((Upper - Lower) / 2), Upper);

                return range;
            }

            public int GetValue()
            {
                return Lower == Upper
                    ? Lower
                    : throw new InvalidOperationException($"Lower ({Lower}) and Upper ({Upper}) do not match");
            }
        }

        [Test]
        [TestCase("FBFBBFFRLR", ExpectedResult = 357)]
        [TestCase("BFFFBBFRRR", ExpectedResult = 567)]
        [TestCase("FFFBBBFRRR", ExpectedResult = 119)]
        [TestCase("BBFFBBFRLL", ExpectedResult = 820)]
        public int Part1_Example(string seatReference)
        {
            return CalculateSeatId(seatReference);
        }

        [Test]
        [TestCase("Day5_Input", ExpectedResult = 991)]
        public int Part1_Solution(string inputFile)
        {
            var input = PuzzleInputLoader.GetInput<string>(inputFile);

            return input.Max(CalculateSeatId);
        }

        private static int CalculateSeatId(string seatReference)
        {
            var row =
                seatReference.Take(7)
                             .Aggregate(new Range(0, 127), (r, c) => c == 'F' ? r.TakeLowerHalf() : r.TakeUpperHalf());

            var column =
                seatReference.Skip(7)
                             .Aggregate(new Range(0, 7), (r, c) => c == 'L' ? r.TakeLowerHalf() : r.TakeUpperHalf());

            int CalculateSeatId(int rowId, int columnId) => rowId * 8 + columnId;

            return CalculateSeatId(row.GetValue(), column.GetValue());
        }
    }
}
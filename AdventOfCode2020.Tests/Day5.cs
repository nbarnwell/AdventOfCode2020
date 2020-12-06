using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using NUnit.Framework;

namespace AdventOfCode2020.Tests
{
    public class Day5
    {
        private class Seat
        {
            public int Row { get; }
            public int Column { get; }
            public bool Taken { get; }

            public int Id => Row * 8 + Column;

            public Seat(int row, int column, bool taken)
            {
                Row    = row;
                Column = column;
                Taken  = taken;
            }

            public static Seat FromBoardingPassReference(string reference)
            {
                var row =
                    reference.Take(7)
                             .Aggregate(new Range(0, 127), (r, c) => c == 'F' ? r.TakeLowerHalf() : r.TakeUpperHalf());

                var column =
                    reference.Skip(7)
                             .Aggregate(new Range(0, 7), (r, c) => c == 'L' ? r.TakeLowerHalf() : r.TakeUpperHalf());

                return new Seat(row.GetValue(), column.GetValue(), true);
            }
        }

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
            return Seat.FromBoardingPassReference(seatReference).Id;
        }

        [Test]
        [TestCase("Day5_Input", ExpectedResult = 991)]
        public int Part1_Solution(string inputFile)
        {
            var input = PuzzleInputLoader.GetInput<string>(inputFile);

            return input.Max(x => Seat.FromBoardingPassReference(x).Id);
        }

        [Test]
        [TestCase("Day5_Input", ExpectedResult = 534)]
        public int Part2(string inputFile)
        {
            var input = PuzzleInputLoader.GetInput<string>(inputFile);

            var takenSeats = input.Select(Seat.FromBoardingPassReference)
                                  .ToDictionary(x => x.Id);

            var seats = Enumerable.Range(0, 127)
                                  .SelectMany(row => Enumerable.Range(0, 7).Select(col => new Seat(row, col, false)))
                                  .Select(x => takenSeats.ContainsKey(x.Id) ? takenSeats[x.Id] : x);

            var mySeat =
                seats.Single(
                    x =>
                        x.Taken == false &&
                        x.Row != 0 &&
                        x.Row != 127 &&
                        takenSeats.ContainsKey(x.Id - 1) &&
                        takenSeats.ContainsKey(x.Id + 1));

            return mySeat.Id;
        }
    }
}
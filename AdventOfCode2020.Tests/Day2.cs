﻿using System;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace AdventOfCode2020.Tests
{
    public class Day2
    {
        [Test]
        [TestCase("Day2_Example", ExpectedResult = 2)]
        [TestCase("Day2_Input", ExpectedResult = 548)]
        public int Check_passwords(string inputFile)
        {
            var input = PuzzleInputLoader.GetInput<string>(inputFile);

            var result =
                input.Select(x => Regex.Match(x, @"^(\d+)-(\d+) (\w): (\w+)$"))
                     .Where(x => x.Success)
                     .Select(x => new
                     {
                         Min            = int.Parse(x.Groups[1].Value),
                         Max            = int.Parse(x.Groups[2].Value),
                         CheckCharacter = char.Parse(x.Groups[3].Value),
                         Password       = x.Groups[4].Value
                     })
                     .Select(x => new
                     {
                         Min                 = x.Min,
                         Max                 = x.Max,
                         CheckCharacter      = x.CheckCharacter,
                         Password            = x.Password,
                         CheckCharacterCount = x.Password.Count(c => c == x.CheckCharacter)
                     })
                     .Count(x => x.CheckCharacterCount >= x.Min && x.CheckCharacterCount <= x.Max);

            return result;
        }
    }
}
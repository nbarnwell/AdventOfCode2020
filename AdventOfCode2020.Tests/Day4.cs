using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace AdventOfCode2020.Tests
{
    public class Day4
    {
        [Test]
        [TestCase("Day4_Example", ExpectedResult = 2)]
        [TestCase("Day4_Input", ExpectedResult   = 190)]
        public int Part1(string inputFile)
        {
            var input = PuzzleInputLoader.GetInput<string>(inputFile);

            var fields         = new[] { "byr", "iyr", "eyr", "hgt", "hcl", "ecl", "pid" };
            var requiredFields = fields.Where(x => x != "cid").ToArray();

            return GetPassports(input).Count(x => x.Keys.Intersect(requiredFields).Count() == requiredFields.Length);
        }

        [Test]
        [TestCase("Day4_Example", ExpectedResult                   = 2)]
        [TestCase("Day4_Example2_InvalidPassports", ExpectedResult = 0)]
        [TestCase("Day4_Example2_ValidPassports", ExpectedResult   = 4)]
        [TestCase("Day4_Input", ExpectedResult                     = 121)]
        public int Part2(string inputFile)
        {
            var input = PuzzleInputLoader.GetInput<string>(inputFile);

            bool IsNumberBetween(string inputValue, int lower, int higher)
            {
                var numeric = int.Parse(inputValue);

                return numeric >= lower && numeric <= higher;
            }

            bool IsHeightBetween(string inputValue, int lower, int higher, string uom)
            {
                var regexResult = Regex.Match(inputValue, @"(\d+)(cm|in)");
                var number      = regexResult.Groups[1].Value;
                var units       = regexResult.Groups[2].Value;

                return regexResult.Success && units == uom && IsNumberBetween(number, lower, higher);
            }

            bool IsHeightInCentimetresBetween(string inputValue, int lower, int higher)
            {
                return IsHeightBetween(inputValue, lower, higher, "cm");
            }

            bool IsHeightInInchesBetween(string inputValue, int lower, int higher)
            {
                return IsHeightBetween(inputValue, lower, higher, "in");
            }

            bool IsValidHairColour(string inputValue)
            {
                return Regex.IsMatch(inputValue, @"#[a-f0-9]{6}");
            }

            bool IsValidEyeColour(string inputValue)
            {
                return Regex.IsMatch(inputValue, @"amb|blu|brn|gry|grn|hzl|oth");
            }

            bool IsValidPassportId(string inputValue)
            {
                return Regex.IsMatch(inputValue, @"^\d{9}$");
            }

            var fields = new Dictionary<string, Predicate<string>>
            {
                { "byr", s => IsNumberBetween(s, 1920, 2002) },
                { "iyr", s => IsNumberBetween(s, 2010, 2020) },
                { "eyr", s => IsNumberBetween(s, 2020, 2030) },
                { "hgt", s => IsHeightInCentimetresBetween(s, 150, 193) || IsHeightInInchesBetween(s, 59, 76) },
                { "hcl", s => IsValidHairColour(s) },
                { "ecl", s => IsValidEyeColour(s) },
                { "pid", s => IsValidPassportId(s) },
                { "cid", s => true }
            };

            Assert.IsTrue(fields["byr"]("2002"));
            Assert.IsFalse(fields["byr"]("2003"));

            Assert.IsTrue(fields["hgt"]("60in"));
            Assert.IsTrue(fields["hgt"]("190cm"));
            Assert.IsFalse(fields["hgt"]("190in"));
            Assert.IsFalse(fields["hgt"]("190"));

            Assert.IsTrue(fields["hcl"]("#123abc"));
            Assert.IsFalse(fields["hcl"]("#123abz"));
            Assert.IsFalse(fields["hcl"]("123abc"));

            Assert.IsTrue(fields["ecl"]("brn"));
            Assert.IsFalse(fields["ecl"]("wat"));

            Assert.IsTrue(fields["pid"]("000000001"));
            Assert.IsFalse(fields["pid"]("0123456789"));

            var requiredFields = fields.Keys.Where(x => x != "cid").ToArray();

            return GetPassports(input).Where(x => x.Keys.Intersect(requiredFields).Count() == requiredFields.Length)
                                      .Where(x => x.All(kvp => fields[kvp.Key](kvp.Value)))
                                      .ForEach(LogPassportData(fields))
                                      .Count();
        }

        private static Action<IDictionary<string, string>, int> LogPassportData(Dictionary<string, Predicate<string>> fields)
        {
            return (x, idx) =>
            {
                Console.WriteLine($"Passport {idx}:");
                var isPassportValid = true;
                foreach (var kvp in x)
                {
                    var isValid = fields[kvp.Key](kvp.Value);

                    if (!isValid)
                    {
                        isPassportValid = false;
                    }

                    var isValidText = isValid ? " (Valid)" : " (Invalid)";
                    Console.WriteLine($" - {kvp.Key} = {kvp.Value} {isValidText}");
                }

                var isPassportValidText = isPassportValid ? " Valid" : " Invalid";
                Console.WriteLine($" Passport is {isPassportValidText}");
            };
        }

        private IEnumerable<IDictionary<string, string>> GetPassports(IEnumerable<string> batchFile)
        {
            var passport = new Dictionary<string, string>();
            foreach (var line in batchFile)
            {
                if (!string.IsNullOrEmpty(line))
                {
                    var kvp = line.Split(' ').Select(x => x.Split(':'));
                    foreach (var item in kvp)
                    {
                        passport.Add(item[0].Trim(), item[1].Trim());
                    }
                }
                else
                {
                    yield return passport;
                    passport = new Dictionary<string, string>();
                }
            }

            if (passport.Values.Any())
            {
                yield return passport;
            }
        }
    }
}
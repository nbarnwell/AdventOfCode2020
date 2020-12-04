using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace AdventOfCode2020.Tests
{
    public class Day4
    {
        [Test]
        [TestCase("Day4_Example", ExpectedResult = 2)]
        [TestCase("Day4_Input", ExpectedResult = 190)]
        public int Part1(string inputFile)
        {
            var input = PuzzleInputLoader.GetInput<string>(inputFile);

            var fields         = new[] {"byr", "iyr", "eyr", "hgt", "hcl", "ecl", "pid"};
            var requiredFields = fields.Where(x => x != "cid").ToArray();

            return GetPassports(input).Count(x => x.Keys.Intersect(requiredFields).Count() == requiredFields.Length);
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
                        passport.Add(item[0], item[1]);
                    }
                }
                else
                {
                    yield return passport;
                    passport = new Dictionary<string, string>();
                }
            }
        }
    }
}
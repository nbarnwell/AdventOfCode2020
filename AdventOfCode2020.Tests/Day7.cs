using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace AdventOfCode2020.Tests
{
    [TestFixture(Description = "Day 7: Handy Haversacks")]
    public class Day7
    {
        [Test]
        [TestCase("Day7_Example", ExpectedResult = 4)]
        public int Part1(string inputFile)
        {
            var input = PuzzleInputLoader.GetInputLines<string>(inputFile);

            var rules = ParseRules(input);

            var graph = rules.ToDictionary(r => r.Source);


            return -1;
        }

        [Test]
        public void Convert_colour_description_to_colour_name()
        {
            Assert.AreEqual("BrightWhite", Regex.Replace("bright white", @"(\b| )(\w)", m => m.Groups[2].Value.ToUpperInvariant()));
        }

        public class BagNode
        {
            public BagColor BagColor { get; }

            public BagNode(BagColor bagColor)
            {
                BagColor = bagColor;
            }
        }

        public class BagRelationship    
        {
            public int Quantity { get; }
            public Relationship Relationship { get; }

            public BagRelationship(int quantity, Relationship relationship)
            {
                Quantity     = quantity;
                Relationship = relationship;
            }
        }

        private IEnumerable<BagRule> ParseRules(IEnumerable<string> input)
        {
            foreach (var item in input)
            {
                var cleaned         = Regex.Replace(item, @"\bbag(s){0,1}\b", "").Replace(".", "");
                var split           = Regex.Split(cleaned, @"\bcontain\b").Select(x => x.Trim()).ToArray();
                var container       = Regex.Replace(split[0], @"(\b| )(\w)", m => m.Groups[2].Value.ToUpperInvariant());

                var containerColour = Enum.Parse<BagColor>(container);

                var relationships =
                    split[1].Split(',').Select(x => x.Trim())
                            .Select(x => Regex.Match(x, @"(\d+) ([\w ]+)"))
                            .Select(x => new BagRule(containerColour, Relationship.Contains,
                                                     Enum.Parse<BagColor>(x.Groups[2].Value),
                                                     int.Parse(x.Groups[1].Value)));

                foreach (var relationship in relationships)
                {
                    yield return relationship;
                }
            }
        }

        public class BagRule
        {
            public BagColor Source { get; }
            public Relationship Relationship { get; }
            public BagColor Destination { get; }

            public BagRule(BagColor source, Relationship relationship, BagColor destination, int quantity)
            {
                Source       = source;
                Relationship = relationship;
                Destination  = destination;
                Quantity     = quantity;
            }

            public int Quantity { get; }
        }

        public enum Relationship
        {
            Contains
        }

        public enum BagColor
        {
            LightRed,
            DarkOrange,
            BrightWhite,
            MutedYellow,
            ShinyGold,
            DarkOlive,
            VibrantPlum,
            FadedBlue,
            DottedBlack
        }
    }
}
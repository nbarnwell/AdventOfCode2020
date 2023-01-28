using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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

            var graph = BuildGraph(input);

            var shinyGold = graph.Nodes.Single(x => x.BagColor == BagColor.ShinyGold);

            var acc = new Accumulator();
            GetNumberOfTopLevelBags(graph, shinyGold, acc);

            return acc.Value;
        }

        private void GetNumberOfTopLevelBags(Graph graph, Node node, Accumulator accumulator)
        {
            var inRelationships = graph.InRelationships(node).ToList();
            if (!inRelationships.Any())
            {
                // This is a top-level container/bag
                accumulator.Increment();
            }
            else
            {
                foreach (var relationship in inRelationships)
                {
                    GetNumberOfTopLevelBags(graph, relationship.From, accumulator);
                }
            }
        }

        private class Accumulator
        {
            private int _value;

            public int Value => _value;

            public void Increment()
            {
                _value++;
            }
        }

        private string ToPascalCase(string input)
        {
            return Regex.Replace(input, @"(\b| )(\w)", m => m.Groups[2].Value.ToUpperInvariant());
        }

        private BagColor ParseBagColor(string input)
        {
            return Enum.Parse<BagColor>(ToPascalCase(input));
        }

        private Graph BuildGraph(IEnumerable<string> input)
        {
            var graph = new Graph();

            foreach (var item in input)
            {
                var cleaned        = Regex.Replace(item, @"\bbag(s){0,1}\b", "").Replace(".", "");
                var split          = Regex.Split(cleaned, @"\bcontain\b").Select(x => x.Trim()).ToArray();
                var containerColor = ParseBagColor(split[0]);
                var containerNode  = graph.AddNode(new Node(containerColor));

                var matches =
                    split[1].Split(',', StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim())
                            .Select(x => Regex.Match(x, @"^(\d+) ([\w ]+)$"))
                            .Where(x => x.Success);

                foreach (var match in matches)
                {
                    var toNode = graph.AddNode(new Node(ParseBagColor(match.Groups[2].Value)));

                    var r = graph.AddRelationship(containerNode, RelationshipType.Contains, toNode);
                    // quantity = int.Parse(x.Groups[1].Value),
                }
            }

            return graph;
        }

        public class Graph
        {
            private readonly IDictionary<BagColor, Node> _nodes = new Dictionary<BagColor, Node>();

            private readonly IDictionary<Tuple<BagColor, RelationshipType, BagColor>, Relationship> _relationships =
                new Dictionary<Tuple<BagColor, RelationshipType, BagColor>, Relationship>();

            public IEnumerable<Node> Nodes => _nodes.Values;

            public Node AddNode(Node node)
            {
                if (_nodes.TryGetValue(node.BagColor, out var found))
                {
                    return found;
                }

                _nodes.Add(node.BagColor, node);
                return node;
            }

            public Relationship AddRelationship(Node from, RelationshipType type, Node to)
            {
                var key = Tuple.Create(from.BagColor, type, to.BagColor);

                if (_relationships.TryGetValue(key, out var found))
                {
                    return found;
                }

                var relationship = new Relationship(from, type, to);
                _relationships.Add(key, relationship);
                return relationship;
            }

            public IEnumerable<Relationship> InRelationships(Node node)
            {
                var result = _relationships.Values.Where(x => x.To == node).ToList();
                return result;
            }
        }

        public class Node
        {
            public BagColor BagColor { get; }

            public Node(BagColor bagColor)
            {
                BagColor = bagColor;
            }

            public override string ToString()
            {
                return BagColor.ToString();
            }

            protected bool Equals(Node other)
            {
                return BagColor == other.BagColor;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((Node)obj);
            }

            public override int GetHashCode()
            {
                return (int)BagColor;
            }
        }

        public class Relationship
        {
            public Node From { get; }
            public RelationshipType Type { get; }
            public Node To { get; }

            public Relationship(Node from, RelationshipType type, Node to)
            {
                From = from;
                Type = type;
                To   = to;
            }
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

    public enum RelationshipType
    {
        Contains
    }
}
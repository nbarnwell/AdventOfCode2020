using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace AdventOfCode2020.Tests
{
    [TestFixture(Description = "Day 7: Handy Haversacks")]
    public class Day7
    {
        [Test]
        [TestCase("Day7_Example", ExpectedResult = 4)]
        [TestCase("Day7_Input", ExpectedResult = 101)]
        public int Part1(string inputFile)
        {
            var input = PuzzleInputLoader.GetInputLines<string>(inputFile);

            var graph = BuildGraph(input);

            var shinyGold = graph.Nodes.Single(x => x.BagColor == BagColor.ShinyGold);

            var result = GetNumberOfTopLevelBags(graph, shinyGold).Distinct().Count();

            return result;
        }

        [Test]
        [TestCase("Day7_Example", ExpectedResult = 32)]
        [TestCase("Day7_Example2", ExpectedResult = 126)]
        [TestCase("Day7_Input", ExpectedResult = 108636)]
        public int Part2(string inputFile)
        {
            var input = PuzzleInputLoader.GetInputLines<string>(inputFile);

            var graph = BuildGraph(input);

            var shinyGold = graph.Nodes.Single(x => x.BagColor == BagColor.ShinyGold);

            var result = GetBagsRequiredToHave(graph, shinyGold).Sum();

            return result;
        }

        [Test]
        [TestCase("Day7_Example")]
        [TestCase("Day7_Example2")]
        [TestCase("Day7_Input")]
        public void Test_PlantUML_Generation(string inputFile)
        {
            var input = PuzzleInputLoader.GetInputLines<string>(inputFile);

            var graph = BuildGraph(input);

            var shinyGold = graph.Nodes.Single(x => x.BagColor == BagColor.ShinyGold);

            var report = new StringBuilder();
            report.AppendLine("@startuml");

            GeneratePlantUml(graph, shinyGold, report);

            report.AppendLine("@enduml");

            Console.WriteLine(report.ToString());
        }

        private IEnumerable<int> GetBagsRequiredToHave(Graph graph, Node node, int multiplier = 1)
        {
            var outRelationships = graph.OutRelationships(node).ToList();
            foreach (var relationship in outRelationships)
            {
                var value = relationship.Quantity * multiplier;
                Console.WriteLine(value);
                yield return value;

                foreach (var result in GetBagsRequiredToHave(graph, relationship.To, value))
                {
                    yield return result;
                }
            }
        }

        private IEnumerable<BagColor> GetNumberOfTopLevelBags(Graph graph, Node node)
        {
            var inRelationships = graph.InRelationships(node).ToList();
            foreach (var relationship in inRelationships)
            {
                yield return relationship.From.BagColor;

                foreach (var result in GetNumberOfTopLevelBags(graph, relationship.From))
                {
                    yield return result;
                }
            }
        }

        private void GeneratePlantUml(Graph graph, Node node, StringBuilder report)
        {
            var relationships = graph.OutRelationships(node).ToList();
            foreach (var relationship in relationships)
            {
                report.AppendLine($"{relationship.From.BagColor} \"1\" *-- \"{relationship.Quantity}\" {relationship.To.BagColor} : contains");

                GeneratePlantUml(graph, relationship.To, report);
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

        private void PrintBagColorNames(IEnumerable<string> input)
        {
            var list = new List<BagColor>();
            foreach (var item in input)
            {
                var cleaned = Regex.Replace(item, @"\bbag(s){0,1}\b", "").Replace(".", "");
                var split   = Regex.Split(cleaned, @"\bcontain\b").Select(x => x.Trim()).ToArray();
                list.Add(ParseBagColor(split[0]));

                var matches =
                    split[1].Split(',', StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim())
                            .Select(x => Regex.Match(x, @"^(\d+) ([\w ]+)$"))
                            .Where(x => x.Success);

                foreach (var match in matches)
                {
                    list.Add(ParseBagColor(match.Groups[2].Value));
                }
            }

            foreach (var color in list.Distinct())
            {
                Console.WriteLine("{0},", color);
            }
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
                    var toNode   = graph.AddNode(new Node(ParseBagColor(match.Groups[2].Value)));
                    var quantity = int.Parse(match.Groups[1].Value);

                    var r = graph.AddRelationship(containerNode, RelationshipType.Contains, quantity, toNode);
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

            public Relationship AddRelationship(Node from, RelationshipType type, int quantity, Node to)
            {
                var key = Tuple.Create(from.BagColor, type, to.BagColor);

                if (_relationships.TryGetValue(key, out var found))
                {
                    return found;
                }

                var relationship = new Relationship(from, type, quantity, to);
                _relationships.Add(key, relationship);
                return relationship;
            }

            public IEnumerable<Relationship> OutRelationships(Node node)
            {
                var result = _relationships.Values.Where(x => x.From == node).ToList();
                return result;
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
            public int Quantity { get; }

            public Relationship(Node from, RelationshipType type, int quantity, Node to)
            {
                From     = from;
                Type     = type;
                Quantity = quantity;
                To       = to;
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
            DottedBlack,
            StripedBeige,
            DullBeige,
            DarkTurquoise,
            DarkBronze,
            PoshTan,
            MirroredTurquoise,
            DimCrimson,
            ClearCrimson,
            DottedBlue,
            StripedGray,
            MutedViolet,
            ClearFuchsia,
            DullViolet,
            BrightCyan,
            DimAqua,
            VibrantSalmon,
            DarkAqua,
            DimBrown,
            DullMagenta,
            DimPlum,
            DarkCoral,
            MirroredWhite,
            PoshTeal,
            DullSilver,
            DullCrimson,
            DullBlue,
            ShinyViolet,
            PlaidMagenta,
            DullGreen,
            MutedWhite,
            PaleRed,
            StripedMaroon,
            StripedTomato,
            ShinyGray,
            DimChartreuse,
            MutedTurquoise,
            PoshYellow,
            DimCyan,
            WavyTan,
            PlaidBeige,
            PoshMaroon,
            MirroredBlue,
            FadedRed,
            DrabRed,
            StripedCyan,
            BrightBlack,
            PoshCyan,
            StripedPurple,
            PaleFuchsia,
            FadedTeal,
            VibrantGray,
            DimBlack,
            MutedLime,
            StripedAqua,
            StripedGold,
            DrabBronze,
            MirroredOrange,
            DullCyan,
            PaleChartreuse,
            ClearBeige,
            DottedOrange,
            MutedPurple,
            PoshOlive,
            DrabMaroon,
            VibrantCrimson,
            VibrantAqua,
            DottedCyan,
            PlaidSilver,
            VibrantBlack,
            PaleOlive,
            BrightTurquoise,
            BrightTeal,
            MutedPlum,
            DimMaroon,
            StripedOrange,
            PaleBronze,
            ShinyBlue,
            DottedTan,
            DimGreen,
            ClearGray,
            MirroredChartreuse,
            MirroredAqua,
            DarkCrimson,
            DimOrange,
            ClearLavender,
            FadedSalmon,
            MutedFuchsia,
            LightViolet,
            FadedWhite,
            ClearPurple,
            PoshBronze,
            DrabCoral,
            FadedOlive,
            DimWhite,
            DrabGray,
            WavyLime,
            DrabChartreuse,
            PlaidLavender,
            ShinyAqua,
            MirroredGold,
            FadedGreen,
            PaleLavender,
            DullPlum,
            ClearViolet,
            DullLime,
            MirroredBronze,
            DottedTurquoise,
            DottedLavender,
            ClearBronze,
            MirroredMagenta,
            DarkTomato,
            PaleGray,
            DottedBronze,
            LightYellow,
            DrabSilver,
            DullOlive,
            BrightAqua,
            MutedSilver,
            StripedTurquoise,
            ClearCyan,
            LightBlack,
            DottedPlum,
            WavyTeal,
            BrightViolet,
            VibrantPurple,
            DottedIndigo,
            PlaidTeal,
            PaleViolet,
            StripedTeal,
            MutedMagenta,
            DarkBrown,
            DarkGreen,
            ShinyGreen,
            StripedCrimson,
            DarkGray,
            BrightGray,
            MutedIndigo,
            DarkLime,
            BrightPlum,
            DullGray,
            VibrantFuchsia,
            DrabBlack,
            StripedChartreuse,
            PalePurple,
            DottedBeige,
            LightTomato,
            VibrantTomato,
            ClearBrown,
            LightOlive,
            ShinyBeige,
            PoshSalmon,
            WavySalmon,
            StripedBlue,
            ShinyCyan,
            DullSalmon,
            ClearChartreuse,
            DrabWhite,
            FadedBlack,
            FadedPlum,
            MutedTeal,
            LightGray,
            DrabMagenta,
            PlaidAqua,
            DimGold,
            MutedBronze,
            WavyCyan,
            VibrantLavender,
            PaleTan,
            PaleAqua,
            StripedBlack,
            ShinyPlum,
            BrightCrimson,
            FadedTurquoise,
            FadedLavender,
            PoshSilver,
            ShinyTeal,
            DrabBeige,
            PaleWhite,
            ClearWhite,
            DottedTomato,
            PlaidBlack,
            ShinyTomato,
            PlaidGreen,
            VibrantGold,
            PlaidTan,
            MirroredBrown,
            MirroredRed,
            LightGold,
            DrabCrimson,
            ShinyBronze,
            DottedLime,
            PaleGreen,
            PlaidOrange,
            DarkTan,
            WavyTomato,
            BrightMagenta,
            VibrantRed,
            MirroredCrimson,
            StripedWhite,
            DottedWhite,
            MutedLavender,
            FadedGold,
            DottedRed,
            ClearMagenta,
            ShinyYellow,
            PoshFuchsia,
            WavyBlue,
            MirroredTeal,
            DullOrange,
            BrightIndigo,
            DarkYellow,
            MirroredViolet,
            LightMaroon,
            DimCoral,
            ShinyRed,
            PalePlum,
            DullGold,
            FadedCyan,
            VibrantViolet,
            WavyBronze,
            WavyOrange,
            DottedOlive,
            StripedMagenta,
            VibrantBeige,
            MirroredCoral,
            VibrantGreen,
            MutedTan,
            VibrantBronze,
            ClearTurquoise,
            MirroredLime,
            FadedOrange,
            LightFuchsia,
            DimTomato,
            MirroredSilver,
            VibrantTeal,
            WavyChartreuse,
            DrabOrange,
            WavyGreen,
            DarkCyan,
            ClearLime,
            DullTeal,
            DrabTomato,
            FadedChartreuse,
            StripedSilver,
            LightBlue,
            DimLime,
            MutedGreen,
            PaleIndigo,
            BrightGreen,
            LightLavender,
            PlaidGold,
            PlaidFuchsia,
            PoshGold,
            DimMagenta,
            WavyCrimson,
            DarkMagenta,
            PaleSalmon,
            DottedAqua,
            DimRed,
            DullWhite,
            PaleCyan,
            MirroredPlum,
            PlaidLime,
            VibrantMagenta,
            PaleBlack,
            ClearGold,
            DarkChartreuse,
            ClearTeal,
            DottedMagenta,
            MutedAqua,
            DullAqua,
            PlaidCrimson,
            MirroredIndigo,
            BrightCoral,
            PaleCrimson,
            DottedFuchsia,
            VibrantCoral,
            DarkLavender,
            PoshBrown,
            VibrantTurquoise,
            ClearBlue,
            MirroredGray,
            PoshTomato,
            DimBeige,
            LightCoral,
            DimTan,
            StripedViolet,
            VibrantMaroon,
            PaleMaroon,
            ShinySilver,
            VibrantCyan,
            PoshChartreuse,
            PoshRed,
            MutedTomato,
            StripedLime,
            DrabYellow,
            PaleSilver,
            DullBlack,
            DimGray,
            DrabAqua,
            WavyRed,
            DrabOlive,
            VibrantBrown,
            DimLavender,
            WavyPlum,
            DimYellow,
            BrightBeige,
            DarkGold,
            DullPurple,
            MirroredTomato,
            WavyOlive,
            ShinyOrange,
            DottedPurple,
            MutedMaroon,
            StripedLavender,
            ShinyCoral,
            MutedCrimson,
            DimOlive,
            StripedBrown,
            DimTeal,
            DrabSalmon,
            FadedTomato,
            ShinyMagenta,
            DarkWhite,
            ClearOlive,
            DrabBrown,
            BrightOlive,
            VibrantChartreuse,
            FadedPurple,
            PaleBeige,
            BrightTomato,
            MutedGray,
            LightCyan,
            DottedGold,
            StripedRed,
            DottedSalmon,
            BrightBronze,
            MutedGold,
            LightChartreuse,
            StripedBronze,
            WavyPurple,
            DrabFuchsia,
            DarkPurple,
            PoshGray,
            DullCoral,
            FadedMaroon,
            MirroredPurple,
            StripedYellow,
            DottedChartreuse,
            FadedCoral,
            MutedCoral,
            DullIndigo,
            ShinyCrimson,
            LightTurquoise,
            BrightPurple,
            DullBrown,
            MirroredLavender,
            LightOrange,
            VibrantWhite,
            ShinyLime,
            BrightChartreuse,
            DottedCrimson,
            WavyTurquoise,
            ClearMaroon,
            FadedSilver,
            DottedTeal,
            MutedCyan,
            WavyYellow,
            DrabCyan,
            WavyViolet,
            PaleTomato,
            ClearOrange,
            LightLime,
            PoshLime,
            ShinyMaroon,
            DullRed,
            DullChartreuse,
            MutedBlack,
            DarkIndigo,
            MirroredCyan,
            VibrantSilver,
            PlaidOlive,
            PlaidChartreuse,
            WavyCoral,
            BrightMaroon,
            PaleLime,
            ShinyOlive,
            PoshBlack,
            VibrantOlive,
            DarkSalmon,
            DarkSilver,
            MirroredSalmon,
            MutedOrange,
            DarkFuchsia,
            ShinyTurquoise,
            DimViolet,
            LightTan,
            PaleCoral,
            DullLavender,
            DottedMaroon,
            DrabViolet,
            PlaidBrown,
            BrightTan,
            PoshViolet,
            FadedIndigo,
            WavyIndigo,
            DullTomato,
            DimSilver,
            PlaidIndigo,
            DimIndigo,
            StripedFuchsia,
            LightBronze,
            VibrantIndigo,
            VibrantLime,
            MirroredFuchsia,
            PlaidMaroon,
            ShinyPurple,
            LightAqua,
            DullBronze,
            ClearTomato,
            DrabBlue,
            BrightRed,
            DarkBlack,
            DottedBrown,
            BrightSilver,
            LightBeige,
            PlaidBronze,
            MirroredGreen,
            MutedOlive,
            PoshIndigo,
            StripedPlum,
            DullTurquoise,
            PoshPurple,
            LightTeal,
            BrightBrown,
            PoshAqua,
            WavyFuchsia,
            PoshMagenta,
            ClearPlum,
            WavySilver,
            PaleTeal,
            ShinySalmon,
            ClearRed,
            MirroredYellow,
            FadedViolet,
            BrightLime,
            DarkPlum,
            VibrantBlue,
            PlaidPurple,
            WavyBeige,
            WavyBrown,
            WavyGray,
            DimBlue,
            MutedBeige,
            PaleMagenta,
            PlaidCoral,
            DrabTeal,
            WavyGold,
            WavyWhite,
            DimFuchsia,
            ShinyWhite,
            DarkBeige,
            PlaidPlum,
            VibrantOrange,
            PlaidBlue,
            PlaidTomato,
            PoshCrimson,
            ShinyLavender,
            DarkRed,
            MutedBrown,
            DrabPurple,
            BrightOrange,
            FadedBronze,
            PoshTurquoise,
            PlaidViolet,
            BrightLavender,
            ClearSalmon,
            LightMagenta,
            DottedGray,
            WavyLavender,
            FadedFuchsia,
            ClearBlack,
            BrightYellow,
            PoshBlue,
            PlaidTurquoise,
            DottedGreen,
            PlaidWhite,
            FadedAqua,
            MutedChartreuse,
            DrabGold,
            FadedMagenta,
            PaleOrange,
            MirroredMaroon,
            BrightBlue,
            DarkViolet,
            StripedGreen,
            PoshLavender,
            PaleGold,
            PaleYellow,
            LightIndigo,
            DottedViolet,
            ClearGreen,
            MirroredBeige,
            LightSalmon,
            LightSilver,
            MutedBlue,
            DimTurquoise,
            FadedLime,
            MutedRed,
            DrabLavender,
            PoshCoral,
            FadedBeige,
            PlaidRed,
            VibrantTan,
            WavyBlack,
            StripedTan,
            StripedCoral,
            LightWhite,
            PoshGreen,
            StripedOlive,
            ClearSilver,
            LightCrimson,
            FadedYellow,
            ShinyBlack,
            ClearIndigo,
            StripedIndigo,
            ClearTan,
            DottedCoral,
            VibrantYellow,
            ClearAqua,
            PlaidGray,
            WavyMagenta,
            LightBrown,
            ShinyTan,
            LightPurple,
            FadedBrown,
            DrabLime,
            PoshBeige,
            DullTan,
            DullFuchsia,
            FadedGray,
            DarkBlue,
            DottedYellow,
            LightPlum,
            PlaidCyan,
            MirroredOlive,
            PaleBrown,
            DrabGreen,
            DimSalmon,
            FadedTan,
            DrabTan,
            DarkMaroon,
            ShinyIndigo,
            DrabPlum,
            WavyMaroon,
            BrightGold,
            DullYellow,
            DarkTeal,
            LightGreen,
            PaleTurquoise,
            BrightFuchsia,
            PoshWhite,
            DrabTurquoise,
            DottedSilver,
            DullMaroon,
            PoshOrange,
            PoshPlum,
            MirroredTan,
            DimPurple,
            MirroredBlack,
            StripedSalmon,
            WavyAqua,
            ShinyChartreuse,
            ClearYellow,
            ShinyFuchsia,
            DrabIndigo,
            PlaidSalmon,
            ShinyBrown,
            DimBronze,
            PlaidYellow,
            FadedCrimson,
            BrightSalmon,
            ClearCoral,
            MutedSalmon,
            PaleBlue
        }
    }

    public enum RelationshipType
    {
        Contains
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode2020.Tests
{
    public static class PuzzleInputLoader 
    {
        public static IEnumerable<T> GetInput<T>(string name)
        {
            var filename = $"{name}.txt";

            return File.ReadLines(filename)
                       .Select(x => Convert.ChangeType(x, typeof(T)))
                       .Cast<T>();
        }
    }
}
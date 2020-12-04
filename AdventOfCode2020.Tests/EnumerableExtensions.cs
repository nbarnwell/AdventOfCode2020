using System;
using System.Collections;
using System.Collections.Generic;

namespace AdventOfCode2020.Tests
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> LogToConsole<T>(this IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                Console.WriteLine(item);

                yield return item;
            }
        }
    }
}
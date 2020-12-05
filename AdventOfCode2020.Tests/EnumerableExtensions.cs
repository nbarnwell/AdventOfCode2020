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
                Console.WriteLine(" - " + item);

                yield return item;
            }
        }

        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach (var item in items)
            {
                action(item);

                yield return item;
            }
        }

        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> items, Action<T, int> action)
        {
            int i = 0;
            foreach (var item in items)
            {
                action(item, i++);

                yield return item;
            }
        }
    }
}
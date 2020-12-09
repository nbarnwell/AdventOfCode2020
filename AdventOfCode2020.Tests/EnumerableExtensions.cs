using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2020.Tests
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> LogToConsole<T>(this IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                if (item is IEnumerable anEnumerable && !typeof(string).IsAssignableFrom(typeof(T)))
                {
                    Console.WriteLine(" - Enumerable:" + item);
                    foreach (var subItem in anEnumerable)
                    {
                        Console.WriteLine("  - " + subItem);
                    }
                }
                else
                {
                    Console.WriteLine(" - " + item);
                }

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
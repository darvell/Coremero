using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coremero.Utilities
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
        {
            return source.OrderBy(x => Guid.NewGuid());
        }

        public static T GetRandom<T>(this IEnumerable<T> source)
        {
            return source.GetRandom(1).Single();
        }

        public static IEnumerable<T> GetRandom<T>(this IEnumerable<T> source, int count)
        {
            return source.Shuffle().Take(count);
        }
    }
}
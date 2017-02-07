using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coremero.Utilities
{
    public static class EnumerableExtensions
    {

        public static T GetRandom<T>(this IEnumerable<T> source)
        {
            return source.Shuffle().FirstOrDefault();
        }

        public static IEnumerable<TSource> Shuffle<TSource>(this IEnumerable<TSource> source)
        {
            return source.OrderBy(x => Guid.NewGuid());
        }


    }
}

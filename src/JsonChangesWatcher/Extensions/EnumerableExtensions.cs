using System;
using System.Collections.Generic;
using System.Linq;

namespace JsonFileWatcher.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> SelectManyRecursive<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> selector)
        {
            foreach (var item in source)
            {
                yield return item;

                var children = selector(item);

                if (children != null)
                {
                    foreach (var child in children.SelectManyRecursive(selector))
                    {
                        yield return child;
                    }
                }
            }
        }

        public static T FirstOrDefaultRecursive<T>(this IEnumerable<T> source, Predicate<T> predicate, Func<T, IEnumerable<T>> childSelector)
        {
            foreach (var item in source)
            {
                bool found = predicate(item);
                if (found)
                {
                    return item;
                }
                else
                {
                    var result =  childSelector(item).FirstOrDefaultRecursive(predicate, childSelector);
                    if(result == null)
                    {
                        continue;
                    }
                    return result;
                }
            }

            return default(T);
        }
    }
}

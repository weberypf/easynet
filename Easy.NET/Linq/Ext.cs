using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Easy.NET.Linq
{
    public static class Ext
    {
        public static void Each<TSource>(this IEnumerable<TSource> array, Action<TSource> action)
        {
            if (array != null)
            {
                IEnumerator<TSource> enumerator = array.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    action(enumerator.Current);
                }
            }
        }

        public static void Each<TSource>(this IEnumerable<TSource> array, Action<TSource, int> action)
        {
            if (array != null)
            {
                IEnumerator<TSource> enumerator = array.GetEnumerator();
                for (int i = 0; enumerator.MoveNext(); i++)
                {
                    action(enumerator.Current, i);
                }
            }
        }

        public static void Each<TSource>(this IEnumerable<TSource> array, Func<TSource, bool> action)
        {
            if (array != null)
            {
                IEnumerator<TSource> enumerator = array.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (!action(enumerator.Current))
                    {
                        break;
                    }
                }
            }
        }

        public static void Each<TSource>(this IEnumerable<TSource> array, Func<TSource, int, bool> action)
        {
            if (array != null)
            {
                IEnumerator<TSource> enumerator = array.GetEnumerator();
                for (int i = 0; enumerator.MoveNext(); i++)
                {
                    if (!action(enumerator.Current, i))
                    {
                        break;
                    }
                }
            }
        }

        public static Dictionary<TKey, TElement> ToDictionarySafe<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
        {
            return Enumerable.ToDictionary<TSource, TKey, TElement>(from p in source
                                                                    group p by keySelector(p) into p
                                                                    select p.First<TSource>(), keySelector, elementSelector);
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Utility
{
    public static class EnumerableExtensions {
        public static TValue TryGet<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key) {
            var value = default(TValue);
            return dict.TryGetValue(key, out value) ? value : default(TValue);
        }
    
        public static IEnumerable<T> UniqueBy<T, TKey>(this IEnumerable<T> enumerable, Func<T, TKey> key) {
            var keys = new HashSet<TKey>();
            return enumerable.Where(item => keys.Add(key(item)));
        }

        public static IEnumerator Catch(this IEnumerator enumerator, Action<Exception> catcher)
        {
            var next = true;
            while (next) 
            {
                try 
                {
                    next = enumerator.MoveNext();
                } 
                catch (Exception e)
                {
                    catcher(e);
                    yield break;
                }

                if (next)
                {
                    yield return enumerator.Current;
                }
            }
        }
    }
}
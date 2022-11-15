using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UntoldGarden
{
    public static class MethodExtensions
    {
        private static System.Random rng = new System.Random();
        public static List<T> Shuffle<T>(this List<T> _list)
        {
            List<T> list = _list;
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
            return list;
        }

        
    }
}

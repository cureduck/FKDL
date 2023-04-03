using System;
using System.Collections.Generic;
using System.Linq;
using Random = System.Random;

namespace Tools
{
    public static class Tools
    {
        public static T[] ChooseRandom<T>(int count, IList<T> list, Random random)
        {
            var s = new T[count];
            int[] selectNumArray = Enumerable.Range(0, list.Count).OrderBy(t => random.Next(10000)).Take(count).ToArray();
            for (int i = 0; i < s.Length; i++)
            {
                s[i] = list.ToList()[selectNumArray[i]];
            }
            return s;
        }
        
        public static T ChooseRandom<T>(IList<T> list, Random random)
        {
            return ChooseRandom<T>(1, list, random)[0];
        }
    }
}


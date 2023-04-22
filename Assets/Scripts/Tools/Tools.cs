using System;
using System.Collections;
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
        
        public static T ChooseRandom<T>(this IList<T> list, Random random) where T : class
        {
            var L = ChooseRandom<T>(1, list, random);
            if (L.Length > 0)
            {
                return L[0];
            }
            else
            {
                return null;
            }
        }


        public static T RandomElementByWeight<T>(this IEnumerable<T> sequence, Func<T, float> weightSelector, Random random) {
            float totalWeight = sequence.Sum(weightSelector);
            // The weight we are after...
            float itemWeightIndex =  (float)random.NextDouble() * totalWeight;
            float currentWeightIndex = 0;

            foreach(var item in from weightedItem in sequence select new { Value = weightedItem, Weight = weightSelector(weightedItem) }) {
                currentWeightIndex += item.Weight;
            
                // If we've hit or passed the weight we are after for this item then it's the one we want....
                if(currentWeightIndex >= itemWeightIndex)
                    return item.Value;
            
            }
            return default(T);
        }
    }
}


using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using Random = System.Random;

namespace Tools
{
    public static class Tools
    {
        public static T[] ChooseRandom<T>(int count, IList<T> list, Random random)
        {
            var s = new T[count];
            int[] selectNumArray =
                Enumerable.Range(0, list.Count).OrderBy(t => random.Next(10000)).Take(count).ToArray();
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

        public static T ChooseRandom<T>(this IEnumerable<T> list, Random random)
        {
            var index = random.Next(0, list.Count());
            return list.ElementAt(index);
        }


        public static T RandomElementByWeight<T>(this IEnumerable<T> sequence, Func<T, float> weightSelector,
            Random random)
        {
            float totalWeight = sequence.Sum(weightSelector);
            // The weight we are after...
            float itemWeightIndex = (float)random.NextDouble() * totalWeight;
            float currentWeightIndex = 0;

            foreach (var item in from weightedItem in sequence
                     select new { Value = weightedItem, Weight = weightSelector(weightedItem) })
            {
                currentWeightIndex += item.Weight;

                // If we've hit or passed the weight we are after for this item then it's the one we want....
                if (currentWeightIndex >= itemWeightIndex)
                    return item.Value;
            }

            return default(T);
        }


        /// <summary>
        /// 计算默认<>号内的数学表达式
        /// </summary>
        /// <param name="s"></param>
        /// <param name="pair">标识计算段落的左右标识符，默认为<></param>
        /// <returns></returns>
        public static string Calculate(this string s, (char left, char right) pair = default)
        {
            if (pair == default) pair = ('<', '>');
            var pattern = $"{pair.left}.{{1,}}{pair.right}";
            var match = Regex.Match(s, pattern);
            var ca = Regex.Replace(match.Value, "[<>]", "");
            object result = new DataTable().Compute(ca, "");
            return Regex.Replace(s, pattern, result.ToString());
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Game;

namespace Tools
{
    public class CustomDictionary<T> : Dictionary<string, T> where T : IRank
    {
        private int _rankLevels;
        public Random Random;

        public int RankLevels
        {
            get
            {
                if (_rankLevels == 0)
                {
                    _rankLevels = Values.GroupBy(skill => skill.Rank).Count();
                    return _rankLevels;
                }
                else
                {
                    return _rankLevels;
                }
            }
        }

        public T[] ChooseRandom(int count = 1)
        {
            return Values.ChooseRandom(count, Random);
        }

        public T[] ChooseRandom(Rank r, int count = 1)
        {
            return Values.Where(rank => rank.Rank == r).ChooseRandom(count, Random);
        }
    }


    public static class DictionaryExtend
    {
        public static TValue RandomGet<TKey, TValue>(this Dictionary<TKey, TValue> dict, Random random)
        {
            return dict.Values.ChooseRandom(random);
        }

        public static int RankLevels<TKey, TValue>(this Dictionary<TKey, TValue> dict) where TValue : IRank
        {
            return dict.Values.GroupBy((rank => rank.Rank)).Count();
        }
    }
}
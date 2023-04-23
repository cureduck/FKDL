using System.Collections.Generic;
using System.Linq;
using Game;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = System.Random;

namespace Tools
{
    public class CustomDictionary<T> : Dictionary<string, T> where T: IRank
    {
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

        private int _rankLevels;
        
        public T[] ChooseRandom(int count = 1)
        {
            return Tools.ChooseRandom(count, Values.ToArray(), Random);
        }
        
        public T[] ChooseRandom(Rank r, int count = 1)
        {
            return Tools.ChooseRandom(count, Values.ToArray().Where(rank => rank.Rank == r).ToArray(), Random);
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
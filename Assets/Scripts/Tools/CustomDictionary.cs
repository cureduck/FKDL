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
        
        public int RankLevels => Values.GroupBy(skill => skill.Rank).Count();
        
        public T[] ChooseRandom(int count = 1)
        {
            return Tools.ChooseRandom(count, Values.ToArray(), Random);
        }
        
        public T[] ChooseRandom(Rank r, int count = 1)
        {
            return Tools.ChooseRandom(count, Values.ToArray().Where(rank => rank.Rank == r).ToArray(), Random);
        }
        
    }
}
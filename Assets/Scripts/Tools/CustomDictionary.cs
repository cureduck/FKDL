using System.Collections.Generic;
using System.Linq;
using Game;

namespace Tools
{
    public class CustomDictionary<T> : Dictionary<string, T> where T: IRank
    {
        public T[] ChooseRandom(int count = 1)
        {
            return Tools.ChooseRandom(count, Values.ToArray());
        }
        
        public T[] ChooseRandom(Rank r, int count = 1)
        {
            return Tools.ChooseRandom<T>(count, Values.ToArray().Where(rank => rank.Rank == r).ToArray());
        }
    }
}
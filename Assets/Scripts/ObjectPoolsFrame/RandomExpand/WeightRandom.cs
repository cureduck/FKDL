using CH.Formula;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CH.RandomExpand
{
    public static class WeightRandom
    {
        public static T GetRandomTarget<T>(Dictionary<T, int> pools)
        {
            int total = 1;
            foreach (var c in pools.Values)
            {
                total += c;
            }
            //Debug.Log("当前总权重:" + total);
            int curRandomIndex = UnityEngine.Random.Range(1, total);
            //Debug.Log("当前随机值:" + curRandomIndex);

            T curRandomValue = default;
            var ge = pools.GetEnumerator();
            while (ge.MoveNext()) 
            {
                curRandomIndex -= ge.Current.Value;
                if (curRandomIndex <= 0)
                {
                    curRandomValue = ge.Current.Key;
                    break;
                }
                
            }
            ge.Dispose();
            //foreach (var c in pools)
            //{

            //}

            return curRandomValue;
        }


    }
}
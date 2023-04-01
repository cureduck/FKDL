using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CH.RandomExpand
{
    public static class  PoolMultipleRandom<T>
    {
        private static List<T> tempT = new List<T>();
        private static List<T> resultT = new List<T>();
        /// <summary>
        /// ��һ���б���������maxgetcount��Ԫ��
        /// </summary>
        /// <param name="curList"></param>
        /// <param name="maxGetCount"></param>
        /// <returns></returns>
        public static T[] GetTargetCountRandom(IEnumerable<T> curList,int maxGetCount) 
        {
            //List<T> x = new List<T>();
            //List<T> y = new List<T>();
            tempT.Clear();
            resultT.Clear();
            foreach (var c in curList)
            {
                tempT.Add(c);
            }

            for (int i = 0; i < maxGetCount; i++)
            {
                if (tempT.Count > 0)
                {
                    int randomIndex = Random.Range(0, tempT.Count);
                    resultT.Add(tempT[randomIndex]);
                    tempT.RemoveAt(randomIndex);
                }
                else 
                {
                    break;
                }
            }

            return resultT.ToArray();
        }
    }
}
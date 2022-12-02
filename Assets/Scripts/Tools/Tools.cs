using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;


public static class Tools
{
    public static T[] ChooseRandom<T>(int count, IList<T> list)
    {
        var s = new T[count];
        int[] selectNumArray = Enumerable.Range(0, list.Count).OrderBy(t => Guid.NewGuid()).Take(count).ToArray();
        for (int i = 0; i < s.Length; i++)
        {
            s[i] = list.ToList()[selectNumArray[i]];
        }
        return s;
    }
        
    public static T ChooseRandom<T>(IList<T> list)
    {
        return list[Random.Range(0, list.Count - 1)];
    }

    
}

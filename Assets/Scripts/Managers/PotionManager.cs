using System;
using System.Collections.Generic;
using Csv;
using Game;
using Sirenix.OdinInspector;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Serialization;

namespace Managers
{
    public class PotionManager : Singleton<PotionManager>
    {
        public Dictionary<string, Potion> Lib;

        public Dictionary<Rank, List<Potion>> Ordered;
        
        private void Start()
        {
            Load();
        }


        private void Load()
        {
            Lib = new Dictionary<string, Potion>();
            var csv = File.ReadAllText(Paths.PotionDataPath);
            Ordered = new Dictionary<Rank, List<Potion>>();

            foreach (var line in CsvReader.ReadFromText(csv))
            {
                try
                {
                    var potion = Line2Potion(line);
                    Lib[potion.Id] = potion;

                    if (!Ordered.ContainsKey(potion.Rank))
                        Ordered[potion.Rank] = new List<Potion>();
                    Ordered[potion.Rank].Add(potion);
                }
                catch (Exception)
                {
                    Debug.Log("potion load failed");
                }
            }
            
            FuncMatch();
        }


        private static Potion Line2Potion(ICsvLine line)
        {
            return new Potion
            {
                Id = line[0].ToLower().Replace(" ", ""),
                Rank = (Rank) int.Parse(line[1]),
                Param1 = float.Parse(line[2]),
            };
        }
        
        
        public string[] Roll(Rank roll, int count)
        {
            var s = new string[count];
            int[] selectNumArray = Enumerable.Range(0, Ordered[roll].Count).OrderBy(t => Guid.NewGuid()).Take(count).ToArray();
            for (int i = 0; i < s.Length; i++)
            {
                s[i] = Ordered[roll].ToList()[selectNumArray[i]].Id;
            }
            return s;
        }


        private void FuncMatch()
        {
            foreach (var v in Lib.Values)
            {
                v.Fs = new Dictionary<Timing, MethodInfo>();
            }

            foreach (var method in typeof(PotionData).GetMethods())
            {
                var attr = method.GetCustomAttribute<EffectAttribute>();

                if ((attr != null) && (Lib.ContainsKey(attr.id.ToLower())))
                {
                    Lib[attr.id.ToLower().Trim()].Fs[attr.timing] = method;
                }
            }
        }

    }
}
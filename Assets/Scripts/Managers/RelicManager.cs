using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Csv;
using Game;
using I2.Loc;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using Random = System.Random;


namespace Managers
{
    [ExecuteAlways]
    public class RelicManager : Singleton<RelicManager>
    {
        public Dictionary<string, Relic> Lib;
        public Dictionary<Rank, LinkedList<Relic>> Ordered;

        private void Start()
        {
            Load();
        }


        [Button]
        private void Load()
        {
            Lib = new Dictionary<string, Relic>();
            Ordered = new Dictionary<Rank, LinkedList<Relic>>();

            var csv = File.ReadAllText(Paths.RelicDataPath, Encoding.UTF8);
            
            foreach (var line in CsvReader.ReadFromText(csv))
            {
                try
                {
                    var skill = Line2Relic(line);
                    Lib[skill.Id] = skill;
                    if (!Ordered.ContainsKey(skill.Rank))
                        Ordered[skill.Rank] = new LinkedList<Relic>();
                    Ordered[skill.Rank].AddLast(skill);
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                    Debug.Log("skill load failed");
                }
            }
            FuncMatch();
        }


        [Button]
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
        
        
        private static Relic Line2Relic(ICsvLine line)
        {
            return new Relic()
            {
                Id = line["id"].ToLower(),
                Rank = (Rank)int.Parse(line["rank"]),
            };
        }

        private void FuncMatch()
        {
            foreach (var v in Lib.Values)
            {
                v.Fs = new Dictionary<Timing, MethodInfo>();
            }
            
            foreach (var method in typeof(RelicData).GetMethods())
            {
                var attr = method.GetCustomAttribute<EffectAttribute>();

                /*if ((attr!=null)||(attr.activated == false))
                {
                    continue;
                }*/
                
                if (attr!=null)
                {
#if UNITY_EDITOR
                    if (!Lib.ContainsKey(attr.id.ToLower()))
                    {
                        var sk = new Relic
                        {
                            Id = attr.id.ToLower(),
                            Fs = new Dictionary<Timing, MethodInfo>(),
                            Rank = Rank.Uncommon,
                        };
                        Lib[attr.id.ToLower()] = sk;
                        Ordered[Rank.Uncommon].AddLast(sk);
                    }
#endif

                    Lib[attr.id.ToLower()].Fs[attr.timing] = method;
                }
            }
        }
    }
}
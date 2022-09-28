using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Csv;
using Game;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = System.Random;


namespace Managers
{
    public class SkillManager : Singleton<SkillManager>
    {
        public Dictionary<string, Skill> Lib;
        public Dictionary<Rank, LinkedList<Skill>> Ordered;

        private void Start()
        {
            Load();
        }


        private void Load()
        {
            Lib = new Dictionary<string, Skill>();
            Ordered = new Dictionary<Rank, LinkedList<Skill>>();

            var csv = File.ReadAllText(Paths.SkillDataPath);
            
            foreach (var line in CsvReader.ReadFromText(csv))
            {
                try
                {
                    var skill = Line2Skill(line);
                    Lib[skill.Id] = skill;
                    if (!Ordered.ContainsKey(skill.Rank))
                        Ordered[skill.Rank] = new LinkedList<Skill>();
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
        
        
        private static Skill Line2Skill(ICsvLine line)
        {
            return new Skill
            {
                Id = line[0].ToLower(),
                Rank = (Rank) int.Parse(line[1]),
                Positive = line[2] == "TRUE",
                MaxLv = int.Parse(line[3]),
                Param1 = int.Parse(line[4] != ""?line[4]:"0"),
                Param2 = int.Parse(line[5] != ""?line[5]:"0"),
            };
        }

        private void FuncMatch()
        {
            foreach (var v in Lib.Values)
            {
                v.Fs = new Dictionary<Timing, MethodInfo>();
            }
            
            foreach (var method in typeof(SkillData).GetMethods())
            {
                var attr = method.GetCustomAttribute<EffectAttribute>();

                if (attr!=null)
                {
#if UNITY_EDITOR
                    if (!Lib.ContainsKey(attr.id.ToLower()))
                    {
                        var sk = new Skill
                        {
                            Id = attr.id.ToLower(),
                            Fs = new Dictionary<Timing, MethodInfo>(),
                            Rank = Rank.Uncommon,
                            MaxLv = 3,
                            Param1 = 1,
                            Positive = false
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
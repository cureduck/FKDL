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
using Tools;
using UnityEngine;
using Random = System.Random;


namespace Managers
{
    [ExecuteAlways]
    public class SkillManager : Singleton<SkillManager>
    {
        public CustomDictionary<Skill> Lib;
        public Dictionary<Rank, LinkedList<Skill>> Ordered;

        private void Start()
        {
            Load();
        }


        [Button]
        private void Load()
        {
            Lib = new CustomDictionary<Skill>();
            Ordered = new Dictionary<Rank, LinkedList<Skill>>();

            var csv = File.ReadAllText(Paths.SkillDataPath, Encoding.UTF8);
            
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

        public Skill GetSkillByStringID(string id) 
        {
            if (id == null) return null;

            Skill curSkill;
            //foreach (var c in Lib)
            //{
            //    Debug.Log(c.Value.Id);
            //}
            Lib.TryGetValue(id, out curSkill);
            return curSkill;
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
            int.TryParse(line["cd"], out var cooldown);
            int.TryParse(line["cost"], out var cost);
            
            
            Debug.Log(line["id"]);
            
            return new Skill
            {
                Id = line["id"].ToLower(),
                Rank = (Rank) int.Parse(line["Rarity"]),
                Pool = line["Pool"],
                Positive = bool.Parse(line["Positive"]),
                BattleOnly = bool.Parse(line["BattleOnly"]),
                MaxLv = int.Parse(line["MaxLv"]),
                Param1 = float.Parse(line["P1"] != ""?line["P1"]:"0"),
                Param2 = float.Parse(line["P2"] != ""?line["P2"]:"0"),
                CostInfo = new CostInfo()
                {
                    Value = cost,
                    CostType = line["CostType"] == "" ? CostType.Mp : CostType.Hp,
                },
                Cooldown = cooldown,
                //Description = line[10]
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

                /*if ((attr!=null)||(attr.activated == false))
                {
                    continue;
                }*/
                
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
                    
                    if (Lib.TryGetValue(attr.id.ToLower(), out var v))
                    {
                        v.Fs[attr.timing] = method;
                    }
                    
                }
            }
        }
    }
}
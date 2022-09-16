using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Csv;
using Game;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = System.Random;


namespace Managers
{
    public class SkillManager : Singleton<SkillManager>
    {
        public Dictionary<string, Skill> Skills;
        public Dictionary<Rank, LinkedList<Skill>> OrderedSkills;
        
        private static string path => Path.Combine(Application.dataPath, "GameData", "Skills", "Skills.csv");
        

        private void Start()
        {
            Load();
        }


        private void Load()
        {
            Skills = new Dictionary<string, Skill>();
            OrderedSkills = new Dictionary<Rank, LinkedList<Skill>>();

            var csv = File.ReadAllText(path);
            
            foreach (var line in CsvReader.ReadFromText(csv))
            {
                try
                {
                    var skill = Line2Skill(line);
                    Skills[skill.Id] = skill;
                    if (!OrderedSkills.ContainsKey(skill.Rank))
                        OrderedSkills[skill.Rank] = new LinkedList<Skill>();
                    OrderedSkills[skill.Rank].AddLast(skill);
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                    Debug.Log("skill load failed");
                }
            }
        }


        [Button]
        public string[] Roll(Rank roll, int count)
        {
            var s = new string[count];
            int[] selectNumArray = Enumerable.Range(0, OrderedSkills[roll].Count).OrderBy(t => Guid.NewGuid()).Take(count).ToArray();
            for (int i = 0; i < s.Length; i++)
            {
                s[i] = OrderedSkills[roll].ToList()[selectNumArray[i]].Id;
            }
            return s;
        }
        
        
        private static Skill Line2Skill(ICsvLine line)
        {
            return new Skill
            {
                Id = line[0],
                Rank = (Rank) int.Parse(line[1]),
                Positive = line[2] == "TRUE",
                MaxLv = int.Parse(line[3]),
                Param1 = int.Parse(line[4] != ""?line[4]:"0"),
                Param2 = int.Parse(line[5] != ""?line[5]:"0"),
            };
        }
    }
}
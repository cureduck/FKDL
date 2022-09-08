using System;
using System.Collections.Generic;
using System.IO;
using Csv;
using Game;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Managers
{
    public class SkillManager : Singleton<SkillManager>
    {
        public Dictionary<string, Skill> Skills;
        private static string path => Application.dataPath + "/GameData/Skills/Skills.csv";


        private void Start()
        {
            Load();
        }


        private void Load()
        {
            Skills = new Dictionary<string, Skill>();
            var csv = File.ReadAllText(path);


            foreach (var line in CsvReader.ReadFromText(csv))
            {
                try
                {
                    var skill = Line2Skill(line);
                    Skills[skill.Id] = skill;
                }
                catch (Exception)
                {
                    Debug.Log("skill load failed");
                }
            }
        }


        private static Skill Line2Skill(ICsvLine line)
        {
            return new Skill
            {
                Id = line[0],
                Rank = (Rank) int.Parse(line[1]),
                Positive = line[2] == "TRUE",
                MaxLv = int.Parse(line[3]),
                Param1 = int.Parse(line[4] == ""?line[4]:"0"),
                Param2 = int.Parse(line[5] == ""?line[5]:"0"),
            };
        }
    }
}
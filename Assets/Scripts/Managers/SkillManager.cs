using System;
using System.Collections.Generic;
using System.IO;
using Csv;
using Game;
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
                    var skill = new Skill
                    {
                        Id = line[0],
                        Description = line[1],
                        MaxLv = int.Parse(line[2])
                    };
                    Skills[skill.Id] = skill;
                }
                catch (Exception)
                {
                    Debug.Log("skill load failed");
                }
            }
        }
    }
}
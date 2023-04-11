using System.Collections.Generic;
using System.Reflection;
using Managers;
using Newtonsoft.Json;
using UnityEngine;

namespace Game
{
    public class Skill : CsvData
    {
        public string Pool;
        public bool Positive;
        public bool BattleOnly;
        public int MaxLv;

        public string Description;
        
        public float Param1;
        public float Param2;
        public int Cooldown;
        public CostInfo CostInfo;

        public Skill(Rank rank, string id, Sprite icon) : base(rank, id, icon)
        {
            
        }
        

        public Skill(Rank rank, string id) : base(rank, id)
        {
        }

        public Skill()
        {
        }
    }
}
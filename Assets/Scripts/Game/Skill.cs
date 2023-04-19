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
        
        public float Param1;
        public float Param2;
        public int Cooldown;
        public CostInfo CostInfo;

        public readonly HashSet<Timing> AlwaysActiveTiming;
        
        
        public Skill(Rank rank, string id, Sprite icon) : base(rank, id, icon)
        {
            AlwaysActiveTiming = new HashSet<Timing>();
        }
        

        public Skill(Rank rank, string id) : base(rank, id)
        {
        }

        public Skill()
        {
        }
    }
}
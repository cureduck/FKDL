using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using UnityEngine;

namespace Game
{
    public class Skill : IRank
    {
        [JsonIgnore] public Sprite Icon;
        public string Id;
        public string Pool;
        public bool Positive;
        public bool BattleOnly;
        public int MaxLv;
        public Rank Rank { get; set; }

        public string Description;
        
        public float Param1;
        public float Param2;
        public int Cooldown;
        public CostInfo CostInfo;

        [JsonIgnore] public Dictionary<Timing, MethodInfo> Fs;
    }
}
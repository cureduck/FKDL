using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using UnityEngine;

namespace Game
{
    public class Skill
    {
        [JsonIgnore] public Texture Icon;
        public string Id;
        public bool Positive;
        public int MaxLv;
        public Rank Rank;

        public string Description;
        
        public float Param1;
        public float Param2;
        public int Cooldown;

        [JsonIgnore] public Dictionary<Timing, MethodInfo> Fs;
    }
}
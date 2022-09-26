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

        public int Param1;
        public int Param2;

        [JsonIgnore] public Dictionary<Timing, MethodInfo> Fs;
    }
}
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using UnityEngine;

namespace Game
{
    public class Relic
    {
        [JsonIgnore] public Texture Icon;
        public string Id;
        public string Description;
        public Rank Rank;

        [JsonIgnore] public Dictionary<Timing, MethodInfo> Fs;
    }
}
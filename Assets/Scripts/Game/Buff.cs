﻿using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using UnityEngine;

namespace Game
{
    public class Buff
    {
        [JsonIgnore] public Texture Icon;
        
        public string Id;

        [JsonIgnore] public Dictionary<Timing, MethodInfo> Fs;
    }
}
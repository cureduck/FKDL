﻿using System;
using Newtonsoft.Json;
using Sirenix.Utilities;
using UnityEngine;

namespace Game
{
    public struct SkillData
    {
        public string Id;
        public int CurLv;

        [JsonIgnore] public bool IsEmpty => Id.IsNullOrWhitespace();
    }
}
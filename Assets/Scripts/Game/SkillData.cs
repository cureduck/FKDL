using System;
using Managers;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace Game
{
    public struct SkillData
    {
        public string Id;
        public int CurLv;

        [JsonIgnore] public bool IsEmpty => Id.IsNullOrWhitespace();
        
        
        
        [SkillEffect("Furious", Timing.Attack)]
        public Attack Furious(Attack attack, FighterData fighter, FighterData defender)
        {
            fighter.Status.CurHp += 1;
            Activate?.Invoke();
            return attack;
        }

        [ShowInInspector] public event Action Activate;
    }
}
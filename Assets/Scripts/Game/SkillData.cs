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


        [SkillEffect("Burst", Timing.Equip)]
        public void BurstEquip(FighterData fighter)
        {
            fighter.Status.PAtk += Bp.Param1 * CurLv;
        }
        
        [SkillEffect("Burst", Timing.LvUp)]
        public void BurstLvUp(FighterData fighter)
        {
            fighter.Status.PAtk += Bp.Param1 * CurLv;
        }
        

        [JsonIgnore] public Skill Bp => SkillManager.Instance.Skills[Id];
        [ShowInInspector] public event Action Activate;
    }
}
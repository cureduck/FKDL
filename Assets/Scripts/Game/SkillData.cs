﻿using System;
using System.Reflection;
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
        public int Local;

        [JsonIgnore] public bool IsEmpty => Id.IsNullOrWhitespace();

        [JsonIgnore] public Action<FighterData> OnLvUp;
        [JsonIgnore] public Action<FighterData> OnUnEquip;


        public void LvUp(FighterData fighter, int lv = 1)
        {
            CurLv += lv;
            OnLvUp?.Invoke(fighter);
        }

        public void UnEquip(FighterData fighter)
        {
            OnUnEquip?.Invoke(fighter);
        }
        
        [JsonIgnore] public Skill Bp => SkillManager.Instance.Lib[Id];
        [ShowInInspector] public event Action Activate;
        
        
        
        #region 具体技能

        [SkillEffect("furious", Timing.OnAttack)]
        public Attack Furious(Attack attack, FighterData fighter, FighterData defender)
        {
            fighter.Status.CurHp += 1;
            Activate?.Invoke();
            return attack;
        }


        [SkillEffect("burst", Timing.OnEquip)]
        public void BurstEquip(FighterData fighter)
        {
            fighter.Status.PAtk += Bp.Param1 * CurLv;
        }
        
        [SkillEffect("burst", Timing.OnLvUp)]
        public void BurstLvUp(FighterData fighter)
        {
            fighter.Status.PAtk += Bp.Param1;
        }

        [SkillEffect("burst", Timing.OnUnEquip)]
        public void BurstUnEquip(FighterData fighter)
        {
            fighter.Status.PAtk -= Bp.Param1 * CurLv;
        }
        
        #endregion
        

        


    }
}
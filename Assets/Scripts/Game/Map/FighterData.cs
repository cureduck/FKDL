﻿using System;
using System.Collections.Generic;
using System.Reflection;
using Managers;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game
{
    public class FighterData : MapData
    {
        public BattleStatus Status;
        public SkillData[] Skills;

        
        [JsonIgnore, NonSerialized, ShowInInspector]
        public LinkedList<Func<Attack, FighterData, FighterData, Attack>> AttackModifiers;
        [JsonIgnore, NonSerialized, ShowInInspector]
        public LinkedList<Func<Result, FighterData, FighterData,  Result>> SettleModifiers;
        [JsonIgnore, NonSerialized, ShowInInspector]
        public LinkedList<Func<Attack, FighterData, FighterData, Attack>> DefendModifiers;


        public FighterData()
        {
            AttackModifiers = new LinkedList<Func<Attack, FighterData, FighterData, Attack>>();
            SettleModifiers = new LinkedList<Func<Result, FighterData, FighterData, Result>>();
            DefendModifiers = new LinkedList<Func<Attack, FighterData, FighterData, Attack>>();
        }
        
        
        public Attack ForgeAtk()
        {
            return new Attack
            {
                PAtk = Status.PAtk,
                MAtk = Status.MAtk,
                CAtk = 0,
                Id = null
            };
        }


        public Result Suffer(Attack attack)
        {
            Status.CurHp -= attack.PAtk - Status.PDef + attack.MAtk - Status.MDef + attack.CAtk;
            
            return new Result
            {
                PAtk = attack.PAtk - Status.PDef,
                MAtk = attack.MAtk - Status.MDef,
                CAtk = attack.CAtk
            };
        }
        
        
        public void OnEquip(SkillData skill)
        {
            foreach (var pi in typeof(SkillData).GetMethods())
            {
                var msg = pi.GetCustomAttribute<SkillEffectAttribute>();
                if ((msg == null)||(msg.id != skill.Id)) continue;

                switch (msg.timing)
                {
                    case Timing.Attack:
                        var f = (Func<Attack, FighterData, FighterData, Attack>) 
                            pi.CreateDelegate(typeof(Func<Attack, FighterData, FighterData, Attack>), skill);
                        AttackModifiers.AddLast(f);
                        break;
                    case Timing.Settle:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
        
        public void OnUnEquip(SkillData skill)
        {
            foreach (var pi in typeof(SkillData).GetMethods())
            {
                var msg = pi.GetCustomAttribute<SkillEffectAttribute>();
                if ((msg == null)||(msg.id != skill.Id)) continue;

                switch (msg.timing)
                {
                    case Timing.Attack:
                        var f = (Func<Attack, FighterData, FighterData, Attack>)
                            pi.CreateDelegate(typeof(Func<Attack, FighterData, FighterData, Attack>), this);
                        AttackModifiers.Remove(f);
                        break;
                    case Timing.Settle:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}
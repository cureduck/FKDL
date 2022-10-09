using System;
using System.Reflection;
using Managers;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game
{
    public struct SkillData : IEffectContainer
    {
        public string Id;
        public int CurLv;
        public int Local;

        [JsonIgnore] public bool IsEmpty => Id.IsNullOrWhitespace();

        public event Action<FighterData> OnLvUp;
        public event Action<FighterData> OnUnEquip;


        
        public void LvUp(FighterData fighter, int lv = 1)
        {
            CurLv += lv;

            if (Bp.Fs.TryGetValue(Timing.OnLvUp, out var method))
            {
                method.Invoke(this, new object[]{fighter});
            }
            OnLvUp?.Invoke(fighter);
        }

        public void UnEquip(FighterData fighter)
        {
            if (Bp.Fs.TryGetValue(Timing.OnUnEquip, out var method))
            {
                method.Invoke(this, new object[]{fighter});
            }
            OnUnEquip?.Invoke(fighter);
        }




        [JsonIgnore] public Skill Bp => SkillManager.Instance.Lib[Id.ToLower()];
        [ShowInInspector] public event Action Activate;
        
        
        
        public bool MayAffect(Timing timing, out int priority)
        {
            if (Bp.Fs.TryGetValue(timing, out var f))
            {
                priority = f.GetCustomAttribute<EffectAttribute>().priority;
                return true;
            }
            else
            {
                priority = 0;
                return false;
            }        
        }

        public T Affect<T>(Timing timing, object[] param)
        {
            return (T) Bp.Fs[timing].Invoke(this, param);
        }
        
        public static SkillData Empty => new SkillData();


        #region 具体技能

        [Effect("furious", Timing.OnAttack)]
        public Attack Furious(Attack attack, FighterData fighter, FighterData defender)
        {
            fighter.OnRecover(new BattleStatus{CurHp = CurLv}, HealType.Blood);
            Activate?.Invoke();
            return attack;
        }


        [Effect("burst", Timing.OnEquip)]
        public void BurstEquip(FighterData fighter)
        {
            fighter.Strengthen(new BattleStatus{PAtk = CurLv});
        }
        
        [Effect("burst", Timing.OnLvUp)]
        public void BurstLvUp(FighterData fighter)
        {
            fighter.Strengthen(new BattleStatus{PAtk = 1});
        }

        [Effect("burst", Timing.OnUnEquip)]
        public void BurstUnEquip(FighterData fighter)
        {
            fighter.Status.PAtk -= Bp.Param1 * CurLv;
        }


        [Effect("Light", Timing.OnHeal)]
        public BattleStatus LightHeal(BattleStatus modify, HealType healType, FighterData fighter)
        {
            Activate?.Invoke();
            return modify * (1 + CurLv);
        }

        [Effect("BarBlood", Timing.OnStrengthen)]
        public BattleStatus BarBlood(BattleStatus modify, FighterData fighter)
        {
            if (Random.value < CurLv * .4f)
            {
                fighter.Strengthen(new BattleStatus{PAtk = 1});
                Activate?.Invoke();
            }
            return modify;
        }
        
        #endregion
    }
}
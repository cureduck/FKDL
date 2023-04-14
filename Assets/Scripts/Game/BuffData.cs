using System;
using System.Collections;
using System.Reflection;
using Managers;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game
{
    public class BuffData : IEffectContainer, ICloneable
    {
        public string Id { get; private set; }
        [ShowInInspector] public int CurLv { get; private set; }
        [JsonIgnore] public Buff Bp => BuffManager.Instance.TryGetById(Id.ToLower(), out var buff) ? buff : null;
        



        public void StackChange(int value)
        {
            CurLv += value;
            if (CurLv == 0)
            {
                Remove();
            }

            if (CurLv < 0)
            {
                if (Bp.OppositeBp != null)
                {
                    Id = Bp.OppositeId;
                    CurLv = -CurLv;
                }
                else
                {
                    Remove();
                }
            }
        }
        
        
        public BuffData(string id, int curLv)
        {
            Id = id.ToLower();
            CurLv = curLv;
        }

        public BuffData()
        {
        }

        public event Action Removed;

        public void Remove()
        {
            Removed?.Invoke();
        }
        
        public event Action Activate;

        public bool MayAffect(Timing timing, out int priority)
        {
            if (Bp == null)
            {
                priority = 0;
                return false;
            }
            
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

        public void Affect(Timing timing, object[] param)
        {
            Bp.Fs[timing].Invoke(this, param);
        }

        public override string ToString()
        {
            return $"{Id}:{CurLv}";
        }


        #region 具体效果

        [Effect("PPlus", Timing.OnAttack, priority = -4)]
        public Attack Anger(Attack attack, FighterData f1, FighterData f2)
        {
            attack.PAtk += CurLv;
            CurLv -= 1;
            Activate?.Invoke();
            return attack;
        }

        [Effect("MPlus", Timing.OnAttack, priority = -4)]
        public Attack Surging(Attack attack, FighterData f1, FighterData f2)
        {
            attack.MAtk += CurLv;
            CurLv -= 1;
            Activate?.Invoke();
            return attack;
        }
        
        [Effect("PMinus", Timing.OnAttack, priority = -4)]
        public Attack PMinus(Attack attack, FighterData f1, FighterData f2)
        {
            attack.PAtk -= CurLv;
            CurLv -= 1;
            Activate?.Invoke();
            return attack;
        }

        [Effect("MMinus", Timing.OnAttack, priority = -4)]
        public Attack MMinus(Attack attack, FighterData f1, FighterData f2)
        {
            attack.MAtk -= CurLv;
            CurLv -= 1;
            Activate?.Invoke();
            return attack;
        }
        
        
        [Effect("Poison", Timing.OnPreAttack, priority = -4)]
        public Attack Poison(Attack attack, FighterData f1, FighterData f2)
        {
            f1.Suffer(new Attack{MAtk = CurLv});
            CurLv -= 1;
            Activate?.Invoke();
            return attack;
        }
        
        [Effect("Bellow", Timing.OnAttack, priority = -4)]
        public Attack Bellow(Attack attack, FighterData f1, FighterData f2)
        {
            if (attack.PAtk > 0) attack.PAtk += CurLv;
            if (attack.MAtk > 0) attack.MAtk += CurLv;
            if (attack.CAtk > 0) attack.CAtk += CurLv;
            
            CurLv = 0;
            Activate?.Invoke();
            return attack;
        }        
        #endregion

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
using System;
using System.Collections;
using System.Reflection;
using Managers;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UI;
using UnityEngine;
using UnityEngine.Assertions.Must;

namespace Game
{
    public class BuffData : IEffectContainer, ICloneable, IActivated
    {
        [ShowInInspector] public string Id { get; private set; }
        [ShowInInspector] public int CurLv { get; private set; }
        [JsonIgnore] public Buff Bp => BuffManager.Instance.TryGetById(Id, out var buff)? buff : null;


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
        
        [JsonConstructor]
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
        
        public event Action Activated;

        public bool MayAffect(Timing timing, out int priority)
        {
            if (Id.IsNullOrWhitespace()||(Bp == null))
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
        private Attack Anger(Attack attack, FighterData f1, FighterData f2)
        {
            attack.PAtk += CurLv;
            CurLv -= 1;
            Activated?.Invoke();
            return attack;
        }

        [Effect("MPlus", Timing.OnAttack, priority = -4)]
        private Attack Surging(Attack attack, FighterData f1, FighterData f2)
        {
            attack.MAtk += CurLv;
            CurLv -= 1;
            Activated?.Invoke();
            return attack;
        }
        
        [Effect("PMinus", Timing.OnAttack, priority = -4)]
        private Attack PMinus(Attack attack, FighterData f1, FighterData f2)
        {
            attack.PAtk -= CurLv;
            CurLv -= 1;
            Activated?.Invoke();
            return attack;
        }

        [Effect("MMinus", Timing.OnAttack, priority = -4)]
        private Attack MMinus(Attack attack, FighterData f1, FighterData f2)
        {
            attack.MAtk -= CurLv;
            CurLv -= 1;
            Activated?.Invoke();
            return attack;
        }
        
        
        [Effect("Poison", Timing.OnPreAttack, priority = -4)]
        private void Poison(FighterData f1, FighterData f2)
        {
            f1.Defend(new Attack(mAtk: CurLv), null);
            CurLv -= 1;
            Activated?.Invoke();
        }
        
        [Effect("Bellow", Timing.OnAttack, priority = -4)]
        private Attack Bellow(Attack attack, FighterData f1, FighterData f2)
        {
            if (attack.PAtk > 0) attack.PAtk += CurLv;
            if (attack.MAtk > 0) attack.MAtk += CurLv;
            if (attack.CAtk > 0) attack.CAtk += CurLv;
            
            CurLv = 0;
            Activated?.Invoke();
            return attack;
        }
        #endregion

        public object Clone()
        {
            return MemberwiseClone();
        }

    }
}
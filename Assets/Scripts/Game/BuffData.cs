using System;
using System.Collections;
using System.Reflection;
using Managers;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game
{
    public class BuffData : IEffectContainer
    {
        public string Id;
        public int CurLv;

        [JsonIgnore] public bool Positive => CurLv > 0;
        [JsonIgnore] public Buff Bp => BuffManager.Instance.Lib.TryGetValue(Id.ToLower(), out var buff) ? buff : null;


        public event Action Removed;

        public void Remove()
        {
            Removed?.Invoke();
        }
        
        public event Action Activate;


        #region 具体效果

        [Effect("HpPotion", Timing.OnAttack)]
        public Attack HpPotion(Attack attack, FighterData f1, FighterData f2)
        {
            Debug.Log("buff loaded");
            Activate?.Invoke();
            return attack;
        }
        
        #endregion

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
    }
}
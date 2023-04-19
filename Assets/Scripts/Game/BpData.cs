using System;
using System.Reflection;
using Managers;
using Newtonsoft.Json;
using UnityEngine;

namespace Game
{
    public abstract class BpData<TT> where TT : CsvData
    {
        public string Id;
        [JsonIgnore] public abstract TT Bp { get; }
        
        
        public virtual bool MayAffect(Timing timing, out int priority)
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
            try
            {
                return (T) Bp.Fs[timing].Invoke(this, param);
            }
            catch (Exception e)
            {
                Debug.LogError($"{Id} Affect Failed");
                return (T)param[0];
            }
        }

        public void Affect(Timing timing, object[] param)
        {
            try
            {
                Bp.Fs[timing].Invoke(this, param);
            }
            catch (Exception e)
            {
                Debug.LogError($"{Id} Affect Failed");
            }
        }
    }
}
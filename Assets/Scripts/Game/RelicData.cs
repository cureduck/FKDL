using System;
using System.Reflection;
using Managers;
using Newtonsoft.Json;
using UI;
using UnityEngine.Assertions.Must;

namespace Game
{
    public class RelicData : IEffectContainer, IActivated
    {
        public readonly string Id;
        public int Counter;

        
        [JsonConstructor]
        public RelicData(string id, int counter = 0)
        {
            Id = id;
            Counter = counter;
        }
        
        public RelicData(Relic relic, int counter = 0)
        {
            Id = relic.Id;
            Counter = counter;
        }
        
        public Relic Bp => RelicManager.Instance.GetById(Id);
        
        public bool MayAffect(Timing timing, out int priority)
        {
            if (!RelicManager.Instance.ContainsKey(Id.ToLower()))
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
            return Id;
        }

        #region Effects

        [Effect("test", Timing.OnApplied, 900)]
        private BuffData Test(BuffData buff, FighterData fighterData)
        {
            var tmp = buff;
            tmp.StackChange(1);
            return tmp;
        }
        
        
        [Effect("test2", Timing.OnApplied, 900)]
        private BuffData Test2(BuffData buff, FighterData fighterData)
        {
            var tmp = buff;
            tmp.StackChange(-1);
            return tmp;
        }
        
        [Effect("test3", Timing.OnApplied, 900)]
        private BuffData Test3(BuffData buff, FighterData fighterData)
        {
            var tmp = buff;
            tmp.StackChange(-1);
            return tmp;
        }

        [Effect("test4", Timing.OnSetCoolDown, 900)]
        private SkillData Test4(SkillData skill, FighterData fighterData)
        {
            skill.Cooldown += GameDataManager.Instance.SecondaryData.CurGameRandom.Next(-3, 2);
            return skill;
        }
        
        #endregion

        public event Action Activated;
    }
}
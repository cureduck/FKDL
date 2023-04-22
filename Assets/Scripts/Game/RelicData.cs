using System;
using System.Linq;
using System.Reflection;
using Managers;
using Newtonsoft.Json;
using Sirenix.Utilities;
using Tools;
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

        private SecondaryData SData => GameDataManager.Instance.SecondaryData;

        #region Effects

        [Effect("wjmy", Timing.OnApplied, 900)]
        private BuffData Test(BuffData buff, FighterData fighterData)
        {
            var tmp = buff;
            tmp.StackChange(1);
            Activated?.Invoke();
            return tmp;
        }
        
        
        [Effect("kwzx", Timing.OnApplied, 900)]
        private BuffData Test2(BuffData buff, FighterData fighterData)
        {
            var tmp = buff;
            tmp.StackChange(-1);
            Activated?.Invoke();
            return tmp;
        }
        
        [Effect("lmzw", Timing.OnAttack, 900)]
        private Attack lmzw(Attack attack, FighterData player, FighterData enemy)
        {
            attack.Combo += 1;
            var skill = player.Skills.ActiveSkills().Where((data => data.Bp.Positive))
                .ToArray().ChooseRandom(SData.CurGameRandom);
            skill?.BonusCooldown(-(int)Bp.Param1);
            Activated?.Invoke();
            return attack;
        }
        
        
        [Effect("wdys", Timing.OnKill, 900)]
        private Attack Test3(Attack attack, FighterData player, FighterData enemy)
        {
            if (((PlayerData)player).LuckyChance > SData.CurGameRandom.NextDouble())
            {
                player.RandomStrengthen(1);
                ((PlayerData) player).LuckyChance -= .01f;
                Activated?.Invoke();
            }
            return attack;
        }
        
        
        [Effect("xyf", Timing.BeforeAttack)]
        private Attack xyf(Attack attack, FighterData player, FighterData enemy)
        {
            if (((PlayerData)player).LuckyChance > SData.CurGameRandom.NextDouble())
            {
                player.ApplySelfBuff(new BuffData("buffer", 1));
                Activated?.Invoke();
            }
            return attack;
        }

        [Effect("wcsl", Timing.OnSetCoolDown, 900)]
        private SkillData Test4(SkillData skill, FighterData fighterData)
        {
            skill.CooldownLeft += SData.CurGameRandom.Next(-3, 2);
            Activated?.Invoke();
            return skill;
        }
        
        [Effect("jzhb", Timing.OnAttack)]
        private Attack zjhb(Attack attack, FighterData player, FighterData enemy)
        {
            if (attack.Kw.IsNullOrWhitespace())
            {
                Counter += 1;
                Activated?.Invoke();
            }
            else
            {
                attack.Multi += Counter * Bp.Param1;
                Counter = 0;
                Activated?.Invoke();
            }
            return attack;
        }
        
        
        
        
        [Effect("tzns", Timing.OnAttack)]
        private Attack tzns(Attack attack, FighterData player, FighterData enemy)
        {
            if (!attack.Kw.IsNullOrWhitespace())
            {
                attack.CostInfo.Value += (int)Bp.Param1;
                player.Strengthen(new BattleStatus(){MaxMp = 1});
                Activated?.Invoke();
            }
            return attack;
        }

        [Effect("zqms", Timing.OnGet)]
        private void zqms(FighterData fighter)
        {
            ((PlayerData) fighter).LuckyChance += .4f;
        }
        
        
        
        #endregion

        public event Action Activated;
    }
}
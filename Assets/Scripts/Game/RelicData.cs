using System;
using System.Linq;
using System.Reflection;
using Managers;
using Newtonsoft.Json;
using Sirenix.Utilities;
using Tools;
using UI;

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

        private SecondaryData SData => GameDataManager.Instance.SecondaryData;

        public event Action Activated;

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
            return (T)Bp.Fs[timing].Invoke(this, param);
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
                ((PlayerData)player).LuckyChance -= .01f;
                Activated?.Invoke();
            }

            return attack;
        }


        [Effect("xyf", Timing.BeforeAttack)]
        private void xyf(FighterData player, FighterData enemy)
        {
            if (((PlayerData)player).LuckyChance > SData.CurGameRandom.NextDouble())
            {
                player.ApplySelfBuff(new BuffData("buffer", 1));
                Activated?.Invoke();
            }
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
                player.Strengthen(new BattleStatus() { MaxMp = 1 });
                Activated?.Invoke();
            }

            return attack;
        }


        /// <summary>
        /// cost P1% hp, upgrade a random skill
        /// </summary>
        /// <param name="fighterData"></param>
        /// <returns></returns>
        [Effect("rxss", Timing.OnMarch)]
        private FighterData rxss(FighterData fighterData)
        {
            fighterData.Cost(CostInfo.HpCost((int)(fighterData.Status.CurHp * Bp.Param1 / 100)));
            ((PlayerData)fighterData).UpgradeRandomSkill(SkillData.CanBeUpgrade, out _);
            Activated?.Invoke();
            return fighterData;
        }


        /// <summary>
        /// upgrade a random potion when march
        /// </summary>
        /// <param name="fighterData"></param>
        /// <returns></returns>
        [Effect("klyp", Timing.OnMarch)]
        private FighterData klyp(FighterData fighterData)
        {
            ((PlayerData)fighterData).Potions.Where((data => data.CanBeUpgrade)).ChooseRandom(SData.CurGameRandom)
                .Upgrade(out _);
            Activated?.Invoke();
            return fighterData;
        }


        /// <summary>
        /// double the physical damage taken, half the magic damage taken
        /// </summary>
        /// <param name="attack"></param>
        /// <param name="player"></param>
        /// <param name="enemy"></param>
        /// <returns></returns>
        [Effect("xwzy", Timing.OnDefendSettle, priority = 10)]
        private Attack xwzy(Attack attack, FighterData player, FighterData enemy)
        {
            attack.PDmg /= 2;
            attack.MDmg *= 2;

            if (attack.PDmg > 0 || attack.MDmg > 0)
            {
                Activated?.Invoke();
            }

            return attack;
        }

        /// <summary>
        /// improve luck by P1
        /// </summary>
        /// <param name="fighter"></param>
        [Effect("zqms", Timing.OnGet)]
        private void zqms(FighterData fighter)
        {
            ((PlayerData)fighter).LuckyChance += Bp.Param1;
        }

        /// <summary>
        /// add P1 to multi if engaging
        /// </summary>
        /// <param name="attack"></param>
        /// <param name="player"></param>
        /// <param name="enemy"></param>
        /// <returns></returns>
        [Effect("zdjq", Timing.OnAttack, priority = 5)]
        private Attack zdjq(Attack attack, FighterData player, FighterData enemy)
        {
            var p = (PlayerData)player;
            if (p.Engaging)
            {
                attack.Multi += (int)Bp.Param1;
                Activated?.Invoke();
            }

            return attack;
        }


        /// <summary>
        /// heals P1 hp when lv up
        /// </summary>
        /// <param name="skill"></param>
        /// <param name="fighter"></param>
        /// <returns></returns>
        [Effect("kbyh", Timing.OnLvUp)]
        private SkillData kbyh(SkillData skill, FighterData fighter)
        {
            Activated?.Invoke();
            fighter.Heal(BattleStatus.HP((int)Bp.Param1), "kbyh");

            return skill;
        }

        /// <summary>
        /// rcover P1 hp when attack
        /// </summary>
        /// <param name="attack"></param>
        /// <param name="player"></param>
        /// <param name="enemy"></param>
        /// <returns></returns>
        [Effect("ftzx", Timing.OnAttack)]
        private Attack ftzx(Attack attack, FighterData player, FighterData enemy)
        {
            player.Recover(BattleStatus.HP((int)Bp.Param1), enemy, "ftzx");
            Activated?.Invoke();
            return attack;
        }


        /// <summary>
        /// overheal to mp
        /// </summary>
        /// <param name="modifier"></param>
        /// <param name="fighter"></param>
        /// <param name="kw"></param>
        /// <returns></returns>
        [Effect("jmzg", Timing.OnHeal, priority = 30)]
        private BattleStatus jmzg(BattleStatus modifier, FighterData fighter, string kw)
        {
            if (modifier.CurHp <= 0 || fighter.Status.CurMp == fighter.Status.MaxMp) return modifier;

            var overHeal = modifier.CurHp + fighter.Status.CurHp - fighter.Status.MaxHp;
            if (overHeal > 0)
            {
                modifier.CurHp -= overHeal;
                fighter.Heal(new BattleStatus(curMp: overHeal));
                Activated?.Invoke();
            }

            return modifier;
        }

        /// <summary>
        /// overmax mp to hp
        /// </summary>
        [Effect("pgly", Timing.OnHeal, priority = 30)]
        private BattleStatus pgly(BattleStatus modifier, FighterData fighter, string kw)
        {
            if (modifier.CurMp <= 0 || fighter.Status.CurHp == fighter.Status.MaxHp) return modifier;

            var overHeal = modifier.CurMp + fighter.Status.CurMp - fighter.Status.MaxMp;
            if (overHeal > 0)
            {
                modifier.CurMp -= overHeal;
                fighter.Heal(new BattleStatus(curHp: overHeal));
                Activated?.Invoke();
            }

            return modifier;
        }


        [Effect("gjb", Timing.OnGet)]
        private void gjb(FighterData fighter)
        {
            ((PlayerData)fighter).Status.Gold += 300;
        }

        #endregion
    }
}
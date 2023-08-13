using System;
using System.Collections.Generic;
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
        public static Dictionary<string, string> ProfRelic = new Dictionary<string, string>()
        {
            { "ALC", "jjxs" },
            { "MAG", "xywz" },
            { "KNI", "qsqz" },
            { "ASS", "aq" },
            { "BAR", "zwzr" },
            { "CUR", "lw" }
        };

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
            if (!attack.IsCommonAttack) return attack;
            attack.Combo += 1;
            var skill = player.Skills.Where((data => data.IsValid && data.Bp.Positive))
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
            var p = (PlayerData)player;

            if (p.LuckyChance > SData.CurGameRandom.NextDouble())
            {
                p.LuckyChance -= .03f;
                player.ApplySelfBuff(new BuffData("buffer", 1));
                Activated?.Invoke();
            }
            else
            {
                p.LuckyChance += .01f;
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
            ((PlayerData)fighterData).UpgradeRandomSkill(out _, true);
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


        [Effect("zqms", Timing.OnMarch)]
        private void zqms2(FighterData fighter)
        {
            ((PlayerData)fighter).LuckyChance += Bp.Param1;
            Activated?.Invoke();
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
            fighter.Heal(BattleStatus.Hp((int)Bp.Param1), "kbyh");

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
            player.Recover(BattleStatus.Hp((int)Bp.Param1), enemy, "ftzx");
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

        /// <summary>
        /// can remove obsidian
        /// </summary>
        /// <param name="mapData"></param>
        /// <param name="fighterData"></param>
        /// <returns></returns>
        [Effect("kgtq", Timing.OnReact)]
        private MapData kgtq(MapData mapData, FighterData fighterData)
        {
            if (mapData is ObsidianSaveData)
            {
                Activated?.Invoke();
                MapData.Destroy(mapData);
            }

            return mapData;
        }


        [Effect("xsjz", Timing.OnGet)]
        private void xsjz(FighterData fighter)
        {
            Activated?.Invoke();
            ((PlayerData)fighter).AddSkillSlot();
        }

        /// <summary>
        /// 下层时，获得1瓶技能药，但是失去所有金币
        /// </summary>
        /// <returns></returns>
        [Effect("edzh", Timing.OnMarch)]
        private FighterData edzh(FighterData fighterData)
        {
            var player = (PlayerData)fighterData;
            player.Status.Gold = 0;
            player.TryTakePotion("skillpotion+", out _);
            Activated?.Invoke();
            return player;
        }

        /// <summary>
        /// 获得时随机技能升级P1次
        /// </summary>
        [Effect("pdlh", Timing.OnGet)]
        private void pdlh(FighterData fighter)
        {
            var player = (PlayerData)fighter;
            for (var i = 0; i < Bp.Param1; i++)
            {
                player.UpgradeRandomSkill(out _);
            }
        }

        /// <summary>
        /// 斩杀首领时：获得99层启示
        /// </summary>
        /// <returns></returns>
        [Effect("xszq", Timing.OnKill)]
        private Attack xszq(Attack attack, FighterData player, FighterData enemy)
        {
            if (((EnemySaveData)enemy).Rank == Rank.Rare)
            {
                Activated?.Invoke();
                ((PlayerData)player).AppliedBuff(BuffData.Divinity(99));
            }

            return attack;
        }

        /// <summary>
        /// 商店不可重复进入，交互时获得P1金币
        /// </summary>
        /// <param name="mapData"></param>
        /// <param name="fighterData"></param>
        [Effect("slxk", Timing.OnReact)]
        private void slxk(MapData mapData, FighterData fighterData)
        {
            if (mapData is ShopSaveData)
            {
                Activated?.Invoke();
                ((PlayerData)fighterData).Gain((int)Bp.Param1);
                MapData.Destroy(mapData);
            }
        }

        /// <summary>
        /// 进入下层时:无消耗使用所有非战主动技能一次
        /// </summary>
        /// <param name="fighterData"></param>
        /// <returns></returns>
        [Effect("hxzg", Timing.OnMarch)]
        private FighterData hxzg(FighterData fighterData)
        {
            var player = (PlayerData)fighterData;
            foreach (var skill in player.Skills)
            {
                if (skill.IsValid && !skill.Bp.BattleOnly)
                {
                    player.CastNonAimingSkill(skill, 0f);
                }
            }

            Activated?.Invoke();
            return player;
        }

        /// <summary>
        /// 技能攻击时：当前所有技能，每剩余1点冷却，攻击倍率+{[P1]}
        /// </summary>
        /// <returns></returns>
        [Effect("myrg", Timing.OnAttack, priority = 4)]
        private Attack myrg(Attack attack, FighterData player, FighterData enemy)
        {
            if (attack.IsCommonAttack) return attack;
            var playerData = (PlayerData)player;
            var bonus = player.Skills.Sum((data => data.CooldownLeft));
            attack.Multi += bonus * Bp.Param1;
            Activated?.Invoke();
            return attack;
        }


        /// <summary>
        /// 防御时:每有一种诅咒,敌人的物防-{[P1]}
        /// </summary>
        /// <param name="attack"></param>
        /// <param name="player"></param>
        /// <param name="enemy"></param>
        /// <returns></returns>
        [Effect("hz", Timing.OnDefendSettle)]
        private Attack hz(Attack attack, FighterData player, FighterData enemy)
        {
            var playerData = (PlayerData)player;
            var curseCount = playerData.Buffs.Count(buff => buff.Bp.BuffType == BuffType.Curse);
            enemy.Status.PDef -= (int)(curseCount * Bp.Param1);
            Activated?.Invoke();
            return attack;
        }

        /// <summary>
        /// 获得诅咒时:获得{[P1]}次主职业的属性升级
        /// </summary>
        /// <param name="buff"></param>
        /// <param name="fighter"></param>
        /// <returns></returns>
        [Effect("ztsl", Timing.OnApplied)]
        private BuffData ztsl(BuffData buff, FighterData fighter)
        {
            if (buff.Bp.BuffType == BuffType.Curse)
            {
                var player = (PlayerData)fighter;
                for (var i = 0; i < Bp.Param1; i++)
                {
                    player.Strengthen(BattleStatus.GetProfessionUpgrade(PlayerData.ProfInfo[0]));
                }
            }

            return buff;
        }


        /// <summary>
        /// 移除诅咒时:每移除一种诅咒,最大生命值提升{[P1]}%
        /// </summary>
        /// <param name="buff"></param>
        /// <param name="fighter"></param>
        /// <returns></returns>
        [Effect("lqfz", Timing.OnPurify)]
        private BuffData lqfz(BuffData buff, FighterData fighter)
        {
            if (buff.Bp.BuffType == BuffType.Curse)
            {
                var player = (PlayerData)fighter;
                player.Status.MaxHp += (int)(player.Status.MaxHp * Bp.Param1 / 100);
            }

            return buff;
        }


        /// <summary>
        /// 获得祝福时:获得{[P1]}层连击
        /// </summary>
        /// <param name="buff"></param>
        /// <param name="fighter"></param>
        /// <returns></returns>
        [Effect("xtt", Timing.OnApplied)]
        private BuffData xtt(BuffData buff, FighterData fighter)
        {
            if (buff.Bp.BuffType == BuffType.Blessing)
            {
                var player = (PlayerData)fighter;
                player.AppliedBuff(BuffData.Vigor((int)Bp.Param1));
            }

            return buff;
        }

        /// <summary>
        /// 斩杀首领时:获得随机一种诅咒,当前所有技能升级1次
        /// </summary>
        /// <param name="attack"></param>
        /// <param name="player"></param>
        /// <param name="enemy"></param>
        /// <returns></returns>
        [Effect("jnp", Timing.OnKill, priority = 3)]
        private Attack jnp(Attack attack, FighterData player, FighterData enemy)
        {
            if (((EnemySaveData)enemy).Rank == Rank.Rare)
            {
                Activated?.Invoke();
                var playerData = (PlayerData)player;
                playerData.AppliedBuff(((BuffManager)(BuffManager.Instance)).GetRandomBuffData(BuffType.Curse));
                foreach (var skill in playerData.Skills)
                {
                    if (skill.IsValid)
                    {
                        playerData.Upgrade(skill);
                    }
                }
            }

            return attack;
        }

        [Effect("qsqz", Timing.OnDefendSettle)]
        private Attack qsqz(Attack attack, FighterData player, FighterData enemy)
        {
            Counter += attack.SumDmg;
            if (Counter >= Bp.Param1)
            {
                Activated?.Invoke();
                player.ApplySelfBuff(BuffData.Sturdy((int)(Counter / Bp.Param1)));
                Counter = (int)(Counter % Bp.Param1);
            }

            return attack;
        }

        [Effect("xywz", Timing.OnGetSkillCost)]
        private CostInfo xywz(CostInfo costInfo, SkillData skill, FighterData player, bool istry)
        {
            if (costInfo.CostType == CostType.Mp)
            {
                costInfo.Value -= 1;
                Activated?.Invoke();
            }

            return costInfo;
        }

        [Effect("xywz", Timing.OnGet)]
        private void xywz2(FighterData player)
        {
            if (player is PlayerData p)
            {
                p.TryTakeSkill("hq_mag", out _);
            }
        }


        [Effect("jjxs", Timing.OnUsePotion)]
        private PotionData jjxs(PotionData potion, FighterData player)
        {
            Activated?.Invoke();
            player.Heal(Bp.Param1 / 100);
            return potion;
        }

        [Effect("aq", Timing.OnAttack)]
        private Attack aq(Attack attack, FighterData player, FighterData enemy)
        {
            Counter += 1;
            if (Counter > Bp.Param1)
            {
                Activated?.Invoke();
                Counter = 0;
                attack.Combo += 1;
            }

            return attack;
        }


        [Effect("zwzr", Timing.OnMarch)]
        private void zwzr(FighterData player)
        {
            if (player.Status.PAtk > player.Status.MAtk)
            {
                player.Strengthen(new BattleStatus(mAtk: (int)Bp.Param1));
            }
            else
            {
                player.Strengthen(new BattleStatus(pAtk: (int)Bp.Param1));
            }
        }


        [Effect("lw", Timing.OnApply)]
        private BuffData lw(BuffData buff, FighterData player)
        {
            if (buff.Bp.BuffType == BuffType.Negative)
            {
                Activated?.Invoke();
                foreach (var b in player.Buffs)
                {
                    if (b.Bp.BuffType == BuffType.Curse)
                    {
                        b.StackChange(-b.CurLv + 1);
                    }
                }
            }

            return buff;
        }


        [Effect("jbp", Timing.OnReact)]
        private MapData jbp(MapData map, FighterData player)
        {
            if (map is SupplySaveData supply)
            {
                Activated?.Invoke();
                player.Gain((int)Bp.Param1);
            }

            return map;
        }


        /// <summary>
        /// 获得一个其他职业的初始遗物
        /// </summary>
        /// <param name="player"></param>
        [Effect("hyhz", Timing.OnGet)]
        private void hyhz(FighterData player)
        {
            if (player is PlayerData p)
            {
                var cur_relic = ProfRelic[SData.Profs[0]];
                var relics = ProfRelic.Values.ToList();
                relics.Remove(cur_relic);
                var relic = relics.ChooseRandom(SData.CurGameRandom);

                p.TryTakeRelic(relic, out _);
            }
        }


        /// <summary>
        /// 攻击时：每有一种buff，恢复P1生命值
        /// </summary>
        /// <param name="attack"></param>
        /// <param name="player"></param>
        /// <param name="enemy"></param>
        /// <returns></returns>
        [Effect("gcgz", Timing.OnAttack)]
        private Attack gcgz(Attack attack, FighterData player, FighterData enemy)
        {
            if (attack.IsEmpty()) return attack;
            var buffs = player.Buffs.Where(b => b.Bp.BuffType == (BuffType.Buff)).ToList();

            player.Heal((int)(buffs.Count * Bp.Param1));

            return attack;
        }

        /// <summary>
        /// 物防、魔防提升时：额外提升1点
        /// </summary>
        /// <returns></returns>
        [Effect("wgxl", Timing.OnStrengthen)]
        private BattleStatus wgxl(BattleStatus status, FighterData player)
        {
            if (status.PDef > 0)
            {
                Activated?.Invoke();
                status.PDef += 1;
            }

            if (status.MDef > 0)
            {
                Activated?.Invoke();
                status.MDef += 1;
            }

            return status;
        }

        [Effect("zdjy", Timing.OnStrengthen)]
        private BattleStatus zdjy(BattleStatus status, FighterData player)
        {
            if (status.PAtk > 0)
            {
                Activated?.Invoke();
                status.PAtk += 1;
            }

            if (status.MAtk > 0)
            {
                Activated?.Invoke();
                status.MAtk += 1;
            }

            return status;
        }

        [Effect("fszh", Timing.OnStrengthen)]
        private BattleStatus fszh(BattleStatus status, FighterData player)
        {
            if (status.MaxHp > 0)
            {
                Activated?.Invoke();
                status.MaxHp += (int)Bp.Param1;
            }

            if (status.MaxMp > 0)
            {
                Activated?.Invoke();
                status.MaxMp += (int)Bp.Param1;
            }

            return status;
        }

        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Managers;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using Tools;
using UnityEngine;
using static Unity.Mathematics.math;

namespace Game
{
    public class SkillData : BpData<Skill>, IEffectContainer, ICloneable
    {
        public int CooldownLeft;

        public int Counter;
        public int CurLv;

        public int InitCoolDown;
        public bool Sealed = false;

        [JsonIgnore, Obsolete("Use IsValid instead")]
        public bool IsEmpty => Id.IsNullOrWhitespace();

        [JsonIgnore] public override Skill Bp => SkillManager.Instance.GetById(Id.ToLower());

        public static SkillData Empty => new SkillData();
        private float Usual => Bp.Param1 + Bp.Param2 * CurLv;
        private float Unusual => Bp.Param1 - Bp.Param2 * CurLv;

        private bool InBattle => GameManager.Instance.InBattle;
        private SecondaryData SData => GameDataManager.Instance.SecondaryData;

        [JsonIgnore] private MapData CurrentMapData => GameManager.Instance.Focus.Data;
        [JsonIgnore] public bool IsValid => !Id.IsNullOrWhitespace() && Bp != null;

        [JsonIgnore] public bool InCoolDown => CooldownLeft > 0;


        public object Clone()
        {
            return MemberwiseClone();
        }


        public override bool MayAffect(Timing timing, out int priority)
        {
            if (Sealed && !Bp.AlwaysActiveTiming.Contains(timing))
            {
                priority = 0;
                return false;
            }

            return base.MayAffect(timing, out priority);
        }


        /// <summary>
        /// 将技能转化为另一个技能
        /// </summary>
        /// <param name="id"></param>
        private void SwitchTo(string id)
        {
            Id = id.ToLower();
        }

        public static bool CanBreakOut(SkillData skill)
        {
            return skill != null && !skill.Id.IsNullOrWhitespace() && skill.Bp.MaxLv != 1;
        }

        public static bool CanUpgrade(SkillData skill)
        {
            return CanBreakOut(skill) && skill.CurLv < skill.Bp.MaxLv;
        }


        public void Load(Skill skill, int lv = 1)
        {
            CurLv = lv;
            Id = skill.Id;
            CooldownLeft = 0;
            Sealed = skill.BattleOnly;
            Counter = 0;
        }


        /// <summary>
        /// 设置冷却时间
        /// </summary>
        /// <param name="cd"></param>
        public void SetCooldown(int cd = default)
        {
            CooldownLeft = cd;
            CooldownLeft = CooldownLeft < 0 ? 0 : CooldownLeft;
            InitCoolDown = CooldownLeft;
        }

        /// <summary>
        /// 冷却时间减少
        /// </summary>
        /// <param name="cd"></param>
        public void BonusCooldown(int cd)
        {
            CooldownLeft -= cd;
            CooldownLeft = CooldownLeft < 0 ? 0 : CooldownLeft;
        }


        public bool CanCast(out Info info, bool autoBroadcast)
        {
            var reasons = new List<FailureReason>();
            if (!Bp.Positive)
            {
                reasons.Add(FailureReason.SkillPassive);
            }

            if ((Bp.BattleOnly) && (!GameManager.Instance.InBattle))
            {
                reasons.Add(FailureReason.NoTarget);
            }

            if (CooldownLeft > 0)
            {
                reasons.Add(FailureReason.SkillNotReady);
            }

            if (reasons.Count == 0)
            {
                info = new SuccessInfo();
                return true;
            }
            else
            {
                info = new FailureInfo(reasons, autoBroadcast);
                return false;
            }
        }

        [ShowInInspector] public event Action Activated;

        public override string ToString()
        {
            return Id;
        }

        #region 正式技能

        [Effect("YWLZ_ALC", Timing.SkillEffect)]
        private void BrewPotion(FighterData fighter)
        {
            var p = PotionManager.Instance.RollT(Rank.Normal).First();
            ((PlayerData)fighter).TryTakeOffer(new Offer(p), out _);
            //SetCooldown();
        }


        [Effect("JSLC_ALC", Timing.SkillEffect)]
        private void MetalTrans(FighterData fighter)
        {
            if (InBattle)
            {
                var b = fighter.ApplyBuff(new BuffData("pminus", (int)Bp.Param1));
                fighter.Enemy.AppliedBuff(b);
            }
            else
            {
                fighter.ApplySelfBuff(new BuffData("pplus", (int)Bp.Param1));
            }
        }


        [Effect("XRLC_ALC", Timing.SkillEffect)]
        private void FleshTrans(FighterData fighter)
        {
            if (InBattle)
            {
                var enemy = fighter.Enemy;
                if (enemy is EnemySaveData e && e.Bp.Rank >= Rank.Rare)
                {
                    fighter.Enemy.Strengthen(new BattleStatus()
                    {
                        MaxHp = -(int)(fighter.Enemy.Status.MaxHp * Usual / 200)
                    });
                }
                else
                {
                    fighter.Enemy.Strengthen(new BattleStatus()
                    {
                        MaxHp = -(int)(fighter.Enemy.Status.MaxHp * Usual / 100)
                    });
                }
            }
            else
            {
                fighter.Heal(new BattleStatus { CurHp = (int)(Usual * fighter.Status.MaxHp) / 100 });
            }
        }


        [Effect("TYTQ_ALC", Timing.OnKill)]
        private Attack HumorialExtraction(Attack attack, FighterData fighter, FighterData enemy)
        {
            if (SData.CurGameRandom.NextDouble() < Usual)
            {
                var p = PotionManager.Instance.RollT(Rank.Normal).First();
                ((PlayerData)fighter).TryTakeOffer(new Offer(p), out _);
                Activated?.Invoke();
            }

            return attack;
        }


        [Effect("JPX_ALC", Timing.OnAttack)]
        private Attack Anatomy(Attack attack, FighterData fighter, FighterData enemy)
        {
            fighter.Recover(new BattleStatus { CurHp = CurLv }, enemy);
            Activated?.Invoke();
            return attack;
        }

        [Effect("NYX_ALC", Timing.OnCounterCharge)]
        private BattleStatus DrugResistance(BattleStatus status, FighterData fighter, string kw)
        {
            status.CurHp -= (int)(Bp.Param1 * CurLv);
            status.CurHp = max(status.CurHp, 0);
            Activated?.Invoke();
            return status;
        }


        [Effect("DYX_ALC", Timing.OnApply)]
        private BuffData DYX_ALC(BuffData buff, FighterData fighter)
        {
            if (buff.Id == "poison")
            {
                buff.StackChange((int)Usual);
                Activated?.Invoke();
            }

            return buff;
        }


        [Effect("XYLC_ALC", Timing.SkillEffect)]
        private void XYLC_ALC(FighterData player)
        {
            var v = (int)Bp.Param1 + (int)Bp.Param2 * CurLv;
            if (GameManager.Instance.InBattle)
            {
                player.Enemy.SingleDefendSettle(new Attack(cAtk: v), null);
            }
            else
            {
                player.Heal(BattleStatus.Hp(v));
            }
        }


        [Effect("DZXY_ALC", Timing.OnDefendSettle)]
        private Attack PoisonBlood(Attack attack, FighterData fighter, FighterData enemy)
        {
            if (enemy == null) return attack;

            if (attack.PDmg > 0)
            {
                var v = min(attack.PDmg, fighter.Status.CurHp);
                Activated?.Invoke();
                fighter.ApplyBuff(new BuffData("poison", (int)(Usual * v)), enemy);
            }

            return attack;
        }

        [Effect("ZH_ALC", Timing.OnDefendSettle)]
        private Attack SelfDestructive(Attack attack, FighterData fighter, FighterData enemy)
        {
            if (attack.SumDmg > 0)
            {
                if (SData.CurGameRandom.NextDouble() < Usual / 100)
                {
                    fighter.RandomStrengthen();
                    Activated?.Invoke();
                }
            }

            return attack;
        }

        [Effect("MY_ALC", Timing.OnUsePotion)]
        private PotionData MagicAddiction(PotionData potion, FighterData fighter)
        {
            fighter.Heal(BattleStatus.Hp((int)Usual));
            fighter.Heal(BattleStatus.Mp((int)Usual));
            fighter.RemoveBuff("torture");
            Activated?.Invoke();
            return potion;
        }


        /// <summary>
        /// 攻击时，为自己施加一层折磨
        /// </summary>
        /// <param name="attack"></param>
        /// <param name="fighter"></param>
        /// <param name="enemy"></param>
        /// <returns></returns>
        [Effect("MY_ALC", Timing.OnAttack)]
        private Attack MagicAddiction(Attack attack, FighterData fighter, FighterData enemy)
        {
            fighter.ApplySelfBuff(BuffData.Torture(1));
            Activated?.Invoke();
            return attack;
        }


        [Effect("ZN_ALC", Timing.OnCounterCharge, priority = -100)]
        private BattleStatus SelfAbuse(BattleStatus status, FighterData fighter, string kw)
        {
            var chance = status.CurHp * Usual;
            if (SData.CurGameRandom.NextDouble() < chance)
            {
                fighter.Strengthen(new BattleStatus() { MaxHp = 1, MaxMp = 1 });
                Activated?.Invoke();
            }

            return status;
        }


        [Effect("BSTY_ALC", Timing.OnStrengthen)]
        private BattleStatus NDE(BattleStatus modify, FighterData fighter)
        {
            var chance = fighter.Status.CurHp / fighter.Status.MaxHp;
            if (SData.CurGameRandom.NextDouble() < chance)
            {
                modify = modify.LvUp(1);
                Activated?.Invoke();
            }

            return modify;
        }

        [Effect("JLYJ_ALC", Timing.OnUsePotion)]
        private PotionData RefiningElixir(PotionData potion, FighterData fighter)
        {
            if (SData.CurGameRandom.NextDouble() < Usual / 100)
            {
                ((PlayerData)fighter).TryTakePotion(potion.Id, out _);
                Activated?.Invoke();
            }

            return potion;
        }

        [Effect("TNFB_ALC", Timing.OnUsePotion, alwaysActive = true)]
        private PotionData TNFB_ALC(PotionData potion, FighterData fighter)
        {
            if (CooldownLeft > 0)
            {
                BonusCooldown(1);
                Activated?.Invoke();
            }

            return potion;
        }

        [Effect("TNFB_ALC", Timing.SkillEffect)]
        private void TNFB_ALC2(FighterData fighter)
        {
            fighter.CoolDown();
        }


        [Effect("HQ_MAG", Timing.OnAttack, priority = -100)]
        private Attack FireBall(Attack attack, FighterData fighter, FighterData enemy)
        {
            return attack.Change(mAtk: fighter.Status.MAtk, multi: (int)Usual);
        }


        [Effect("ZZFD_MAG", Timing.OnAttack, priority = -100)]
        private Attack TraceMissile(Attack attack, FighterData fighter, FighterData enemy)
        {
            var multi = 2f;
            if (enemy.Buffs.Any((data => data.Bp.BuffType == BuffType.Negative)))
            {
                multi += Usual;
            }

            return attack.Change(mAtk: fighter.Status.MAtk, multi: multi);
        }

        [Effect("ASFD_MAG", Timing.OnAttack, priority = -100)]
        private Attack ArcaneMissile(Attack attack, FighterData fighter, FighterData enemy)
        {
            return attack.Change(mAtk: fighter.Status.MAtk, combo: (int)Bp.Param1);
        }


        [Effect("ANBF_MAG", Timing.OnAttack, priority = 100)]
        private Attack ANBF_MAG(Attack attack, FighterData fighter, FighterData enemy)
        {
            if (attack.PAtk == 0)
            {
                attack.Multi += Bp.Param1 * CurLv;
            }

            return attack;
        }


        [Effect("BF_MAG", Timing.OnHandleSkillInfo)]
        private Info BF_MAG(Info info, SkillData skill, PlayerData player)
        {
            if (info is FailureInfo failure && failure.Reason.Contains(FailureReason.SkillNotReady))
            {
                player.CounterCharge(-BattleStatus.Hp((int)(Bp.Param1 - Bp.Param2 * CurLv) * skill.CooldownLeft));
                failure.Reason.Remove(FailureReason.SkillNotReady);
                if (failure.Reason.Count == 0)
                {
                    return new SuccessInfo();
                }
                else
                {
                    return failure;
                }
            }

            return info;
        }


        [Effect("YSS_MAG", Timing.OnAttack, priority = -100)]
        private Attack Meteor(Attack attack, FighterData fighter, FighterData enemy)
        {
            return attack.Change(mAtk: fighter.Status.MAtk, multi: Usual);
        }

        [Effect("AFLQ_MAG", Timing.SkillEffect)]
        private void AFLQ_MAG(FighterData player)
        {
            player.CoolDown(1);
        }

        [Effect("MDX_MAG", Timing.OnStrengthen)]
        private BattleStatus MDX_MAG(BattleStatus modify, FighterData player)
        {
            if (modify.MAtk > 0)
            {
                modify.MDef += 1;
                Activated?.Invoke();
            }

            return modify;
        }


        [Effect("JZ_MAG", Timing.BeforeAttack)]
        private void Arrogance(PlayerData player, FighterData enemy)
        {
            if (player.Engaging)
            {
                Activated?.Invoke();
                ((PlayerData)player).ApplySelfBuff(new BuffData("mplus", (int)Usual));
            }
        }


        [Effect("MS_MAG", Timing.OnKill)]
        private Attack MieShi(Attack attack, FighterData player, FighterData enemy)
        {
            player.Strengthen(new BattleStatus { MaxMp = (int)Usual });
            Activated?.Invoke();
            return attack;
        }


        [Effect("YTZQ_MAG", Timing.OnAttack)]
        private Attack EtherBody(Attack attack, FighterData fighter, FighterData enemy)
        {
            var num = fighter.Status.CurMp - fighter.Status.CurHp;
            if (num > 0 && attack.MAtk > 0)
            {
                attack.MAtk += (int)(num * Usual);
                Activated?.Invoke();
            }

            return attack;
        }

        [Effect("FLBZ_MAG", Timing.OnAttack)]
        private Attack FaLiBaoZou(Attack attack, FighterData fighter, FighterData enemy)
        {
            var num = attack.CostInfo.ActualValue;
            if (num != 0 && attack.MAtk > 0)
            {
                attack.MAtk += (int)(Usual * num);
                Activated?.Invoke();
            }

            return attack;
        }

        [Effect("ADLF_MAG", Timing.OnAttack)]
        private Attack ADLF_MAG(Attack attack, FighterData fighter, FighterData enemy)
        {
            if (CooldownLeft > 0)
            {
                return attack;
            }

            if (attack.Combo > 1)
            {
                attack.Combo += 1;
                SetCooldown(Bp.Cooldown);
                Activated?.Invoke();
            }

            return attack;
        }

        [Effect("ADLF_MAG", Timing.OnSetCoolDown)]
        private SkillData ADLF_CD(SkillData skill, FighterData player)
        {
            skill.InitCoolDown -= CurLv;
            skill.BonusCooldown(CurLv);
            return skill;
        }

        [Effect("ANHJ_MAG", Timing.OnDefendSettle, priority = -1)]
        private Attack ANHJ(Attack attack, FighterData fighter, FighterData enemy)
        {
            var leftMp = fighter.Status.CurMp;

            if (leftMp > 0 && attack.PDmg > 0)
            {
                Activated?.Invoke();

                var maxAbsorb = min(fighter.Status.CurMp * Usual, attack.PDmg);

                fighter.Cost(new CostInfo((int)(maxAbsorb / Usual), CostType.Mp));

                attack.PDmg -= (int)maxAbsorb;
            }


            return attack;
        }


        [Effect("LJZL_MAG", Timing.OnAttackSettle)]
        private Attack LJZL_MAG(Attack attack, FighterData fighter, FighterData enemy)
        {
            if (!attack.Kw.IsNullOrWhitespace()) return attack;

            if (fighter.CooldownRandomSkill(1))
            {
                Activated?.Invoke();
            }

            return attack;
        }


        [Effect("YLK_MAG", Timing.SkillEffect)]
        private void YLK_MAG(FighterData player)
        {
            player.ApplySelfBuff(new BuffData("divinity", 1));
            player.ApplySelfBuff(new BuffData("leakage", 3));
            player.ApplySelfBuff(new BuffData("vigor", 1));
        }

        [Effect("YLK_MAG", Timing.OnSetCoolDown)]
        private SkillData YLK_CD(SkillData skill, FighterData player)
        {
            skill.InitCoolDown -= CurLv;
            skill.BonusCooldown(CurLv);
            return skill;
        }


        [Effect("ZHYJ_ALC", Timing.OnDefend)]
        private Attack ZHYJ_ALC(Attack attack, FighterData fighter, FighterData enemy)
        {
            if (enemy == null)
            {
                return attack;
            }

            var count = enemy.Buffs.Sum((data => data.Bp.BuffType == BuffType.Negative ? data.CurLv : 0));
            count = (int)(count * Usual);
            if (count > 0)
            {
                Activated?.Invoke();
            }

            attack.PAtk -= count;
            return attack;
        }


        [Effect("RDTP_ASS", Timing.OnAttack, priority = -100)]
        private Attack RDTP_ASS(Attack attack, FighterData fighter, FighterData enemy)
        {
            return attack.Change(pAtk: fighter.Status.PAtk, multi: Bp.Param1);
        }

        [Effect("RDTP_ASS", Timing.OnStrike, alwaysActive = true)]
        private Attack RDTP_ASS2(Attack attack, FighterData fighter, FighterData enemy, int time)
        {
            BonusCooldown(1);
            return attack;
        }

        [Effect("CD_ASS", Timing.OnStrike)]
        private Attack CD_ASS2(Attack attack, FighterData fighter, FighterData enemy, int time)
        {
            fighter.ApplyBuff(new BuffData("poison", (int)Bp.Param1 * CurLv), enemy);
            Activated?.Invoke();
            return attack;
        }

        [Effect("XJ_ASS", Timing.OnAttack, priority = -100)]
        private Attack XJ_ASS(Attack attack, FighterData fighter, FighterData enemy)
        {
            var multi = ((PlayerData)fighter).Engaging ? Bp.Param1 * CurLv + Bp.Param1 : Bp.Param1;

            return attack.Change(fighter.Status.PAtk, multi: multi);
        }

        [Effect("CJBY_ASS", Timing.OnAttack, priority = -100)]
        private Attack CJBY_ASS(Attack attack, FighterData fighter, FighterData enemy)
        {
            var multi = Bp.Param1 + Bp.Param1 * CurLv;

            return attack.Change(fighter.Status.PAtk, multi: multi);
        }


        [Effect("CJBY_ASS", Timing.OnKill, priority = 100)]
        private Attack CJBY2_ASS(Attack attack, FighterData fighter, FighterData enemy)
        {
            var overKill = attack.PDmg + attack.MDmg + attack.CDmg - enemy.Status.CurHp;
            if (overKill > 0)
            {
                fighter.Gain(overKill);
                Activated?.Invoke();
            }

            return attack;
        }

        [Effect("DMJJ_ASS", Timing.OnAttack, priority = -100)]
        private Attack DMJJ2_ASS(Attack attack, FighterData fighter, FighterData enemy)
        {
            return attack.Change((int)(fighter.Status.PAtk * 0.7f), combo: (int)Usual);
        }


        [Effect("DMJJ_ASS", Timing.OnKill, priority = 100)]
        private Attack CMJJ_ASS(Attack attack, FighterData fighter, FighterData enemy)
        {
            Counter += 1;
            if (Counter >= 7)
            {
                Counter = 0;
                CurLv += 1;
            }

            Activated?.Invoke();
            return attack;
        }


        [Effect("ST_ASS", Timing.OnAttack, priority = -100)]
        private Attack ST_ASS(Attack attack, FighterData fighter, FighterData enemy)
        {
            if (((PlayerData)fighter).Engaging)
            {
                var m = min(Usual, 100);
                var g = (int)(enemy.Status.Gold * m / 100);
                fighter.Gain(g);
                enemy.Gain(-g);
                Activated?.Invoke();
            }

            return attack;
        }

        [Effect("ST_ASS", Timing.OnReact)]
        private MapData ST_ASS2(MapData mapData, FighterData player)
        {
            if (mapData is TravellerSaveData)
            {
                Activated?.Invoke();
                var gold = SData.CurGameRandom.Next(10 * CurLv, 20 * CurLv);
                player.Gain(gold);
            }

            return mapData;
        }


        /// <summary>
        /// 购买技能金币消耗减少P1*Lv
        /// </summary>
        /// <param name="cost"></param>
        /// <param name="player"></param>
        /// <param name="kw"></param>
        /// <returns></returns>
        [Effect("QSRH_ASS", Timing.OnCost, priority = -100)]
        private CostInfo QSRH_ASS(CostInfo cost, FighterData player, string kw)
        {
            if (cost.CostType == CostType.Gold)
            {
                cost.Value -= (int)(CurLv * Bp.Param1);
                Activated?.Invoke();
            }

            return cost;
        }

        /// <summary>
        /// 获得金币时：获得P1*Lv层物增
        /// </summary>
        /// <param name="coin"></param>
        /// <param name="fighter"></param>
        /// <param name="kw"></param>
        /// <returns></returns>
        [Effect("TL_ASS", Timing.OnGain, priority = 100)]
        private int TL_ASS(int coin, FighterData fighter, string kw)
        {
            fighter.ApplySelfBuff(new BuffData("pplus", (int)Usual));
            Activated?.Invoke();
            return coin;
        }

        /// <summary>
        /// 斩杀时：获得<P1*CurLv>(P1*Lv)额外金币
        /// </summary>
        /// <param name="attack"></param>
        /// <param name="fighter"></param>
        /// <param name="enemy"></param>
        /// <returns></returns>
        [Effect("GHXS_ASS", Timing.OnKill, priority = 100)]
        private Attack GHXS_ASS(Attack attack, FighterData fighter, FighterData enemy)
        {
            fighter.Gain((int)Usual);
            Activated?.Invoke();
            return attack;
        }

        /// <summary>
        /// 攻击时，附加等于敌人金币数的临时物攻
        /// </summary>
        /// <param name="attack"></param>
        /// <param name="fighter"></param>
        /// <param name="enemy"></param>
        /// <returns></returns>
        [Effect("JY_ASS", Timing.OnAttack, priority = 100)]
        private Attack JY_ASS(Attack attack, FighterData fighter, FighterData enemy)
        {
            if (enemy != null)
            {
                attack.PAtk += enemy.Gold;
                Activated?.Invoke();
            }

            return attack;
        }


        /// <summary>
        /// 攻击时：有<P1+P2*CurLv>(P1+P2*Lv)的概率当成交锋
        /// </summary>
        /// <param name="attack"></param>
        /// <param name="fighter"></param>
        /// <param name="enemy"></param>
        /// <returns></returns>
        [Effect("YN_ASS", Timing.OnAttack, priority = -10000)]
        private Attack YN_ASS(Attack attack, FighterData fighter, FighterData enemy)
        {
            Counter += 1;
            if (Counter >= Bp.Param1 - CurLv && !((PlayerData)fighter).Engaging)
            {
                Counter = 0;
                ((PlayerData)fighter).Engaging = true;
                Debug.Log("YN!");
                Activated?.Invoke();
            }

            return attack;
        }

        /// <summary>
        /// 造成敌人当前生命<P1+P2*CurLv>(P1+P2*Lv)的物理伤害
        /// </summary>
        /// <param name="attack"></param>
        /// <param name="fighter"></param>
        /// <param name="enemy"></param>
        /// <returns></returns>
        [Effect("SYS_ASS", Timing.OnAttack, priority = -10000)]
        private Attack SYS_ASS(Attack attack, FighterData fighter, FighterData enemy)
        {
            attack.Change(pAtk: (int)(enemy.CurHp * Usual / 100));

            return attack;
        }


        [Effect("XX_MON", Timing.OnAttackSettle, priority = 2)]
        private Attack XX_MON(Attack attack, FighterData player, FighterData enemy)
        {
            player.Recover(BattleStatus.Hp(attack.PDmg) * Usual / 100, enemy);
            return attack;
        }

        [Effect("LX_MON", Timing.OnAttack, priority = -10000)]
        private Attack LX_MON(Attack attack, FighterData attacker, FighterData enemy)
        {
            return attack.Change(attacker.Status.PAtk, attacker.Status.MAtk, multi: Usual);
        }


        [Effect("DT_MON", Timing.OnAttack)]
        private Attack DT_MON(Attack attack, FighterData player, FighterData enemy)
        {
            attack.Combo = (int)Usual;
            return attack;
        }

        [Effect("ZY_MON", Timing.BeforeAttack)]
        private void ZY_MON(FighterData player, FighterData enemy)
        {
            player.Heal(BattleStatus.Hp((int)Usual), "ZY_MON");
        }

        [Effect("HLDJ_MON", Timing.OnAttack, priority: -10000)]
        private Attack HLDJ_MON(Attack attack, FighterData player, FighterData enemy)
        {
            return attack.Change(mAtk: player.Status.PAtk, multi: Usual);
        }


        [Effect("YWSY_ALC", Timing.OnUsePotion)]
        private PotionData YWSY_ALC(PotionData potion, FighterData user)
        {
            var v = ((int)potion.Bp.Rank + 1) * Usual;
            if (SData.CurGameRandom.NextDouble() < ((PlayerData)user).LuckyChance)
            {
                user.Strengthen(new BattleStatus(maxHp: (int)v));
                Activated?.Invoke();
            }

            return potion;
        }

        [Effect("TC_ALC", Timing.SkillEffect)]
        private FighterData TC_ALC(FighterData fighter)
        {
            var p = (PlayerData)fighter;
            foreach (var potion in p.Potions)
            {
                if (potion != null && !potion.Id.IsNullOrWhitespace() && potion.Count > 0 &&
                    potion.Bp.Rank <= Rank.Rare)
                {
                    var creation =
                        MathNet.Numerics.Distributions.Binomial.Sample(SData.CurGameRandom, min(1, Usual / 100),
                            potion.Count);

                    potion.Count += creation;
                }
            }

            Counter += 1;
            return fighter;
        }


        [Effect("TC_ALC", Timing.OnMarch, alwaysActive = true)]
        private void TC_ALC2(FighterData fighter)
        {
            CooldownLeft = 0;
        }


        [Effect("XFYJ_ALC", Timing.OnUsePotion)]
        private PotionData XFYJ_ALC(PotionData potion, FighterData user)
        {
            Counter += 1;
            if (Counter >= Unusual)
            {
                Counter = 0;
                if (user.CooldownRandomSkill(1))
                {
                    Activated?.Invoke();
                }
            }

            return potion;
        }


        /// <summary>
        /// 防御时：恢复已损失魔法值的<P1+P2*CurLv>%(P1+P2*Lv)
        /// </summary>
        /// <param name="attack"></param>
        /// <param name="fighter"></param>
        /// <param name="enemy"></param>
        /// <returns></returns>
        [Effect("LZ_MAG", Timing.OnDefendSettle)]
        private Attack LZ_MAG(Attack attack, FighterData fighter, FighterData enemy)
        {
            if (enemy != null && attack.SumDmg > 0 && fighter.Status.CurMp < fighter.Status.MaxMp)
            {
                var v = fighter.Status.MaxMp - fighter.Status.CurMp;
                fighter.Heal(BattleStatus.Mp((int)(v * Usual / 100)));
                Activated?.Invoke();
            }

            return attack;
        }

        /// <summary>
        /// 造成<P1+P2*CurLv> P1 + P2*Lv倍纯粹伤害
        /// </summary>
        /// <param name="attack"></param>
        /// <param name="fighter"></param>
        /// <param name="enemy"></param>
        /// <returns></returns>
        [Effect("MBS_MAG", Timing.OnAttack, priority = -10000)]
        private Attack MBS_MAG(Attack attack, FighterData fighter, FighterData enemy)
        {
            attack.Change(cAtk: fighter.Status.MAtk, multi: Usual);
            return attack;
        }


        /// <summary>
        /// 攻击时：攻击段数+1，攻击倍率-<P1-P2*CurLv>（P1-P2*Lv）
        /// </summary>
        /// <param name="attack"></param>
        /// <param name="fighter"></param>
        /// <param name="enemy"></param>
        /// <returns></returns>
        [Effect("XYLS_ASS", Timing.OnAttack, priority = 1)]
        private Attack XYLS_ASS(Attack attack, FighterData fighter, FighterData enemy)
        {
            attack.Combo += 1;
            attack.Multi -= Bp.Param1 - Bp.Param2 * CurLv;
            Activated?.Invoke();
            return attack;
        }


        /// <summary>
        /// 获取、升级主职业技能时：获得两次对应提升
        /// </summary>
        /// <param name="skill"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        [Effect("SYZG_COM", Timing.OnLvUp)]
        private SkillData SYZG_COM(SkillData skill, FighterData player)
        {
            if (skill.Bp.IsMainProf())
            {
                player.Strengthen(BattleStatus.GetProfessionUpgrade(skill.Bp.Prof));
                Activated?.Invoke();
            }

            return skill;
        }

        /// <summary>
        /// 获取、升级副职业技能时：获得两次对应提升
        /// </summary>
        /// <param name="skill"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        [Effect("WHBM_COM", Timing.OnLvUp)]
        private SkillData WHBM_COM(SkillData skill, FighterData player)
        {
            if (!skill.Bp.IsMainProf())
            {
                player.Strengthen(BattleStatus.GetProfessionUpgrade(skill.Bp.Prof));
                Activated?.Invoke();
            }

            return skill;
        }

        /// <summary>
        /// 升级技能时：恢复<P1+P2*CurLv>（P1+P2*Lv）点生命值
        /// </summary>
        /// <param name="skill"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        [Effect("XTZX_COM", Timing.OnLvUp)]
        private SkillData XTZX_COM(SkillData skill, FighterData player)
        {
            player.Heal(BattleStatus.Hp((int)Usual));
            player.Heal(BattleStatus.Mp((int)Usual));
            Activated?.Invoke();
            return skill;
        }

        /// <summary>
        /// 访问草地、泉水时：恢复提升<P1+P2*CurLv>%（P1+P2*Lv）%
        /// </summary>
        [Effect("QH_COM", Timing.OnHeal)]
        private BattleStatus QH_COM(BattleStatus status, FighterData fighter, string kw)
        {
            if (kw == "supply")
            {
                status *= 1 + Usual / 100;
                Activated?.Invoke();
            }

            return status;
        }

        /// <summary>
        /// 访问草地、泉水时：不再恢复，提升恢复量<P1+P2*CurLv>%（P1+P2*Lv）%的最大值
        /// </summary>
        [Effect("DLY_COM", Timing.OnHeal, priority = 10)]
        private BattleStatus DLY_COM(BattleStatus status, FighterData fighter, string kw)
        {
            if (kw == "supply")
            {
                status.MaxHp = (int)(status.CurHp * Usual / 100);
                status.CurHp = (int)((1 - Usual / 100) * status.CurHp);
                status.MaxMp = (int)(status.CurMp * Usual / 100);
                status.CurMp = (int)((1 - Usual / 100) * status.CurMp);
                Activated?.Invoke();
            }

            return status;
        }


        /// <summary>
        /// 访问岩石时：消耗法力减少#P1*CurLv#（P1*Lv）,获得金币#P2*CurLv#（P2*Lv）
        /// </summary>
        /// <param name="map"></param>
        /// <param name="fighter"></param>
        /// <returns></returns>
        [Effect("KCTF_COM", Timing.OnCost)]
        private CostInfo KCTF_COM(CostInfo cost, FighterData fighter, string kw)
        {
            if (kw == "rock")
            {
                Activated?.Invoke();
                cost.Value -= (int)(Bp.Param1 * CurLv);
                fighter.Gain((int)Bp.Param2 * CurLv);
            }

            return cost;
        }

        /// <summary>
        /// 访问坟墓、赌场时：获胜的概率提高#P1+P2*CurLv#%（P1+P2*Lv）%
        /// </summary>
        /// <param name="map"></param>
        /// <param name="fighter"></param>
        /// <returns></returns>
        [Effect("PMZD_COM", Timing.OnReact)]
        private MapData PMZD_COM(MapData map, FighterData fighter)
        {
            if (map is CasinoSaveData casino)
            {
                casino.BaseBias = Usual / 100;
                Activated?.Invoke();
            }

            if (map is CemeterySaveData cemetery)
            {
                cemetery.BaseBias = Usual / 100;
                Activated?.Invoke();
            }

            return map;
        }


        /// <summary>
        /// 下层时：#P1+P2*CurLv#（P1+P2*Lv）个1星技能提升1级
        /// </summary>
        /// <returns></returns>
        [Effect("DDZJ_COM", Timing.OnMarch)]
        private void DDZJ_COM(FighterData fighter)
        {
            ((PlayerData)fighter).UpgradeRandomSkills(SData.CurGameRandom, Rank.Normal,
                (int)(Bp.Param1 + Bp.Param2 * CurLv));
            Activated?.Invoke();
        }

        /// <summary>
        /// 下层时：#P1+P2*CurLv#（P1+P2*Lv）个2星技能提升1级
        /// </summary>
        /// <returns></returns>
        [Effect("CKMJ_COM", Timing.OnMarch)]
        private void CKMJ_COM(FighterData fighter)
        {
            if (fighter is PlayerData player)
            {
                player.UpgradeRandomSkills(SData.CurGameRandom, Rank.Rare, (int)(Bp.Param1 + Bp.Param2 * CurLv));
                Activated?.Invoke();
            }
        }

        /// <summary>
        /// 下层时：每有一个3星黄色技能，获得1次主职业对应的属性提升
        /// </summary>
        [Effect("DTRS_COM", Timing.OnMarch)]
        private void DTRS_COM(FighterData fighter)
        {
            if (fighter is PlayerData player)
            {
                var times = player.Skills.Count(skill => skill.Bp.Rank == Rank.Rare);
                for (int i = 0; i < times; i++)
                {
                    player.Strengthen(BattleStatus.GetProfessionUpgrade(PlayerData.ProfInfo[0]));
                }

                Activated?.Invoke();
            }
        }

        /// <summary>
        /// 下层时：若除主职业的技能数量超过2个，则#P1+P2*CurLv#(P1+P2*Lv)个技能升级一次
        /// </summary>
        /// <returns></returns>
        [Effect("ZZ_COM", Timing.OnMarch)]
        private void ZZ_COM(FighterData fighter)
        {
            if (fighter is PlayerData player)
            {
                bool satisfy = player.Skills.Count(skill => !skill.Bp.IsMainProf()) > 2;
                if (satisfy)
                {
                    player.UpgradeRandomSkills(SData.CurGameRandom, Rank.All, (int)(Bp.Param1 + Bp.Param2 * CurLv));
                    Activated?.Invoke();
                }
            }
        }

        /// <summary>
        /// 下层时：若没有一个职业的技能数量超过2个，则#P1+P2*CurLv#(P1+P2*Lv)个技能升级一次
        /// </summary>
        /// <param name="fighter"></param>
        [Effect("RH_COM", Timing.OnMarch)]
        private void RH_COM(FighterData fighter)
        {
            if (fighter is PlayerData player)
            {
                bool satisfy = true;
                foreach (var prof in PlayerData.ProfInfo)
                {
                    if (player.Skills.Count(skill => skill.Bp.Prof == prof) > 2)
                    {
                        satisfy = false;
                        break;
                    }
                }

                if (satisfy)
                {
                    player.UpgradeRandomSkills(SData.CurGameRandom, Rank.All, (int)(Bp.Param1 + Bp.Param2 * CurLv));
                    Activated?.Invoke();
                }
            }
        }


        /// <summary>
        /// 造成#P1+P2*CurLv#（P1+P2*Lv）倍法术伤害
        /// </summary>
        /// <param name="attack"></param>
        /// <param name="fighter"></param>
        /// <returns></returns>
        [Effect("ZSS_COM", Timing.OnAttack)]
        private Attack ZSS_COM(Attack attack, FighterData fighter, FighterData enemy)
        {
            return attack.Change(mAtk: fighter.Status.MAtk, multi: Usual);
        }

        /// <summary>
        /// 造成#P1+P2*CurLv#（P1+P2*Lv）倍物理伤害
        /// </summary>
        /// <param name="attack"></param>
        /// <param name="fighter"></param>
        /// <param name="enemy"></param>
        /// <returns></returns>
        [Effect("QLJ_COM", Timing.OnAttack)]
        private Attack QLJ_COM(Attack attack, FighterData fighter, FighterData enemy)
        {
            attack.Change(pAtk: fighter.Status.PAtk, multi: Usual);
            Activated?.Invoke();
            return attack;
        }

        /// <summary>
        /// 恢复#P1+P2*CurLv#（P1+P2*Lv）点生命值
        /// </summary>
        /// <returns></returns>
        [Effect("ZYS_COM", Timing.SkillEffect)]
        private void ZYS_COM(FighterData fighter)
        {
            fighter.Heal(BattleStatus.Hp((int)Usual));
            Activated?.Invoke();
        }


        [Effect("XM1_ASS", Timing.OnAttack, priority = -1000)]
        private Attack XM1_ASS(Attack attack, FighterData player, FighterData enemy)
        {
            SwitchTo("XM2_ASS");
            attack = attack.Change(pAtk: player.Status.PAtk, multi: Usual);
            return attack;
        }

        [Effect("XM2_ASS", Timing.OnAttack, priority = -1000)]
        private Attack XM2_ASS(Attack attack, FighterData player, FighterData enemy)
        {
            SwitchTo("XM3_ASS");
            attack = attack.Change(mAtk: player.Status.MAtk, multi: Usual);
            return attack;
        }

        [Effect("XM2_ASS", Timing.OnKill, alwaysActive = true)]
        private Attack XM2_ASS2(Attack attack, FighterData player, FighterData enemy)
        {
            SwitchTo("XM1_ASS");
            return attack;
        }

        [Effect("XM3_ASS", Timing.OnAttack, priority = -1000)]
        private Attack XM3_ASS(Attack attack, FighterData player, FighterData enemy)
        {
            SwitchTo("XM1_ASS");
            attack = attack.Change(cAtk: player.Status.PAtk, multi: Usual);
            return attack;
        }

        [Effect("XM3_ASS", Timing.OnKill, alwaysActive = true)]
        private Attack XM3_ASS2(Attack attack, FighterData player, FighterData enemy)
        {
            SwitchTo("XM1_ASS");
            return attack;
        }

        [Effect("AFXT1_MAG", Timing.SkillEffect)]
        private void AFXT1_MAG(FighterData player)
        {
            SwitchTo("AFXT2_MAG");
        }

        [Effect("AFXT2_MAG", Timing.SkillEffect)]
        private void AFXT2_MAG(FighterData player)
        {
            SwitchTo("AFXT1_MAG");
        }

        [Effect("AFXT2_MAG", Timing.OnAttack)]
        private Attack AFXT2_mag2(Attack attack, FighterData player, FighterData enemy)
        {
            if (attack.Kw.IsNullOrWhitespace())
            {
                if (player.CanAfford(Bp.CostInfo, out _))
                {
                    Activated?.Invoke();
                    return attack.Change(mAtk: player.Status.MAtk, multi: Usual);
                }
                else
                {
                    SwitchTo("AFXT1_MAG");
                    return attack;
                }
            }
            else
            {
                return attack;
            }
        }


        [Effect("ZYSY_ASS", Timing.OnReact)]
        private MapData ZYSY(MapData atom, PlayerData playerData)
        {
            var chance = Usual / 100;
            if (atom is CasinoSaveData casino)
            {
                Activated?.Invoke();
                casino.BaseBias += chance;
            }

            if (atom is DoorSaveData door)
            {
                Activated?.Invoke();
                if (SData.CurGameRandom.NextDouble() < chance)
                {
                    door.Open();
                }
            }

            return atom;
        }


        [Effect("SD_ALC", Timing.OnAttack)]
        private Attack SD_ALC(Attack attack, FighterData player, FighterData enemy)
        {
            player.ApplyBuff(BuffData.Poison((int)Usual), enemy);
            return attack.SwitchToEmpty();
        }

        [Effect("QS_MON", Timing.OnAttack)]
        private Attack QS_MON(Attack attack, FighterData player, FighterData enemy)
        {
            return attack.Change(mAtk: player.Status.MAtk);
        }


        [Effect("QX_MAG", Timing.OnAttack)]
        private Attack QX_MAG(Attack attack, FighterData player, FighterData enemy)
        {
            var first_battle_skill =
                player.Skills.FirstOrDefault(skill => skill.IsValid && skill.Bp.BattleOnly && skill != this);
            if (first_battle_skill != null)
            {
                Sealed = true;
                for (var i = 0; i < Usual; i++)
                {
                    if (player.CanAfford(first_battle_skill.Bp.CostInfo, out _) && player.IsAlive)
                    {
                        player.ManageAttackRound(first_battle_skill);
                        if (!enemy.IsAlive) break;
                    }
                }
            }

            return attack.SwitchToEmpty();
        }


        [Effect("DSYB_ALC", Timing.OnAttack)]
        private Attack DSYB_ALC(Attack attack, FighterData player, FighterData enemy)
        {
            var enemyPoison = enemy.Buffs.FirstOrDefault(buff => buff.Id.Equals("poison"));
            if (enemyPoison?.CurLv > 0)
            {
                return attack.Change(mAtk: enemyPoison.CurLv, multi: Usual);
            }
            else
            {
                return attack.Change(multi: Usual);
            }
        }


        [Effect("SH1_KNI", Timing.SkillEffect)]
        private void SH1_KNI(FighterData player)
        {
            SwitchTo("SH2_KNI");
            var gold = player.Status.Gold;
            player.Status *= 1 + Usual / 100;
            player.Status.Gold = gold;
            foreach (var skill in player.Skills)
            {
                if (skill.IsValid && skill.Bp.Prof.Equals("mon"))
                {
                    player.Upgrade(skill, 3);
                }
            }
        }

        [Effect("SH2_KNI", Timing.SkillEffect)]
        private void SH2_KNI(FighterData player)
        {
            SwitchTo("SH1_KNI");
            var gold = player.Status.Gold;
            player.Status *= 1 / (1 + Usual / 100);
            player.Status.Gold = gold;
            foreach (var skill in player.Skills)
            {
                if (skill.IsValid && skill.Bp.Prof.Equals("mon"))
                {
                    skill.CurLv = max(1, skill.CurLv - 3);
                }
            }
        }


        [Effect("SH2_KNI", Timing.BeforeAttack)]
        private void SH2_KNI(FighterData player, FighterData enemy)
        {
            var cost = new CostInfo((int)(player.Status.MaxHp * 0.05f), CostType.Hp);
            if (player.CanAfford(cost, out _))
            {
                player.Cost(cost);
            }
            else
            {
                SwitchTo("SH1_KNI");
            }
        }


        /// <summary>
        /// 攻击时:若生命值在50%以下,则倍率增加#P1+P2*CurLv#（P1+P2*Lv）
        /// </summary>
        /// <param name="attack"></param>
        /// <param name="player"></param>
        /// <param name="enemy"></param>
        /// <returns></returns>
        [Effect("BQ_MON", Timing.OnAttack)]
        private Attack BQ_MON(Attack attack, FighterData player, FighterData enemy)
        {
            if (player.Status.CurHp < player.Status.MaxHp / 2)
            {
                attack.Multi += Usual;
                return attack;
            }
            else
            {
                return attack;
            }
        }

        [Effect("QLFY_KNI", Timing.OnAttack)]
        private Attack QLFY_KNI(Attack attack, FighterData player, FighterData enemy)
        {
            Counter += 1;
            return attack.SwitchToEmpty();
        }


        [Effect("QLFY_KNI", Timing.OnPreDefend, alwaysActive = true, priority = 10000)]
        private Attack QLFY_KNI2(Attack attack, FighterData player, FighterData enemy)
        {
            if (Counter > 0 && enemy != null)
            {
                attack.Multi -= Usual;
                Activated?.Invoke();
                Counter = 0;
            }

            return attack;
        }


        [Effect("JSZD_KNI", Timing.SkillEffect)]
        private void JSZD_KNI(FighterData player)
        {
            player.ApplySelfBuff(BuffData.Sturdy((int)Usual));
        }

        [Effect("JJCS_KNI", Timing.SkillEffect)]
        private void JJCS_KNI(FighterData player)
        {
            player.ApplySelfBuff(BuffData.Thorn((int)Usual));
        }

        [Effect("WS_KNI", Timing.OnAttack)]
        private Attack WS_KNI(Attack attack, FighterData player, FighterData enemy)
        {
            player.ApplyBuff(BuffData.Feeble((int)Usual), enemy);
            player.ApplyBuff(BuffData.PMinus((int)Usual), enemy);
            return attack.SwitchToEmpty();
        }

        [Effect("TZ_KNI", Timing.OnReact)]
        private MapData TZ_KNI(MapData map, FighterData player)
        {
            if (map is SupplySaveData supply && supply.Type == SupplyType.Grassland)
            {
                player.ApplySelfBuff(BuffData.Sturdy((int)Usual));
                Activated?.Invoke();
            }

            return map;
        }

        [Effect("SZZT_KNI", Timing.OnMarch)]
        private void SZZT_KNI(FighterData player)
        {
            player.Strengthen(new BattleStatus(maxHp: (int)Usual));
        }

        [Effect("ZJ_KNI", Timing.OnAttack, priority = 10000)]
        private Attack ZJ_KNI(Attack attack, FighterData player, FighterData enemy)
        {
            if (attack.Combo == 1)
            {
                attack.Multi += Usual;
                Activated?.Invoke();
            }

            return attack;
        }

        [Effect("XZ_KNI", Timing.OnDefendSettle, priority = 1000)]
        private Attack XZ_KNI(Attack attack, FighterData player, FighterData enemy)
        {
            Counter += attack.SumDmg;

            if (Counter > Unusual)
            {
                Activated?.Invoke();
                Counter = 0;
                player.Strengthen(new BattleStatus(pDef: 1));
            }

            return attack;
        }

        [Effect("DPMJ_KNI", Timing.OnAttack, priority = -10000)]
        private Attack DPMJ_KNI(Attack attack, FighterData player, FighterData enemy)
        {
            return attack.Change(pAtk: player.Status.PDef, multi: Usual);
        }


        [Effect("ZSCJ_KNI", Timing.SkillEffect)]
        private void ZSCJ_KNI(FighterData player)
        {
            (player.Status.PDef, player.Status.PAtk) = (player.Status.PAtk, player.Status.PDef);
        }

        [Effect("MC_KNI", Timing.OnAttack)]
        private Attack MC_KNI(Attack attack, FighterData player, FighterData enemy)
        {
            foreach (var sk in enemy.Skills)
            {
                if (sk != null && sk.IsValid)
                {
                    sk.CooldownLeft += 3;
                }
            }

            return attack.Change(multi: Usual);
        }

        [Effect("ZSGH_KNI", Timing.OnAttack)]
        private Attack ZSGH_KNI(Attack attack, FighterData player, FighterData enemy)
        {
            player.ApplySelfBuff(BuffData.Divinity((int)Usual));
            return attack.SwitchToEmpty();
        }

        [Effect("CZ_KNI", Timing.OnAttack)]
        private Attack CZ_KNI(Attack attack, FighterData player, FighterData enemy)
        {
            if (player is PlayerData p)
            {
                attack.Multi += p.BattleRound * Usual;
                Activated?.Invoke();
            }

            return attack;
        }


        [Effect("CX_KNI", Timing.SkillEffect)]
        private void CX_KNI(FighterData player)
        {
            var buff = player.Buffs.Find((data => data.Id == "thorn"));
            player.Heal(BattleStatus.Hp((int)Usual * (buff.CurLv / 2)));
            buff.StackChange(-buff.CurLv / 2);
        }

        [Effect("ZFZJ_KNI", Timing.OnAttack, priority: -10000)]
        private Attack ZFZJ_KNI(Attack attack, FighterData player, FighterData enemy)
        {
            var buff = player.Buffs.Find((data => data.Id == "thorn"));
            attack.Change(cAtk: buff.CurLv / 2, multi: Usual);
            buff.StackChange(-buff.CurLv / 2);
            return attack;
        }

        [Effect("NY_KNI", Timing.OnAttack, priority: -10000)]
        private Attack NY_KNI(Attack attack, FighterData player, FighterData enemy)
        {
            attack.PAtk = (int)(player.Status.CurHp * Usual);
            return attack;
        }

        [Effect("SY_KNI", Timing.OnAttack, priority: -10000)]
        private Attack SY_KNI(Attack attack, FighterData player, FighterData enemy)
        {
            return attack;
        }

        [Effect("SY_KNI", Timing.OnKill, priority: -10000)]
        private Attack SY_KNI2(Attack attack, FighterData player, FighterData enemy)
        {
            player.Strengthen(new BattleStatus(maxHp: (int)(enemy.Status.MaxHp * Usual / 100)));
            return attack;
        }

        [Effect("CZHJ_KNI", Timing.SkillEffect)]
        private void CZHJ_KNI(FighterData player)
        {
            var buff = player.Buffs.Find((data => data.Id == "Sturdy"));
            player.Strengthen(new BattleStatus(maxHp: (int)(buff.CurLv * Usual / 100)));
            player.Buffs.Remove(buff);
        }

        [Effect("CZHJ_KNI", Timing.OnMarch, alwaysActive = true)]
        private void CZHJ_KNI2(FighterData player)
        {
            CooldownLeft = 0;
        }


        [Effect("XXJJ_KNI", Timing.OnDefendSettle)]
        private Attack XXJJ_KNI(Attack attack, FighterData fighter, FighterData enemy)
        {
            if (enemy == null) return attack;

            if (attack.SumDmg > 0)
            {
                var v = min(attack.PDmg, fighter.Status.CurHp);
                Activated?.Invoke();
                fighter.ApplySelfBuff(BuffData.Thorn((int)Usual));
            }

            return attack;
        }

        [Effect("FSFJ_KNI", Timing.OnAttackSettle)]
        private Attack FSFJ_KNI(Attack attack, FighterData fighter, FighterData enemy)
        {
            if (attack.IsEmpty())
            {
                Counter += 1;
                Activated?.Invoke();
            }

            return attack;
        }

        [Effect("FSFJ_KNI", Timing.OnAttack)]
        private Attack FSFJ_KNI2(Attack attack, FighterData fighter, FighterData enemy)
        {
            if (Counter > 0 && !attack.IsEmpty())
            {
                attack.Multi += Usual;
                Counter = 0;
                Activated?.Invoke();
            }

            return attack;
        }


        [Effect("TS_KNI", Timing.OnKill)]
        private Attack TS_KNI(Attack attack, FighterData fighter, FighterData enemy)
        {
            if (enemy == null) return attack;
            foreach (var sk in enemy.Skills)
            {
                if (fighter is PlayerData player)
                {
                    player.TryTakeSkill(sk.Bp.Id, out var _);
                }
            }

            return attack;
        }


        [Effect("BY_MAG", Timing.SkillEffect)]
        private void BY_MAG(FighterData player)
        {
            player.ApplySelfBuff(BuffData.MPlus((int)Usual));
        }


        [Effect("SJDF_KNI", Timing.OnAttack, priority: 100)]
        private Attack SJDF_KNI(Attack attack, FighterData player, FighterData enemy)
        {
            if (attack.IsEmpty())
            {
                Activated?.Invoke();
                player.ApplySelfBuff(BuffData.PPlus((int)Usual));
                player.ApplySelfBuff(BuffData.MPlus((int)Usual));
            }

            return attack;
        }

        [Effect("EY_CUR", Timing.OnAttack, priority: -10000)]
        private Attack EY_CUR(Attack attack, FighterData player, FighterData enemy)
        {
            int count = 3;
            var different_debuffs = new List<BuffData>();
            int index = 0;

            while (index <= count)
            {
                var debuff = BuffManager.Instance.GetRandomBuffData(BuffType.Negative);
                debuff.StackChange((int)Usual - 1);
                if (debuff != null && !different_debuffs.Any((data => data?.Id == debuff.Id)))
                {
                    different_debuffs = different_debuffs.Append(debuff).ToList();
                    index++;
                    player.ApplyBuff(debuff, enemy);
                }
            }

            return attack.SwitchToEmpty();
        }


        [Effect("SJ_CUR", Timing.OnAttack, priority: -10000)]
        private Attack SJ_CUR(Attack attack, FighterData player, FighterData enemy)
        {
            int stack = (int)(player.Status.MAtk * Usual);
            player.ApplyBuff(BuffData.Feeble(stack), enemy);

            return attack.SwitchToEmpty();
        }

        [Effect("ZM_CUR", Timing.OnAttack, priority: -10000)]
        private Attack ZM_CUR(Attack attack, FighterData player, FighterData enemy)
        {
            enemy.Buffs.RemoveAll((data => data.Bp.BuffType == BuffType.Positive));
            return attack.SwitchToEmpty();
        }

        [Effect("KL_CUR", Timing.OnAttack, priority: -10000)]
        private Attack KL_CUR(Attack attack, FighterData player, FighterData enemy)
        {
            int stack = (int)Usual;
            player.ApplySelfBuff(BuffData.Poison(stack));
            player.ApplyBuff(BuffData.Poison(stack * 2), enemy);
            return attack.SwitchToEmpty();
        }

        [Effect("ED_CUR", Timing.OnAttack)]
        private Attack ED_CUR(Attack attack, FighterData player, FighterData enemy)
        {
            if (attack.PAtk <= 0) return attack;
            var stack =
                (int)(Usual * player.Buffs.Sum((data => data.Bp.BuffType == BuffType.Negative ? data.CurLv : 0)));
            attack.PAtk += stack;
            Activated?.Invoke();

            return attack;
        }

        [Effect("ZZ_CUR", Timing.OnApply)]
        private BuffData ZZ_CUR(BuffData buff, FighterData player)
        {
            if (buff.Bp.BuffType != BuffType.Negative) return buff;
            buff.StackChange((int)Usual);
            Activated?.Invoke();
            return buff;
        }

        [Effect("DC_CUR", Timing.SkillEffect)]
        private void DC_CUR(FighterData player)
        {
            var stack = (int)(Usual * player.Buffs.Count((data => data.Bp.BuffType == BuffType.Curse)));
            player.ApplySelfBuff(BuffData.MPlus(stack));
        }

        [Effect("YW_CUR", Timing.OnAttackSettle)]
        private Attack YW_CUR(Attack attack, FighterData player, FighterData enemy)
        {
            if (enemy.CurHp - attack.SumDmg < enemy.Status.MaxHp * Usual / 100)
            {
                attack.CDmg += max(0, enemy.CurHp - attack.SumDmg);
                Activated?.Invoke();
                player.Kill(attack, enemy);
            }

            return attack;
        }

        [Effect("LH_CUR", Timing.OnCurseActivate)]
        private BuffData LH_CUR(BuffData buff, FighterData player)
        {
            if (buff.Bp.BuffType != BuffType.Curse) return buff;
            player.PurifyAll();
            return buff;
        }

        [Effect("EH_CUR", Timing.OnAttack, priority: -10000)]
        private Attack EH_CUR(Attack attack, FighterData player, FighterData enemy)
        {
            foreach (var buff in enemy.Buffs)
            {
                if (buff.Bp.BuffType == BuffType.Negative)
                {
                    buff.StackChange((int)Usual);
                    Activated?.Invoke();
                }
            }

            return attack.SwitchToEmpty();
        }


        [Effect("SG_CUR", Timing.OnKill)]
        private Attack SG_CUR(Attack attack, FighterData player, FighterData enemy)
        {
            if (enemy == null) return attack;
            player.Recover(BattleStatus.Hp((int)Usual), enemy, "SG_CUR");
            Activated?.Invoke();

            return attack;
        }

        [Effect("XD_CUR", Timing.OnReact)]
        private MapData XD_CUR(MapData map, FighterData player)
        {
            if (!(map is TotemSaveData)) return map;

            map.SquareState = SquareState.Done;
            Activated?.Invoke();
            var stack = (int)Usual;

            player.ApplySelfBuff(BuffData.Divinity(stack));
            player.ApplySelfBuff(BuffData.Vigor(stack));

            return map;
        }

        [Effect("LT_CUR", Timing.SkillEffect)]
        private void LT_CUR(FighterData player)
        {
            (player.Status.MDef, player.Status.PDef) = (player.Status.PDef, player.Status.MDef);
        }

        [Effect("SH_CUR", Timing.OnAttack)]
        private Attack SH_CUR(Attack attack, FighterData player, FighterData enemy)
        {
            if (attack.IsEmpty() || attack.IsCommonAttack) return attack;
            attack.Multi += Usual * Counter;
            Activated?.Invoke();
            return attack;
        }

        [Effect("SH_CUR", Timing.OnKill)]
        private Attack SH2_CUR(Attack attack, FighterData player, FighterData enemy)
        {
            Counter += 1;
            Activated?.Invoke();
            return attack;
        }

        [Effect("ZN_CUR", Timing.OnMarch)]
        private void ZN_CUR(FighterData player)
        {
            var count = (int)Usual;
            player.Strengthen(new BattleStatus(maxHp: count, maxMp: count));
            Activated?.Invoke();
        }

        [Effect("YS_CUR", Timing.OnAttack, priority: -10000)]
        private Attack YS_CUR(Attack attack, FighterData player, FighterData enemy)
        {
            foreach (var buff in player.Buffs)
            {
                if (buff.Bp.BuffType == BuffType.Negative)
                {
                    var count = (int)(Usual * buff.CurLv / 100);
                    buff.StackChange(-count);
                    player.ApplyBuff(new BuffData(buff.Id, count), enemy);
                }
            }

            return attack;
        }

        [Effect("BX_CUR", Timing.OnAttack)]
        private Attack BX_CUR(Attack attack, FighterData player, FighterData enemy)
        {
            if (attack.IsEmpty() || attack.IsCommonAttack) return attack;
            var count = (int)(Usual * player.Buffs.Count((data => data.Bp.BuffType == BuffType.Curse)));
            attack.CDmg += count;
            Activated?.Invoke();
            return attack;
        }

        [Effect("KY_CUR", Timing.OnAttack, priority: -10000)]
        private Attack KY_CUR(Attack attack, FighterData player, FighterData enemy)
        {
            foreach (var b in player.Buffs)
            {
                if (b.Bp.BuffType == BuffType.Curse)
                {
                    player.ApplyBuff((BuffData)b.Clone(), enemy);
                }
            }

            return attack.SwitchToEmpty();
        }

        [Effect("MW_CUR", Timing.OnAttack, priority: -10000)]
        private Attack MW_CUR(Attack attack, FighterData player, FighterData enemy)
        {
            var stack =
                (int)(Usual * player.Buffs.Sum((data => data.Bp.BuffType == BuffType.Negative ? data.CurLv : 0)));
            stack += (int)(Usual * enemy.Buffs.Sum((data => data.Bp.BuffType == BuffType.Curse ? data.CurLv : 0)));

            attack = new Attack(cAtk: stack, kw: "MW_CUR");

            enemy.SingleDefendSettle(attack, player);
            return attack.SwitchToEmpty();
        }

        [Effect("XG_CUR", Timing.OnAttack)]
        private Attack XG_CUR(Attack attack, FighterData player, FighterData enemy)
        {
            if (attack.IsEmpty()) return attack;
            if (SData.CurGameRandom.NextDouble() < Usual)
            {
                Activated?.Invoke();
                player.Kill(attack, enemy);
            }

            return attack;
        }

        [Effect("JQ_CUR", Timing.OnAttack, priority: -1000)]
        private Attack JQ_CUR(Attack attack, FighterData player, FighterData enemy)
        {
            var stack = (int)(enemy.Status.CurHp * Usual / 100);
            enemy.SingleDefendSettle(new Attack(cAtk: stack, kw: "JQ_CUR"), player);
            player.Recover(new BattleStatus(curHp: stack), enemy, "JQ_CUR");
            return attack.SwitchToEmpty();
        }


        /// <summary>
        /// random choose one of the following status that is higher than player's
        /// and swap it with player's
        /// </summary>
        /// <param name="attack"></param>
        /// <param name="player"></param>
        /// <param name="enemy"></param>
        /// <returns></returns>
        [Effect("KQ_CUR", Timing.OnAttack)]
        private Attack KQ_CUR(Attack attack, FighterData player, FighterData enemy)
        {
            (bool, bool, bool, bool) HigherThanPlayer = (false, false, false, false);
            if (enemy.Status.PAtk > player.Status.PAtk) HigherThanPlayer.Item1 = true;
            if (enemy.Status.MAtk > player.Status.MAtk) HigherThanPlayer.Item2 = true;
            if (enemy.Status.PDef > player.Status.PDef) HigherThanPlayer.Item3 = true;
            if (enemy.Status.MDef > player.Status.MDef) HigherThanPlayer.Item4 = true;

            var random = SData.CurGameRandom;
            var list = new List<int>();
            if (HigherThanPlayer.Item1) list.Add(0);
            if (HigherThanPlayer.Item2) list.Add(1);
            if (HigherThanPlayer.Item3) list.Add(2);
            if (HigherThanPlayer.Item4) list.Add(3);
            if (list.Count == 0) return attack.SwitchToEmpty();
            var index = list[random.Next(list.Count)];

            switch (index)
            {
                case 0:
                    (player.Status.PAtk, enemy.Status.PAtk) = (enemy.Status.PAtk, player.Status.PAtk);
                    break;
                case 1:
                    (player.Status.MAtk, enemy.Status.MAtk) = (enemy.Status.MAtk, player.Status.MAtk);
                    break;
                case 2:
                    (player.Status.PDef, enemy.Status.PDef) = (enemy.Status.PDef, player.Status.PDef);
                    break;
                case 3:
                    (player.Status.MDef, enemy.Status.MDef) = (enemy.Status.MDef, player.Status.MDef);
                    break;
            }

            return attack.SwitchToEmpty();
        }

        [Effect("KQ_CUR", Timing.OnMarch, alwaysActive: true)]
        private void KQ_CUR(FighterData player)
        {
            CooldownLeft = 0;
        }


        [Effect("EN_CUR", Timing.SkillEffect)]
        private void EN_CUR(FighterData player)
        {
            var curses = player.Buffs.Where((data => data.Bp.BuffType == BuffType.Curse));
            var curse = curses.ChooseRandom(SData.CurGameRandom);

            player.Purify(curse);

            if (player is PlayerData p)
            {
                p.TryTakePotion("cursepotion", out _);
            }
        }

        [Effect("EN_CUR", Timing.OnMarch, alwaysActive = true)]
        private void EN2_CUR(FighterData player)
        {
            CooldownLeft = 0;
        }


        [Effect("SCDJ_BAR", Timing.OnAttack, priority: -10000)]
        private Attack SCDJ_BAR(Attack attack, FighterData player, FighterData enemy)
        {
            attack.Change(pAtk: player.Status.PAtk, mAtk: player.Status.MAtk, multi: Usual);
            return attack;
        }

        [Effect("SL_BAR", Timing.OnAttack, priority: -10000)]
        private Attack SL_BAR(Attack attack, FighterData player, FighterData enemy)
        {
            var loseHp = player.Status.MaxHp - player.CurHp;
            player.ApplyBuff(BuffData.Feeble((int)(loseHp * Usual)), enemy);

            return attack.SwitchToEmpty();
        }

        [Effect("TZZ_BAR", Timing.OnAttack)]
        private Attack TZZ_BAR(Attack attack, FighterData player, FighterData enemy)
        {
            if (attack.IsEmpty() || attack.PAtk <= 0) return attack;
            var minus = max(0, enemy.CurHp - player.CurHp);

            if (minus > 0)
            {
                attack.PAtk += (int)(minus * Usual);
                Activated?.Invoke();
            }

            return attack;
        }

        [Effect("YNFM_BAR", Timing.OnAttack, priority: -10000)]
        private Attack YNFM_BAR(Attack attack, FighterData player, FighterData enemy)
        {
            attack.Change(mAtk: player.Status.PAtk, multi: Usual);
            return attack;
        }

        [Effect("XXDR_BAR", Timing.OnAttack, priority: -10000)]
        private Attack XXDR_BAR(Attack attack, FighterData player, FighterData enemy)
        {
            attack.Change(pAtk: player.Status.MAtk, multi: Usual);
            return attack;
        }

        [Effect("NHZS_BAR", Timing.OnAttack, priority: -10000)]
        private Attack NHZS_BAR(Attack attack, FighterData player, FighterData enemy)
        {
            Counter = 1;
            return attack.SwitchToEmpty();
        }

        [Effect("NHZS_BAR", Timing.OnDefendSettle, priority: -10000, alwaysActive: true)]
        private Attack XJ2_BAR(Attack attack, FighterData player, FighterData enemy)
        {
            if (Counter <= 0) return attack;
            Counter = 0;
            var stack = (int)(attack.SumDmg * Usual);

            player.ApplySelfBuff(BuffData.MPlus(stack));
            player.ApplySelfBuff(BuffData.PPlus(stack));
            Activated?.Invoke();
            return attack;
        }

        [Effect("GDDS_KNI", Timing.OnAttack)]
        private Attack GDDS_KNI(Attack attack, FighterData player, FighterData enemy)
        {
            if (attack.IsEmpty())
            {
                Counter = 1;
            }

            return attack;
        }

        [Effect("GDDS_KNI", Timing.OnDefendSettle, priority: 7, alwaysActive: true)]
        private Attack GDDS2_KNI(Attack attack, FighterData player, FighterData enemy)
        {
            if (Counter <= 0) return attack;
            Counter = 0;
            attack.Reduce(Usual / 100);
            Activated?.Invoke();
            return attack;
        }

        [Effect("GDDS_KNI", Timing.OnKill, priority: 7)]
        private Attack GDDS3_KNI(Attack attack, FighterData player, FighterData enemy)
        {
            Counter = 0;
            return attack;
        }

        [Effect("SZ_BAR", Timing.OnAttack, priority: -10000)]
        private Attack SZ_BAR(Attack attack, FighterData player, FighterData enemy)
        {
            attack.Change(pAtk: player.Status.PAtk, multi: Usual, combo: 3);
            return attack;
        }


        /// <summary>
        /// 击中时:每段的倍率比前一段增加#P1+P2*CurLv#(P1+P2*Lv)
        /// </summary>
        /// <param name="attack"></param>
        /// <param name="player"></param>
        /// <param name="enemy"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        [Effect("NH_BAR", Timing.OnStrike)]
        private Attack NH_BAR(Attack attack, FighterData player, FighterData enemy, int t)
        {
            if (attack.IsEmpty() || attack.Combo <= 1) return attack;
            attack.Multi += Usual * t;
            Activated?.Invoke();
            return attack;
        }

        [Effect("XM_BAR", Timing.OnStrengthen)]
        private BattleStatus XM_BAR(BattleStatus status, FighterData player)
        {
            if (SData.CurGameRandom.Next() < Usual)
            {
                if (status.PAtk > 0)
                {
                    player.Strengthen(new BattleStatus(mAtk: 1));
                    Activated?.Invoke();
                }

                if (status.MAtk > 0)
                {
                    player.Strengthen(new BattleStatus(pAtk: 1));
                    Activated?.Invoke();
                }
            }

            return status;
        }

        [Effect("DG_BAR", Timing.OnAttack, priority: -10000)]
        private Attack DG_BAR(Attack attack, FighterData player, FighterData enemy)
        {
            attack.Change(pAtk: player.Status.PAtk, multi: Usual);
            SwitchTo("LH_BAR");
            return attack;
        }

        [Effect("LH_BAR", Timing.OnAttack, priority: -10000)]
        private Attack LH_BAR(Attack attack, FighterData player, FighterData enemy)
        {
            attack.Change(mAtk: player.Status.MAtk, multi: Usual);
            SwitchTo("DG_BAR");
            return attack;
        }

        [Effect("FX_BAR", Timing.SkillEffect)]
        private void FX_BAR(FighterData player)
        {
            var stack = (int)(player.Status.MAtk * Usual);
            player.ApplySelfBuff(BuffData.PPlus(stack));
        }

        [Effect("FM_BAR", Timing.SkillEffect)]
        private void FM_BAR(FighterData player)
        {
            var stack = (int)(player.Status.PAtk * Usual);
            player.ApplySelfBuff(BuffData.MPlus(stack));
        }


        [Effect("JK_BAR", Timing.SkillEffect)]
        private void JK_BAR(FighterData player)
        {
            var stack = (int)Usual;
            player.ApplySelfBuff(BuffData.BloodLust(stack));
        }

        [Effect("JK_BAR", Timing.OnMarch, alwaysActive = true)]
        private void JK2_BAR(FighterData player)
        {
            CooldownLeft = 0;
        }


        /// <summary>
        /// 技能攻击时:若仅造成魔法伤害,获得#P1+P2*CurLv#(P1+P2*Lv)层物增;若仅造成物理伤害,获得#P1+P2*CurLv#(P1+P2*Lv)层魔增
        /// </summary>
        /// <param name="attack"></param>
        /// <param name="player"></param>
        /// <param name="enemy"></param>
        /// <returns></returns>
        [Effect("SSZW_BAR", Timing.OnAttack)]
        private Attack SSZW_BAR(Attack attack, FighterData player, FighterData enemy)
        {
            if (attack.IsEmpty() || attack.IsCommonAttack) return attack;
            if (attack.IsMagicAttack)
            {
                player.ApplySelfBuff(BuffData.PPlus((int)Usual));
                Activated?.Invoke();
            }
            else if (attack.IsPhysicalAttack)
            {
                player.ApplySelfBuff(BuffData.MPlus((int)Usual));
                Activated?.Invoke();
            }

            return attack;
        }

        [Effect("XZCB_BAR", Timing.OnReact, priority: 3)]
        private MapData XZCB_BAR(MapData map, FighterData player)
        {
            if (map.SquareState == SquareState.Done || !(map is TotemSaveData)) return map;

            if (player is PlayerData p)
            {
                //p.ApplySelfBuff(new BuffData("oblation", (int) Usual));
                Activated?.Invoke();
            }

            return map;
        }


        [Effect("NZ_BAR", Timing.SkillEffect)]
        private void NZ_BAR(FighterData player)
        {
            (player.Status.PAtk, player.Status.MAtk) = (player.Status.MAtk, player.Status.PAtk);
        }


        [Effect("XJ_BAR", Timing.OnGetSkillCost)]
        private CostInfo XJ_BAR(CostInfo cost, SkillData skill, FighterData player, bool isTry)
        {
            if (cost.CostType != CostType.Mp) return cost;
            cost.Value = (int)(cost.Value * Unusual);
            cost.CostType = CostType.Hp;
            Activated?.Invoke();
            return cost;
        }

        [Effect("XN_BAR", Timing.OnCounterCharge)]
        private BattleStatus XN_BAR(BattleStatus status, FighterData player, string kw)
        {
            if (status.CurHp > 0)
            {
                player.ApplySelfBuff(BuffData.PPlus((int)Usual));
                Activated?.Invoke();
            }

            return status;
        }

        [Effect("WJLZ_BAR", Timing.OnAttack, priority: -10000)]
        private Attack WJLZ_BAR(Attack attack, FighterData player, FighterData enemy)
        {
            attack.Change(pAtk: player.Status.PAtk, multi: Usual, combo: 9999);
            return attack;
        }

        [Effect("WJLZ_BAR", Timing.OnStrike, priority: -10000)]
        private Attack WJLZ2_BAR(Attack attack, FighterData player, FighterData enemy, int t)
        {
            player.CounterCharge(new BattleStatus(curHp: -t));
            return attack;
        }

        [Effect("RG_BAR", Timing.OnDefendSettle, priority: 10000)]
        private Attack RG_BAR(Attack attack, FighterData player, FighterData enemy)
        {
            if (attack.IsEmpty() || attack.SumDmg <= 0) return attack;

            var stack = (int)(attack.SumDmg * Usual);
            player.ApplySelfBuff(BuffData.Oblation(stack));
            return attack;
        }

        [Effect("XQ_BAR", Timing.OnAttack)]
        private Attack XQ_BAR(Attack attack, FighterData player, FighterData enemy)
        {
            if (attack.IsEmpty() || attack.IsCommonAttack) return attack;
            if (attack.CostInfo.CostType == CostType.Hp)
            {
                attack.PAtk += attack.CostInfo.ActualValue;
                Activated?.Invoke();
            }

            return attack;
        }


        [Effect("FC_BAR", Timing.OnAttack, priority: -10000)]
        private Attack FC_BAR(Attack attack, FighterData player, FighterData enemy)
        {
            attack.Change(pAtk: (int)(Counter * Usual));
            return attack;
        }

        [Effect("FC_BAR", Timing.OnDefendSettle, priority: 10000, alwaysActive = true)]
        private Attack FC2_BAR(Attack attack, FighterData player, FighterData enemy)
        {
            if (attack.IsEmpty() || attack.SumDmg <= 0) return attack;
            Counter += attack.SumDmg;
            Activated?.Invoke();
            return attack;
        }

        [Effect("FC_BAR", Timing.OnKill, alwaysActive = true)]
        private Attack FC3_BAR(Attack attack, FighterData player, FighterData enemy)
        {
            Counter = 0;
            return attack;
        }


        [Effect("CR_ALC", Timing.OnAttack, priority: -10000)]
        private Attack CR_ALC(Attack attack, FighterData player, FighterData enemy)
        {
            if (attack.IsEmpty()) return attack;
            var stack = player.Buffs.GetStack("poison");
            if (stack > 0)
            {
                player.ApplyBuff(BuffData.Poison((int)(stack * Usual)), enemy);
                Activated?.Invoke();
            }

            return attack;
        }

        [Effect("GS_KNI", Timing.SkillEffect)]
        private void GS_KNI(FighterData player)
        {
            var stack = player.Buffs.GetStack("sturdy");
            if (stack > 0)
            {
                player.ApplySelfBuff(BuffData.Thorn((int)(stack * Usual)));
                Activated?.Invoke();
            }
        }

        /// <summary>
        /// no longer cost mp, but poison self
        /// </summary>
        /// <param name="cost"></param>
        /// <param name="skill"></param>
        /// <param name="player"></param>
        /// <param name="isTry"></param>
        /// <returns></returns>
        [Effect("JB_ALC", Timing.OnGetSkillCost)]
        private CostInfo JB_ALC(CostInfo cost, SkillData skill, FighterData player, bool isTry)
        {
            if (cost.CostType != CostType.Mp) return cost;
            player.ApplySelfBuff(BuffData.Poison((int)Unusual * cost.ActualValue));
            cost.Value = 0;
            Activated?.Invoke();
            return cost;
        }

        #endregion
    }
}
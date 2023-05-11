using System;
using System.Collections.Generic;
using System.Linq;
using Managers;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using Tools;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game
{
    public class SkillData : BpData<Skill>, IEffectContainer, ICloneable
    {
        public int CooldownLeft;

        public int Counter;
        public int CurLv;

        public int InitCoolDown;
        public bool Sealed = false;

        [JsonIgnore] public bool IsEmpty => Id.IsNullOrWhitespace();


        [JsonIgnore] public override Skill Bp => SkillManager.Instance.GetById(Id.ToLower());

        public static SkillData Empty => new SkillData();
        private float Usual => Bp.Param1 + Bp.Param2 * CurLv;

        private bool InBattle => GameManager.Instance.InBattle;
        private SecondaryData SData => GameDataManager.Instance.SecondaryData;

        [JsonIgnore] private MapData CurrentMapData => GameManager.Instance.Focus.Data;


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
                fighter.Enemy.Strengthen(new BattleStatus() { MaxHp = (int)(fighter.Enemy.Status.MaxHp * Bp.Param1) });
            }
            else
            {
                fighter.Heal(new BattleStatus { CurHp = (int)Bp.Param1 * fighter.Status.MaxHp });
            }
        }


        [Effect("TYTQ_ALC", Timing.OnKill)]
        private Attack HumorialExtraction(Attack attack, FighterData fighter, FighterData enemy)
        {
            //var potion = PotionManager.Instance.
            fighter.Recover(BattleStatus.HP(CurLv), enemy);
            Activated?.Invoke();
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
            status.CurHp = math.max(status.CurHp, 0);
            Activated?.Invoke();
            return status;
        }


        [Effect("DYX_ALC", Timing.OnApply)]
        private BuffData DYX_ALC(BuffData buff, FighterData fighter)
        {
            if (buff.Id == "poison")
            {
                buff.StackChange((int)(CurLv * Bp.Param1));
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
                player.Heal(BattleStatus.HP(v));
            }
        }


        [Effect("DZXY_ALC", Timing.OnDefendSettle)]
        private Attack PoisonBlood(Attack attack, FighterData fighter, FighterData enemy)
        {
            if (enemy == null) return attack;

            if (attack.PDmg > 0)
            {
                var v = math.min(attack.PDmg, fighter.Status.CurHp);
                Activated?.Invoke();
                fighter.ApplyBuff(new BuffData("poison", (int)(CurLv * Bp.Param1 * v)), enemy);
            }

            return attack;
        }

        [Effect("ZH_ALC", Timing.OnDefendSettle)]
        private Attack SelfDestructive(Attack attack, FighterData fighter, FighterData enemy)
        {
            var c = Bp.Param1 + Bp.Param2 * CurLv;
            if (attack.SumDmg > 0)
            {
                if (SData.CurGameRandom.NextDouble() < c)
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
            fighter.Heal(new BattleStatus { CurHp = CurLv, CurMp = CurLv });
            Activated?.Invoke();
            return potion;
        }

        [Effect("ZN_ALC", Timing.OnCounterCharge)]
        private BattleStatus SelfAbuse(BattleStatus status, FighterData fighter, string kw)
        {
            fighter.RandomStrengthen();
            Activated?.Invoke();
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
            if (Random.Range(0, 1f) < ((PlayerData)fighter).LuckyChance * Bp.Param1)
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
            return new Attack(0, fighter.Status.MAtk, 0, Usual, 1, "HQ_MAG");
        }


        [Effect("ZZFD_MAG", Timing.OnAttack, priority = -100)]
        private Attack TraceMissile(Attack attack, FighterData fighter, FighterData enemy)
        {
            var multi = 2;
            if (enemy.Buffs.Any((data => data.Bp.BuffType == BuffType.Negative)))
            {
                multi += 2;
            }

            return new Attack
            {
                MAtk = fighter.Status.MAtk,
                Multi = multi,
                Combo = 1,
                Kw = "ZZFD_MAG",
            };
        }

        [Effect("ASFD_MAG", Timing.OnAttack, priority = -100)]
        private Attack ArcaneMissile(Attack attack, FighterData fighter, FighterData enemy)
        {
            return new Attack
            {
                MAtk = fighter.Status.MAtk,
                Multi = 1,
                Combo = (int)Bp.Param1,
                Kw = "ASFD_MAG"
            };
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
                player.CounterCharge(-BattleStatus.HP((int)(Bp.Param1 - Bp.Param2 * CurLv) * skill.CooldownLeft));
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
            return new Attack
            {
                MAtk = fighter.Status.MAtk,
                Multi = 1,
                Combo = (int)Bp.Param1,
                Kw = "ASFD_MAG"
            };
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
            player.Strengthen(new BattleStatus { MaxMp = (int)Bp.Param1 });
            Activated?.Invoke();
            return attack;
        }


        [Effect("YTZQ_MAG", Timing.OnAttack)]
        private Attack EtherBody(Attack attack, FighterData fighter, FighterData enemy)
        {
            var num = (int)((fighter.Status.MaxMp - fighter.Status.CurHp) / Bp.Param1) * Bp.Param2;
            attack.MAtk += (int)num;
            Activated?.Invoke();
            return attack;
        }

        [Effect("FLBZ_MAG", Timing.OnAttack)]
        private Attack FaLiBaoZou(Attack attack, FighterData fighter, FighterData enemy)
        {
            var num = attack.CostInfo.ActualValue;
            if (num != 0)
            {
                attack.MAtk += num;
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

                var maxAbsorb = math.max(fighter.Status.CurMp * Usual, attack.PDmg);

                fighter.Cost(new CostInfo((int)(maxAbsorb / Usual), CostType.Mp));

                attack.PDmg -= (int)maxAbsorb;
            }


            return attack;
        }


        [Effect("LJZL_MAG", Timing.OnAttackSettle)]
        private Attack LJZL_MAG(Attack attack, FighterData fighter, FighterData enemy)
        {
            if (fighter.Skills.CooldownRandomSkill(1))
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
            return new Attack(fighter.Status.PAtk, multi: Bp.Param1);
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

            return new Attack(fighter.Status.PAtk, multi: multi);
        }

        [Effect("CJBY_ASS", Timing.OnAttack, priority = -100)]
        private Attack CJBY_ASS(Attack attack, FighterData fighter, FighterData enemy)
        {
            var multi = Bp.Param1 + Bp.Param1 * CurLv;

            return new Attack(fighter.Status.PAtk, multi: multi);
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
            return new Attack(fighter.Status.PAtk, combo: (int)Usual);
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
                var g = enemy.Status.Gold * Usual;
                fighter.Gain((int)g);
                enemy.Gain(-(int)g);
                Activated?.Invoke();
            }

            return attack;
        }


        /// <summary>
        /// 购买技能金币消耗减少P1*Lv
        /// </summary>
        /// <param name="cost"></param>
        /// <param name="player"></param>
        /// <param name="kw"></param>
        /// <returns></returns>
        [Effect("SQRH_ASS", Timing.OnCost, priority = -100)]
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
            attack = new Attack(pAtk: (int)(enemy.CurHp * Usual));

            return attack;
        }


        [Effect("XX_MON", Timing.OnAttackSettle, priority = 2)]
        private Attack XX_MON(Attack attack, FighterData player, FighterData enemy)
        {
            player.Recover(BattleStatus.HP(attack.PDmg), enemy);
            return attack;
        }

        [Effect("LX_MON", Timing.OnAttack, priority = -10000)]
        private Attack LX_MON(Attack attack, FighterData attacker, FighterData enemy)
        {
            return new Attack(attacker.Status.PAtk, attacker.Status.MAtk, multi: 2f, kw: "LX_MON");
        }


        [Effect("YWSY_ALC", Timing.OnUsePotion)]
        private PotionData YWSY_ALC(PotionData potion, FighterData user)
        {
            var v = ((int)potion.Bp.Rank + 1) * Usual;
            user.Strengthen(new BattleStatus(maxHp: (int)v));
            return potion;
        }

        [Effect("TC_ALC", Timing.SkillEffect)]
        private FighterData TC_ALC(FighterData fighter)
        {
            var p = (PlayerData)fighter;
            foreach (var potion in p.Potions)
            {
                if (potion != null && !potion.Id.IsNullOrWhitespace() && potion.Count > 0)
                {
                    potion.Count += (int)Usual;
                }
            }

            return fighter;
        }

        [Effect("XFYJ_ALC", Timing.OnUsePotion)]
        private PotionData XFYJ_ALC(PotionData potion, FighterData user)
        {
            if (user.Skills.CooldownRandomSkill(1))
            {
                Activated?.Invoke();
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
            if (enemy != null)
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
            attack = new Attack(cAtk: fighter.Status.MAtk, multi: Usual);
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
            player.Heal(BattleStatus.HP((int)(Bp.Param1 + Bp.Param2 * CurLv)));
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
                status.CurHp = 0;
                status.MaxMp = (int)(status.CurMp * Usual / 100);
                status.CurMp = 0;
                Activated?.Invoke();
            }

            return status;
        }

        #endregion
    }
}
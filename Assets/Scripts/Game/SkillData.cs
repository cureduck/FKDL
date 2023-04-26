using System;
using System.Linq;
using System.Reflection;
using Managers;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game
{
    public class SkillData : BpData<Skill>, IEffectContainer, ICloneable
    {
        public int CurLv;

        public int Counter;

        public int InitCoolDown;
        public int CooldownLeft;
        public bool Sealed = false;
        
        [JsonIgnore] public bool IsEmpty => Id.IsNullOrWhitespace();

        public event Action<FighterData> OnLvUp;
        public event Action<FighterData> OnUnEquip;


        public void Load(Skill skill, int lv = 1)
        {
            CurLv = lv;
            Id = skill.Id;
            skill.Cooldown = 0;
            Sealed = skill.BattleOnly;
            Counter = 0;
        }
        
        
        
        public void LvUp(FighterData fighter, int lv = 1)
        {
            CurLv += lv;

            if (Bp.Fs.TryGetValue(Timing.OnLvUp, out var method))
            {
                method.Invoke(this, new object[]{fighter});
            }
            OnLvUp?.Invoke(fighter);
        }
        

        public void SetCooldown(int cd = default)
        {
            CooldownLeft = cd;
            CooldownLeft = CooldownLeft < 0 ? 0 : CooldownLeft;
            InitCoolDown = CooldownLeft;
        }


        public void BonusCooldown(int cd)
        {
            CooldownLeft -= cd;
            CooldownLeft = CooldownLeft < 0 ? 0 : CooldownLeft;
        }


        //public void SetCoolDown(int bonus = 0)
        //{
        //    Cooldown = math.max(0, Bp.Cooldown - bonus);
        //}
        
        public bool CanCast(out Info info, bool autoBroadcast)
        {
            if (!Bp.Positive)
            {
                info = new FailureInfo(FailureReason.SkillPassive, autoBroadcast);
                return false;
            }

            if ((Bp.BattleOnly) && (!GameManager.Instance.InBattle))
            {
                info = new FailureInfo(FailureReason.NoTarget, autoBroadcast);
                return false;
            }

            if (CooldownLeft > 0)
            {
                info = new FailureInfo(FailureReason.SkillNotReady, autoBroadcast);
                return false;
            }
            info = new SuccessInfo();
            return true;
        }


        
        
        
        

        [JsonIgnore] public override Skill Bp => SkillManager.Instance.GetById(Id.ToLower());

        [ShowInInspector] public event Action Activated;
        
        
        public override bool MayAffect(Timing timing, out int priority)
        {
            if (Sealed && !Bp.AlwaysActiveTiming.Contains(timing))
            {
                priority = 0;
                return false;
            }

            return base.MayAffect(timing, out priority);
        }

        public static SkillData Empty => new SkillData();
        
        
        public object Clone()
        {
            return MemberwiseClone();
        }

        public override string ToString()
        {
            return Id;
        }


        /*#region 具体技能

        [Effect("furious", Timing.OnAttack)]
        public Attack Furious(Attack attack, FighterData fighter, FighterData enemy)
        {
            fighter.Recover(new BattleStatus{CurHp = CurLv}, enemy);
            Activate?.Invoke();
            return attack;
        }


        [Effect("feast", Timing.OnKill)]
        public Attack Feast(Attack result, FighterData fighter, FighterData enemy)
        {
            fighter.Recover( new BattleStatus(){CurHp = (int)(CurLv*Bp.Param1)}, enemy);
            Activate?.Invoke();
            return result;
        }

        [Effect("burst", Timing.OnEquip)]
        public void BurstEquip(FighterData fighter)
        {
            fighter.Strengthen(new BattleStatus{PAtk = CurLv});
        }
        
        [Effect("burst", Timing.OnLvUp)]
        public void BurstLvUp(FighterData fighter)
        {
            fighter.Strengthen(new BattleStatus{PAtk = 1});
        }

        [Effect("burst", Timing.OnUnEquip)]
        public void BurstUnEquip(FighterData fighter)
        {
            fighter.Status.PAtk -= (int)Bp.Param1 * CurLv;
        }

        

        [Effect("BarBlood", Timing.OnStrengthen)]
        public BattleStatus BarBlood(BattleStatus modify, FighterData fighter)
        {
            if (Random.value < CurLv * .4f)
            {
                fighter.Strengthen(new BattleStatus{PAtk = 1});
                Activate?.Invoke();
            }
            return modify;
        }

        [Effect("Execution", Timing.OnKill)]
        public Attack Execution(Attack result, FighterData holder, FighterData enemy)
        {
            var overDamage = -enemy.Status.CurHp;
            holder.Gain(overDamage);
            Activate?.Invoke();
            return result;
        }
        
        
        [Effect("Genius", Timing.OnAttack)]
        public Attack Genius(Attack attack, FighterData fighter, FighterData enemy)
        {
            if ((enemy.Status.PAtk > fighter.Status.PAtk)&&(Random.Range(0,1f) < .2f))
            {
                fighter.Strengthen(new BattleStatus(){PAtk = 1});
                Activate?.Invoke();
            }
            return attack;
        }


        [Effect("thief", Timing.OnAttack)]
        public Attack Thief(Attack attack, FighterData fighter, FighterData enemy)
        {
            int gold = (int)(enemy.Gold * Bp.Param1 * CurLv);
            fighter.Gain(gold);
            enemy.Gain(-gold);
            Activate?.Invoke();
            return attack;
        }



        [Effect("Enlightenment", Timing.OnEquip)]
        public void EnlightenmentEquip(FighterData fighter)
        {
            var v = (int)Bp.Param1;
            fighter.Strengthen(new BattleStatus()
            {
                MaxHp = v,
                MaxMp = v,
                MAtk = v,
                PAtk = v,
                MDef = v,
                PDef = v
            });
            Activate?.Invoke();
        }
        
        [Effect("Enlightenment", Timing.OnUnEquip)]
        public void EnlightenmentUnEquip(FighterData fighter)
        {
            var v = -(int)Bp.Param2;
            fighter.Strengthen(new BattleStatus()
            {
                MaxHp = v,
                MaxMp = v,
                MAtk = v,
                PAtk = v,
                MDef = v,
                PDef = v
            });
        }
        
        [Effect("Enlightenment", Timing.OnLvUp)]
        public void EnlightenmentLvUp(FighterData fighter)
        {
            var v = (int)Bp.Param2 *2;
            fighter.Strengthen(new BattleStatus()
            {
                MaxHp = v,
                MaxMp = v,
                MAtk = v,
                PAtk = v,
                MDef = v,
                PDef = v
            });
            Activate?.Invoke();
        }
        
        [Effect("Enlightenment", Timing.OnMarch)]
        public void EnlightenmentMarch(FighterData fighter)
        {
            var v = -(int)Bp.Param2;
            fighter.Strengthen(new BattleStatus()
            {
                MaxHp = v,
                MaxMp = v,
                MAtk = v,
                PAtk = v,
                MDef = v,
                PDef = v
            });
            Activate?.Invoke();
        }
        
        

        [Effect("poison blood", Timing.OnSettle)]
        public Attack PoisonAttack(Attack result, FighterData fighter, FighterData enemy)
        {
            if (result.SumDmg > 0)
            {
                Activate?.Invoke();
                enemy.Defend(new Attack() {MAtk = result.SumDmg}, fighter);
            }

            return result;
        }


        [Effect("bloodlust", Timing.OnHeal)]
        public BattleStatus BloodlustHeal(BattleStatus modifier, FighterData fighterData)
        {
            Activate?.Invoke();
            return modifier * 0.5f;
        }
        
        
        [Effect("bloodlust", Timing.OnRecover)]
        public BattleStatus bloodlustRecover(BattleStatus modifier, FighterData fighter, FighterData enemy)
        {
            Activate?.Invoke();
            return modifier * 2f;
        }
        
        [Effect("strong", Timing.OnAttack)]
        public Attack Strong(Attack attack, FighterData fighter, FighterData enemy)
        {
            Activate?.Invoke();
            attack.PAtk += (int)(fighter.Status.CurHp * Bp.Param1);
            return attack;
        }


        [Effect("subjugate", Timing.OnDefend)]
        public Attack Subjugate(Attack attack, FighterData fighter, FighterData enemy)
        {
            var diff = fighter.Status.PAtk - attack.PAtk;
            if (diff > 0)
            {
                Activate?.Invoke();
                attack.PAtk -= (int)(diff * Bp.Param1 * CurLv);
            }

            return attack;
        }

        [Effect("well-Laid Plans", Timing.OnHeal)]
        public BattleStatus WellLaid(BattleStatus modify, FighterData fighter)
        {
            var diff = modify.CurHp - fighter.LossHp;

            if (diff > 0)
            {
                modify.CurHp -= diff;
                Activate?.Invoke();
                diff = (int)(diff * CurLv * Bp.Param1);
                fighter.Strengthen(new BattleStatus{MaxHp = diff});
            }
            
            
            return modify;
        }


        [Effect("Anger", Timing.SkillEffect)]
        public void Anger(FighterData fighter)
        {
            fighter.AppliedBuff(new BuffData
            {
                CurLv = CurLv,
                Id = "Anger"
            });
        }


        [Effect("One", Timing.SkillEffect)]
        public void One(FighterData fighter, FighterData enemy)
        {
            var atk = fighter.ForgeAttack(enemy);
            
        }
        
        
        #endregion*/

        private bool InBattle => GameManager.Instance.InBattle;
        private SecondaryData SData => GameDataManager.Instance.SecondaryData;
        
        [JsonIgnore] private MapData CurrentMapData => GameManager.Instance.Focus.Data;

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
                var b = fighter.ApplyBuff(new BuffData("pminus", (int) Bp.Param1));
                fighter.Enemy.AppliedBuff(b);
            }
            else
            {
                fighter.ApplySelfBuff(new BuffData("pplus", (int) Bp.Param1));
            }

        }
        
        
        [Effect("XRLC_ALC", Timing.SkillEffect)]
        private void FleshTrans(FighterData fighter)
        {
            if (InBattle)
            {
                fighter.Enemy.Strengthen(new BattleStatus(){MaxHp = (int)(fighter.Enemy.Status.MaxHp * Bp.Param1)});
            }
            else
            {
                fighter.Heal(new BattleStatus{CurHp = (int)Bp.Param1 * fighter.Status.MaxHp});
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
            fighter.Recover(new BattleStatus{CurHp = CurLv}, enemy);
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
            var v = (int) Bp.Param1 + (int) Bp.Param2 * CurLv;
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
                fighter.ApplyBuff(new BuffData("poison",  (int)(CurLv * Bp.Param1* v)), enemy);
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
            fighter.Heal(new BattleStatus{CurHp = CurLv, CurMp = CurLv});
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
                ((PlayerData) fighter).TryTakePotion(potion.Id, out _);
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
            fighter.Heal(BattleStatus.HP(10));
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
                ((PlayerData)player).ApplySelfBuff(new BuffData("Mplus", CurLv));
            }
            Activated?.Invoke();
        }
        
        
        [Effect("MS_MAG", Timing.OnKill)]
        private Attack MieShi(Attack attack, FighterData player, FighterData enemy)
        {
            player.Strengthen(new BattleStatus{MaxMp = (int)Bp.Param1});
            Activated?.Invoke();
            return attack;
        }
        

        [Effect("YTZQ_MAG", Timing.OnAttack)]
        private Attack EtherBody(Attack attack, FighterData fighter, FighterData enemy)
        {
            var num = (int) ((fighter.Status.MaxMp - fighter.Status.CurHp) / Bp.Param1) * Bp.Param2;
            attack.MAtk += (int)num;
            Activated?.Invoke();
            return attack;
        }
        
        [Effect("FLBZ_MAG", Timing.OnAttack)]
        private Attack FaLiBaoZou(Attack attack, FighterData fighter, FighterData enemy)
        {
            var num = attack.CostInfo.ActualValue;
            attack.MAtk += num;
            Activated?.Invoke();
            return attack;
        }

        [Effect("ADLF_MAG", Timing.OnAttack)]
        private Attack ADLF_MAG(Attack attack, FighterData fighter, FighterData enemy)
        {
            if (attack.Combo > 1)
            {
                attack.Combo += 1;
                SetCooldown(Bp.Cooldown);
                Activated?.Invoke();
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
            return attack;
        }
        
        [Effect("XJ_ASS", Timing.OnAttack, priority = -100)]
        private Attack XJ_ASS(Attack attack, FighterData fighter, FighterData enemy)
        {
            var multi = ((PlayerData) fighter).Engaging ? Bp.Param1 * CurLv + Bp.Param1 : Bp.Param1;

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

            return new Attack(fighter.Status.PAtk, combo: (int) Usual);
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
                var g = enemy.Status.Gold * (Bp.Param1 + Bp.Param2 * CurLv);
                fighter.Gain((int)g);
                enemy.Gain(-(int)g);
            }
            return attack;
        }
        
        
        
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
        
        [Effect("TL_ASS", Timing.OnGain, priority = 100)]
        private int TL_ASS(int coin, FighterData fighter, string kw)
        {
            fighter.ApplySelfBuff(new BuffData("pplus", (int)Usual));
            Activated?.Invoke();
            return coin;
        }
        
        [Effect("GHXS_ASS", Timing.OnKill, priority = 100)]
        private Attack GHXS_ASS(Attack attack, FighterData fighter, FighterData enemy)
        {
            fighter.Gain((int)Usual);
            Activated?.Invoke();
            return attack;
        }
        
        [Effect("YN_ASS", Timing.OnAttack, priority = -10000)]
        private Attack YN_ASS(Attack attack, FighterData fighter, FighterData enemy)
        {
            Counter += 1;
            if (Counter >= Bp.Param1 - CurLv && !((PlayerData)fighter).Engaging)
            {
                Counter = 0;
                ((PlayerData) fighter).Engaging = true;
                Debug.Log("YN!");
                Activated?.Invoke();
            }
            
            return attack;
        }
        
        
        [Effect("test1_com", Timing.OnAttack, priority = -10000)]
        private Attack test1_com(Attack attack, FighterData fighter, FighterData enemy)
        {
            return new Attack(pAtk : fighter.Status.PAtk, combo: 3);
        }
        
        [Effect("test2_com", Timing.OnAttack, priority = -10000)]
        private Attack test2_com(Attack attack, FighterData fighter, FighterData enemy)
        {
            return new Attack(pAtk : fighter.Status.PAtk, mAtk: fighter.Status.MAtk, cAtk: 10, combo: 3);
        }
        
        
        #endregion

        private float Usual => Bp.Param1 + Bp.Param2 * CurLv;

    }
}
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

        //public void SetCoolDown(int bonus = 0)
        //{
        //    Cooldown = math.max(0, Bp.Cooldown - bonus);
        //}
        
        public bool CanCast(out Info info)
        {
            if (!Bp.Positive)
            {
                info = new FailureInfo(FailureReason.SkillPassive);
                return false;
            }

            if ((Bp.BattleOnly) && (!GameManager.Instance.InBattle))
            {
                info = new FailureInfo(FailureReason.NoTarget);
                return false;
            }

            if (CooldownLeft > 0)
            {
                info = new FailureInfo(FailureReason.SkillNotReady);
                return false;
            }
            info = new SuccessInfo();
            return true;
        }


        
        
        
        

        [JsonIgnore] public override Skill Bp => SkillManager.Instance.GetById(Id.ToLower());

        [ShowInInspector] public event Action Activate;
        
        
        
        public override bool MayAffect(Timing timing, out int priority)
        {
            if (Sealed)
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
        private PlayerData Player => GameManager.Instance.PlayerData;
        
        
        [JsonIgnore] private MapData CurrentMapData => GameManager.Instance.Focus.Data;

        #region 正式技能

        
        [Effect("YWLZ_ALC", Timing.SkillEffect)]
        private void BrewPotion(FighterData fighter)
        {
            var p = PotionManager.Instance.RollT(Rank.Normal).First();
            Player.TryTakeOffer(new Offer(p), out _);
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
        
        
        [Effect("YRLC_ALC", Timing.SkillEffect)]
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
            Activate?.Invoke();
            return attack;
        }
        
        
        [Effect("JPX_ALC", Timing.OnAttack)]
        private Attack Anatomy(Attack attack, FighterData fighter, FighterData enemy)
        {
            fighter.Recover(new BattleStatus{CurHp = CurLv}, enemy);
            Activate?.Invoke();
            return attack;
        }
        
        [Effect("NYX_ALC", Timing.OnCounterCharge)]
        private BattleStatus DrugResistance(BattleStatus status, FighterData fighter, string kw)
        {
            status.CurHp -= (int)(Bp.Param1 * CurLv);
            status.CurHp = math.max(status.CurHp, 0);
            Activate?.Invoke();
            return status;
        }
        
        
        [Effect("DZXY_ALC", Timing.OnDefendSettle)]
        private Attack PoisonBlood(Attack attack, FighterData fighter, FighterData enemy)
        {
            if (attack.SumDmg > 0)
            {
                Activate?.Invoke();
                fighter.ApplyBuff(new BuffData("poison", attack.SumDmg), enemy);
            }

            return attack;
        }
        
        [Effect("ZHQX_ALC", Timing.OnDefendSettle)]
        private Attack SelfDestructive(Attack attack, FighterData fighter, FighterData enemy)
        {
            if (attack.SumDmg > 0)
            {
                if (Random.Range(0f, 1f) < .3f)
                {
                    fighter.RandomStrengthen();
                    Activate?.Invoke();
                }
            }
            return attack;
        }
        
        [Effect("MY_ALC", Timing.OnUsePotion)]
        private PotionData MagicAddiction(PotionData potion, FighterData fighter)
        {
            fighter.Heal(new BattleStatus{CurHp = CurLv, CurMp = CurLv});
            Activate?.Invoke();
            return potion;
        }
        
        [Effect("ZN_ALC", Timing.OnCounterCharge)]
        private BattleStatus SelfAbuse(BattleStatus status, FighterData fighter, string kw)
        {
            fighter.RandomStrengthen();
            Activate?.Invoke();
            return status;
        }
        
        
        [Effect("BXTY_ALC", Timing.OnStrengthen)]
        private Attack NDE(Attack attack, FighterData fighter, FighterData enemy)
        {
            
            fighter.Recover(new BattleStatus{CurHp = CurLv}, enemy);
            Activate?.Invoke();
            return attack;
        }

        [Effect("JLYJ_ALC", Timing.OnUsePotion)]
        private PotionData RefiningElixir(PotionData potion, FighterData fighter)
        {
            if (Random.Range(0, 1f) < .2f)
            {
                var p = potion;
                ((PlayerData) fighter).TryTakePotion(p.Id, out _);
                Activate?.Invoke();
            }

            return potion;
        }


        [Effect("HQ_MAG", Timing.OnAttack, priority = -100)]
        private Attack FireBall(Attack attack, FighterData fighter, FighterData enemy)
        {
            return new Attack(0, fighter.Status.MAtk, 0, Bp.Param1, 1, "HQ_MAG");
        }


        [Effect("ZZFD_MAG", Timing.OnAttack, priority = -100)]
        private Attack TraceMissile(Attack attack, FighterData fighter, FighterData enemy)
        {
            return new Attack
            {
                MAtk = fighter.Status.MAtk,
                Multi = 2,
                Combo = 1,
                Id = "ZZFD_MAG",
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
                Id = "ASFD_MAG"
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
                Id = "ASFD_MAG"
            };
        }

        [Effect("JZ_MAG", Timing.OnPreAttack)]
        private void Arrogance(PlayerData player, FighterData enemy)
        {
            if (player.Engaging)
            {
                ((PlayerData)player).ApplySelfBuff(new BuffData("surging", CurLv));
            }
            Activate?.Invoke();
        }
        
        
        [Effect("MS_MAG", Timing.OnKill)]
        private Attack MieShi(Attack attack, FighterData player, FighterData enemy)
        {
            player.Strengthen(new BattleStatus{MaxMp = (int)Bp.Param1});
            Activate?.Invoke();
            return attack;
        }
        

        [Effect("YTZQ_MAG", Timing.OnAttack)]
        private Attack EtherBody(Attack attack, FighterData fighter, FighterData enemy)
        {
            var num = (int) ((fighter.Status.MaxMp - fighter.Status.CurHp) / Bp.Param1) * Bp.Param2;
            attack.MAtk += (int)num;
            Activate?.Invoke();
            return attack;
        }
        
        [Effect("FLBZ_MAG", Timing.OnAttack)]
        private Attack FaLiBaoZou(Attack attack, FighterData fighter, FighterData enemy)
        {
            var num = attack.CostInfo.ActualValue;
            attack.MAtk += num;
            Activate?.Invoke();
            return attack;
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
                Activate?.Invoke();
            }
            attack.PAtk -= count;
            return attack;
        }

        [Effect("XYLC_ALC", Timing.SkillEffect)]
        private void XYLC_ALC(FighterData player)
        {
            var v = (int) Bp.Param1 + (int) Bp.Param2 * CurLv;
            if (GameManager.Instance.InBattle)
            {
                player.Enemy.Suffer(new Attack(cAtk: v));
            }
            else
            {
                player.Heal(BattleStatus.HP(v));
            }
        }
        
        #endregion

    }
}
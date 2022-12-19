using System;
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
    public class SkillData : IEffectContainer
    {
        public string Id;
        public int CurLv;

        public int Counter;
        
        public int Cooldown;
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

        public void UnEquip(FighterData fighter)
        {
            if (Bp.Fs.TryGetValue(Timing.OnUnEquip, out var method))
            {
                method.Invoke(this, new object[]{fighter});
            }
            OnUnEquip?.Invoke(fighter);
        }

        public void SetCooldown(int bonus = 0)
        {
            Cooldown = Bp.Cooldown - bonus;
        }


        public bool CanCast => true;

        [JsonIgnore] public bool IsBattleSkill => Bp.Fs[Timing.SkillEffect] == null;


        [JsonIgnore] public Skill Bp => SkillManager.Instance.Lib[Id.ToLower()];
        [ShowInInspector] public event Action Activate;
        
        
        
        public bool MayAffect(Timing timing, out int priority)
        {
            if (Sealed)
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

        public static SkillData Empty => new SkillData();


        #region 具体技能

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
            if (result.Sum > 0)
            {
                Activate?.Invoke();
                enemy.Defend(new Attack() {MAtk = result.Sum}, fighter);
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
        
        
        #endregion


        #region 正式技能

        [JsonIgnore] private MapData CurrentMapData => GameManager.Instance.Focus.Data;
        
        [Effect("brew potion", Timing.SkillEffect)]
        public void BrewPotion(FighterData fighter)
        {
            var p = Provider.Instance.CreateRandomPotion(Rank.Normal);
            GameManager.Instance.PlayerData.TryTake(new Offer(){Id = p.Id, Kind = Offer.OfferKind.Potion});
        }

        [Effect("metal transformation", Timing.SkillEffect)]
        public void MetalTrans(FighterData fighter)
        {
            fighter.ApplySelfBuff(new BuffData{Id = "Bellow", CurLv = (int)Bp.Param1});
        }

        [Effect("blood transformation", Timing.OnCost)]
        public void BloodTrans(FighterData fighter)
        {
            fighter.Heal(new BattleStatus{CurHp = (int)Bp.Param1});
        }
        
        [Effect("flesh transformation", Timing.OnCost)]
        public void FleshTrans(FighterData fighter)
        {
            
        }
        
        
        [Effect("humoral extraction", Timing.OnKill)]
        public Attack HumoralExtraction(Attack attack, FighterData fighter, FighterData enemy)
        {
            //var potion = PotionManager.Instance.
            fighter.Recover(new BattleStatus{CurHp = CurLv}, enemy);
            Activate?.Invoke();
            return attack;
        }
        
        /// <summary>
        /// 解剖学
        /// </summary>
        /// <param name="attack"></param>
        /// <param name="fighter"></param>
        /// <param name="enemy"></param>
        /// <returns></returns>
        [Effect("anatomy", Timing.OnAttack)]
        public Attack Anatomy(Attack attack, FighterData fighter, FighterData enemy)
        {
            fighter.Recover(new BattleStatus{CurHp = CurLv}, enemy);
            Activate?.Invoke();
            return attack;
        }
        
        [Effect("drug resistance", Timing.OnAttack)]
        public Attack DrugResistance(Attack attack, FighterData fighter, FighterData enemy)
        {
            fighter.Recover(new BattleStatus{CurHp = CurLv}, enemy);
            Activate?.Invoke();
            return attack;
        }
        
        
        [Effect("poison blood", Timing.OnAttack)]
        public Attack PoisonBlood(Attack attack, FighterData fighter, FighterData enemy)
        {
            fighter.Recover(new BattleStatus{CurHp = CurLv}, enemy);
            Activate?.Invoke();
            return attack;
        }
        
        [Effect("self-destructive", Timing.OnAttack)]
        public Attack SelfDestructive(Attack attack, FighterData fighter, FighterData enemy)
        {
            fighter.Recover(new BattleStatus{CurHp = CurLv}, enemy);
            Activate?.Invoke();
            return attack;
        }
        
        [Effect("magic addiction", Timing.OnAttack)]
        public Attack MagicAddiction(Attack attack, FighterData fighter, FighterData enemy)
        {
            fighter.Recover(new BattleStatus{CurHp = CurLv}, enemy);
            Activate?.Invoke();
            return attack;
        }
        
        [Effect("self-abuse", Timing.OnAttack)]
        public Attack SelfAbuse(Attack attack, FighterData fighter, FighterData enemy)
        {
            fighter.Recover(new BattleStatus{CurHp = CurLv}, enemy);
            Activate?.Invoke();
            return attack;
        }
        
        
        [Effect("near-death-experience", Timing.OnAttack)]
        public Attack NDE(Attack attack, FighterData fighter, FighterData enemy)
        {
            fighter.Recover(new BattleStatus{CurHp = CurLv}, enemy);
            Activate?.Invoke();
            return attack;
        }
        
        [Effect("refining elixir", Timing.OnAttack)]
        public Attack RefiningElixir(Attack attack, FighterData fighter, FighterData enemy)
        {
            fighter.Recover(new BattleStatus{CurHp = CurLv}, enemy);
            Activate?.Invoke();
            return attack;
        }
        
        
        [Effect("ether body", Timing.OnAttack)]
        public Attack EtherBody(Attack attack, FighterData fighter, FighterData enemy)
        {
            fighter.Recover(new BattleStatus{CurHp = CurLv}, enemy);
            Activate?.Invoke();
            return attack;
        }

        
        
        
        
        
        

        #endregion
    }
}
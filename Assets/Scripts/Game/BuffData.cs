using System;
using System.Collections;
using System.Reflection;
using Managers;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UI;
using UnityEngine;
using UnityEngine.Assertions.Must;

namespace Game
{
    public class BuffData : BpData<Buff>, IEffectContainer, ICloneable, IActivated
    {
        [ShowInInspector] public int CurLv { get; private set; }
        [JsonIgnore] public override Buff Bp => BuffManager.Instance.TryGetById(Id, out var buff)? buff : null;


        public void StackChange(int value)
        {
            CurLv += value;
            if (CurLv == 0)
            {
                Remove();
            }

            if (CurLv < 0)
            {
                if (Bp.OppositeBp != null)
                {
                    Id = Bp.OppositeId;
                    CurLv = -CurLv;
                }
                else
                {
                    Remove();
                }
            }
        }
        
        [JsonConstructor]
        public BuffData(string id, int curLv)
        {
            Id = id.ToLower();
            CurLv = curLv;
        }

        public BuffData()
        {
        }

        public event Action Removed;

        public void Remove()
        {
            Removed?.Invoke();
        }
        
        public event Action Activated;
        

        public override string ToString()
        {
            return $"{Id}:{CurLv}";
        }


        #region 具体效果

        [Effect("PPlus", Timing.OnAttack, priority = -4)]
        private Attack Anger(Attack attack, FighterData f1, FighterData f2)
        {
            attack.PAtk += CurLv;
            CurLv -= 1;
            Activated?.Invoke();
            return attack;
        }

        [Effect("MPlus", Timing.OnAttack, priority = -4)]
        private Attack Surging(Attack attack, FighterData f1, FighterData f2)
        {
            attack.MAtk += CurLv;
            CurLv -= 1;
            Activated?.Invoke();
            return attack;
        }
        
        [Effect("PMinus", Timing.OnAttack, priority = -4)]
        private Attack PMinus(Attack attack, FighterData f1, FighterData f2)
        {
            attack.PAtk -= CurLv;
            CurLv -= 1;
            Activated?.Invoke();
            return attack;
        }

        [Effect("MMinus", Timing.OnAttack, priority = -4)]
        private Attack MMinus(Attack attack, FighterData f1, FighterData f2)
        {
            attack.MAtk -= CurLv;
            CurLv -= 1;
            Activated?.Invoke();
            return attack;
        }
        
        
        [Effect("Poison", Timing.BeforeAttack, priority = -4)]
        private void Poison(FighterData f1, FighterData f2)
        {
            f1.SingleDefendSettle(new Attack(mAtk: CurLv, kw : "poison"), null);
            CurLv -= 1;
            Activated?.Invoke();
        }
        
        [Effect("Bellow", Timing.OnAttack, priority = -4)]
        private Attack Bellow(Attack attack, FighterData f1, FighterData f2)
        {
            if (attack.PAtk > 0) attack.PAtk += CurLv;
            if (attack.MAtk > 0) attack.MAtk += CurLv;
            if (attack.CAtk > 0) attack.CAtk += CurLv;
            
            CurLv = 0;
            Activated?.Invoke();
            return attack;
        }

        [Effect("Divinity", Timing.OnAttack, priority = -4)]
        private Attack Divinity(Attack attack, FighterData f1, FighterData f2)
        {
            attack.Multi += 1;
            CurLv -= 1;
            Activated?.Invoke();
            return attack;
        }

        [Effect("Vigor", Timing.OnAttack, priority = -4)]
        private Attack Vigor(Attack attack, FighterData f1, FighterData f2)
        {
            attack.Combo += 1;
            CurLv -= 1;
            Activated?.Invoke();
            return attack;
        }
        
        
        [Effect("Weaken", Timing.OnAttack, priority = -4)]
        private Attack Weaken(Attack attack, FighterData f1, FighterData f2)
        {
            attack.Combo -= 1;
            CurLv -= 1;
            Activated?.Invoke();
            return attack;
        }
        
        [Effect("Thorn", Timing.OnDefend, priority = -4)]
        private Attack Thorn(Attack attack, FighterData f1, FighterData f2)
        {
            if (f2 == null || !f2.IsAlive) return attack;
            
            f2.SingleDefendSettle(new Attack(pAtk: CurLv), null);
            CurLv -= 1;
            Activated?.Invoke();
            return attack;
        }
        
        [Effect("Flaming", Timing.OnAttack, priority = -4)]
        private Attack Flaming(Attack attack, FighterData f1, FighterData f2)
        {
            attack.MAtk += CurLv;
            CurLv -= 1;
            Activated?.Invoke();
            return attack;
        }


        [Effect("buffer", Timing.OnDefendSettle, priority = 1000)]
        private Attack Buffer(Attack attack, FighterData f1, FighterData f2)
        {
            attack.PDmg = 0;
            attack.PDmg = 0;
            attack.CDmg = 0;
            return attack;
        }


        [Effect("Clarity", Timing.OnGetSkillCost, priority = 3)]
        private CostInfo Clarity(CostInfo cost,SkillData skill,  FighterData f1, bool test = true)
        {
            if (cost.ActualValue > 0)
            {
                cost.Discount -= .5f;
                if (!test) CurLv -= 1;
                Activated?.Invoke();
            }
            return cost;
        }
        
        [Effect("Leakage", Timing.OnGetSkillCost, priority = -1)]
        private CostInfo Leakage(CostInfo cost, SkillData skill, FighterData f1, bool test = true)
        {
            if (cost.ActualValue > 0)
            {
                cost.Discount += .5f;
                if (!test) CurLv -= 1;
                Activated?.Invoke();
            }
            return cost;
        }

        [Effect("BloodLust", Timing.OnAttackSettle, priority = 100)]
        private Attack BloodLust(Attack attack, FighterData f1, FighterData f2)
        {
            f1.Recover(BattleStatus.HP(attack.SumDmg), f2);
            CurLv = 0;
            return attack;
        }


        [Effect("Swift", Timing.OnSetCoolDown)]
        private SkillData Swift(SkillData skill, FighterData fighter)
        {
            skill.InitCoolDown -= 1;
            skill.BonusCooldown(1);
            Activated?.Invoke();
            return skill;
        }
        #endregion

        public object Clone()
        {
            return MemberwiseClone();
        }

        public static BuffData Leakage(int stack = 1)
        {
            return new BuffData("Leakage", stack);
        }

        public static BuffData PPlus(int stack = 1)
        {
            return new BuffData("PPlus", stack);
        }

    }
}
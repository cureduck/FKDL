using System;
using System.Collections.Generic;
using System.Reflection;
using Managers;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;
using Object = System.Object;

namespace Game
{
    public class FighterData : MapData
    {
        public BattleStatus Status;
        public int Gold;
        [ShowInInspector] public SkillAgent Skills;
        [ShowInInspector] public BuffAgent Buffs;

        private Attack InitAttack(SkillData skill = null)
        {
            return new Attack
            {
                PAtk = Status.PAtk,
                MAtk = 0,
                CAtk = 0,
                Skill = skill
            };
        }

        public bool IsPlayer => this == GameManager.Instance.PlayerData;

        public Attack Defend(Attack attack, FighterData enemy)
        {
            
            attack.PDmg = math.max(0, attack.PAtk - Status.PDef);
            attack.MDmg = math.max(0, attack.MAtk - Status.MDef);
            attack.CDmg = attack.CDmg;
            
            attack = CheckChain<Attack>(Timing.OnDefend, new object[] {attack, this, enemy});

            Status.CurHp -= attack.Sum;
            
            Updated();
            
            if ((Status.CurHp <= 0)&&(this is EnemySaveData))
            {
                Updated();
            }
            
            return attack;
        }


        public Attack Suffer(Attack attack)
        {
            return attack;
        }
        


        public Attack ForgeAttack(FighterData target, SkillData skillData = null)
        {
            if (skillData == null)
            {
                var atk = InitAttack();
                atk = CheckChain<Attack>(Timing.OnAttack, new object[] {atk, this, target});
                return atk;
            }
            else
            {
                skillData.Sealed = false;
                var atk = InitAttack(skillData);
                atk = CheckChain<Attack>(Timing.OnAttack, new object[] {atk, this, target});
                skillData.Sealed = true;
                skillData.SetCooldown();
                return atk;
            }
            
        }
        
        
        /// <summary>
        /// 进攻时的触发条件
        /// </summary>
        /// <param name="r"></param>
        /// <param name="enemy"></param>
        /// <returns></returns>
        public Attack Settle(Attack r, FighterData enemy)
        {

            CheckChain<Attack>(Timing.OnDefend, new object[] {r, this, enemy});
            
            CoolDown();
            Buffs.RemoveZeroStackBuff();
            return r;
        }


        public Attack Kill(Attack r, FighterData enemy)
        {
            r = CheckChain<Attack>(Timing.OnKill, new object[] {r, this, enemy});
            
            return r;
        }


        public void Gain(int gold)
        {
            foreach (var skill in Skills)
            {
                if ((!skill.IsEmpty)&&(skill.Bp.Fs.ContainsKey(Timing.OnGain)))
                {
                    var f = skill.Bp.Fs[Timing.OnGain];
                    //modify = (int) f?.Invoke(skill, new object[]{gold, this});
                }
            }

            Gold += gold;
            Updated();
        }


        public void Cost(BattleStatus modify, string kw = null)
        {
            modify = CheckChain<BattleStatus>(Timing.OnCost, new object[] {modify, this, kw});
            Status -= modify;
            Updated();
        }
        
        
        public void CounterCharge(BattleStatus modify, string kw = null)
        {
            modify = CheckChain<BattleStatus>(Timing.OnCounterCharge, new object[] {modify, this, kw});
            Status -= modify;
            Updated();
        }
        
        
        
        protected void Equip(SkillData sk)
        {
            /*foreach (var pi in typeof(SkillData).GetMethods())
            {
                var msg = pi.GetCustomAttribute<EffectAttribute>();
                if ((msg == null)||(msg.id != sk.Id)) continue;

                switch (msg.timing)
                {
                    case Timing.OnEquip:
                        var f2 = (Action<FighterData>) pi.CreateDelegate(typeof(Action<FighterData>), sk);
                        f2.Invoke((FighterData) this);
                        break;
                    default:
                        break;
                }
            }*/

            if (sk.Bp.Fs.TryGetValue(Timing.OnEquip, out var f))
            {
                f = sk.Bp.Fs[Timing.OnEquip];
                f.Invoke(sk, new object[] {this});
                Updated();
            }

            if ((sk.Bp.Positive)&&(sk.Bp.NeedTarget))
            {
                sk.Sealed = true;
            }
        }
        
        public void OnUnEquip(SkillData sk)
        {
            var f = sk.Bp.Fs[Timing.OnUnEquip];
            f.Invoke(this, new object[] {sk, this});
            Updated();
        }
        
        
        protected void Load(SkillData sk)
        {
            foreach (var pi in typeof(SkillData).GetMethods())
            {
                var msg = pi.GetCustomAttribute<EffectAttribute>();
                if ((msg == null)||(msg.id != sk.Id)) continue;
            }
            Updated();
        }
        
        
        
        [Button]
        public void Strengthen(BattleStatus modify)
        {
            modify = CheckChain<BattleStatus>(Timing.OnStrengthen, new object[] {modify, this});
            this.Status += modify;
            Updated();
        }
        

        [Button]
        public void Recover(BattleStatus modify, FighterData enemy)
        {
            modify = CheckChain<BattleStatus>(Timing.OnRecover, new object[] {modify, this, enemy});
            this.Status += modify;
            Status.CurHp = math.min(Status.MaxHp, Status.CurHp);
            Updated();
        }

        public void Heal(BattleStatus modify)
        {
            modify = CheckChain<BattleStatus>(Timing.OnHeal, new object[] {modify, this});
            this.Status += modify;
            Status.CurHp = math.min(Status.MaxHp, Status.CurHp);
            Updated();
        }
        
        
        [Button]
        public void TakeBuff(BuffData buff)
        {
            Buffs.Add(buff);
        }
        
        
        
        [JsonIgnore] public int LossHp => Status.MaxHp - Status.CurHp;
        
        
        /// <summary>
        /// 方法参数查看触发时机注释，必须匹配
        /// </summary>
        /// <param name="timing">触发时机</param>
        /// <param name="param">方法参数</param>
        /// <typeparam name="T">返回值类型</typeparam>
        /// <returns></returns>
        protected T CheckChain<T>(Timing timing, object[] param)
        {
            var tmp = new List<IEffectContainer>();
            
            foreach (var skill in Skills)
            {
                if (!skill.IsEmpty &&(skill.MayAffect(timing, out _)))
                {
                    tmp.Add(skill);
                }
            }

            foreach (var buff in Buffs)
            {
                if (buff.MayAffect(timing, out _))
                {
                    tmp.Add(buff);
                }
            }
            
            
            
            tmp.Sort(
                (x, y) =>
                {
                    x.MayAffect(timing, out var xx);
                    y.MayAffect(timing, out var yy);
                    return xx - yy;
                });

            var origin = (T) param[0];
            foreach (var sk in tmp)
            {
                origin = sk.Affect<T>(timing, param);
            }
            return origin;
        }

        public void ApplyBuff(BuffData buff)
        {
            if (buff.Positive)
            {
                buff = CheckChain<BuffData>(Timing.OnBuff, new object[] {buff, this });
            }
            else
            {
                buff = CheckChain<BuffData>(Timing.OnDeBuff, new object[] {buff, this });
            }
            Buffs.Add(buff);
        }
        
        
        [Button]
        public void CastNonAimingSkill(int index)
        {
            if ((Skills[index].Bp.Positive)&&(!Skills[index].Bp.NeedTarget))
            {
                Skills[index].Bp.Fs[Timing.SkillEffect].Invoke(Skills[index], new object[]{this});
                Skills[index].SetCooldown();
                Updated();
            }
            else
            {
                throw new Exception();
            }
        }


        public void CastAimingSkill(int index, FighterData enemy)
        {
            
        }
        
        


        public void CoolDown(int x = 1)
        {
            for (int i = 0; i < Skills.Count; i++)
            {
                if (Skills[i].IsEmpty)
                {
                    break;
                }
                
                if ((Skills[i].Bp.Positive)&&(Skills[i].Cooldown > 0))
                {
                    Skills[i].Cooldown -= x;
                    if (Skills[i].Cooldown < 0)
                    {
                        Skills[i].Cooldown = 0;
                    }
                }
            }
        }



        public void Attack(FighterData target, Attack attack)
        {
            var result = target.Defend(attack, this);
            var r = Settle(result, target);

            if (r.Death)
            {
                Kill(r, target);
            }

        }
        
        
        
        
        ~FighterData()
        {
            /*if (this is EnemySaveData d)
            {
                Debug.Log($"~ {d.Id}");
            }
            if (this is PlayerData d2)
            {
                Debug.Log($"~ player");
            }*/
        }
        
    }

    public enum HealType
    {
        Heal,
        Rest,
        Blood
    }
}
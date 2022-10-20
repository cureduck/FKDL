﻿using System;
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
        [ShowInInspector] public SkillData[] Skills;
        [ShowInInspector] public BuffAgent Buffs;

        private Attack InitAttack()
        {
            return new Attack
            {
                PAtk = Status.PAtk,
                MAtk = 0,
                CAtk = 0,
                Id = null
            };
        }

        public bool IsPlayer => this == GameManager.Instance.PlayerData;

        public Result Suffer(Attack attack, FighterData enemy)
        {
            attack = CheckChain<Attack>(Timing.OnDefend, new object[] {attack, this, enemy});
            
            var result = new Result
            {
                PAtk = math.max(0, attack.PAtk - Status.PDef),
                MAtk = math.max(0, attack.MAtk - Status.MDef),
                CAtk = attack.CAtk
            };

            Status.CurHp -= result.PAtk + result.MAtk + attack.CAtk;
            
            Updated();
            
            if ((Status.CurHp <= 0)&&(this is EnemySaveData))
            {
                Updated();
            }
            
            return result;
        }


        public Attack ForgeAttack(FighterData target)
        {
            var atk = InitAttack();

            atk = CheckChain<Attack>(Timing.OnAttack, new object[] {atk, this, target});

            return atk;
        }
        
        
        /// <summary>
        /// 进攻时的触发条件
        /// </summary>
        /// <param name="r"></param>
        /// <param name="enemy"></param>
        /// <returns></returns>
        public Result Settle(Result r, FighterData enemy)
        {
            foreach (var skill in Skills)
            {
                if ((!skill.IsEmpty)&&(skill.Bp.Fs.ContainsKey(Timing.OnSettle)))
                {
                    var f = skill.Bp.Fs[Timing.OnSettle];
                    r = (Result) f?.Invoke(skill, new object[]{r, this, enemy});
                }
            }
            
            CoolDown();
            Buffs.Check();
            return r;
        }


        public Result Kill(Result r, FighterData enemy)
        {
            foreach (var skill in Skills)
            {
                if ((!skill.IsEmpty)&&(skill.Bp.Fs.ContainsKey(Timing.OnKill)))
                {
                    var f = skill.Bp.Fs[Timing.OnKill];
                    r = (Result) f?.Invoke(skill, new object[]{r, this, enemy});
                }
            }
            
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
            Updated();
        }

        public void Heal(BattleStatus modify)
        {
            modify = CheckChain<BattleStatus>(Timing.OnHeal, new object[] {modify, this});
            this.Status += modify;
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
        public void Cast(int index)
        {
            if (Skills[index].Bp.Positive)
            {
                Skills[index].Bp.Fs[Timing.OnCast].Invoke(Skills[index], new object[]{this});
            }

            Skills[index].Local = Skills[index].Bp.Cooldown;
            Updated();
        }


        public void CoolDown()
        {
            for (int i = 0; i < Skills.Length; i++)
            {
                if (Skills[i].IsEmpty)
                {
                    break;
                }
                
                if ((Skills[i].Bp.Positive)&&(Skills[i].Local > 0))
                {
                    Skills[i].Local -= 1;
                }
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
using System;
using System.Collections.Generic;
using System.Reflection;
using Managers;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;
using Object = System.Object;

namespace Game
{
    public class FighterData : MapData
    {
        public BattleStatus Status;
        [HideInInspector] public SkillData[] Skills;
        [ShowInInspector] public List<BuffData> Buffs;

        private Attack InitAttack()
        {
            return new Attack
            {
                PAtk = Status.PAtk,
                MAtk = Status.MAtk,
                CAtk = 0,
                Id = null
            };
        }


        public Result Suffer(Attack attack, FighterData enemy)
        {
            
            foreach (var skill in Skills)
            {
                if ((!skill.IsEmpty)&&(skill.Bp.Fs.ContainsKey(Timing.OnDefend)))
                {
                    var f = skill.Bp.Fs[Timing.OnDefend];
                    attack = (Attack) f?.Invoke(skill, new object[]{attack, this, enemy});
                }
            }
            
            Status.CurHp -= attack.PAtk - Status.PDef + attack.MAtk - Status.MDef + attack.CAtk;
            
            Updated();
            
            if ((Status.CurHp <= 0)&&(this is EnemySaveData))
            {
                Updated();
            }
            
            return new Result
            {
                PAtk = attack.PAtk - Status.PDef,
                MAtk = attack.MAtk - Status.MDef,
                CAtk = attack.CAtk
            };
        }


        public Attack ForgeAttack(FighterData target)
        {
            var atk = InitAttack();

            atk = CheckChain<Attack>(Timing.OnAttack, new object[] {atk, this, target});

            return atk;
        }
        
        
        
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

            return r;
        }
        

        protected void OnEquip(SkillData sk)
        {
            foreach (var pi in typeof(SkillData).GetMethods())
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
            }
            
            Updated();
        }
        
        
        protected void OnLoad(SkillData sk)
        {
            foreach (var pi in typeof(SkillData).GetMethods())
            {
                var msg = pi.GetCustomAttribute<EffectAttribute>();
                if ((msg == null)||(msg.id != sk.Id)) continue;
            }
            Updated();
        }
        
        public void OnUnEquip(SkillData skill)
        {
        }
        
        
        [Button]
        public void Strengthen(BattleStatus modify)
        {
            modify = CheckChain<BattleStatus>(Timing.OnStrengthen, new object[] {modify, this});
            this.Status += modify;
            Updated();
        }
        

        [Button]
        public void OnRecover(BattleStatus modify, HealType healType = HealType.Heal)
        {
            modify = CheckChain<BattleStatus>(Timing.OnHeal, new object[] {modify, healType, this});
            this.Status += modify;
            Updated();
        }
        
        
        /// <summary>
        /// 方法参数查看触发时机注释，必须匹配
        /// </summary>
        /// <param name="timing">触发时机</param>
        /// <param name="param">方法参数</param>
        /// <typeparam name="T">返回值类型</typeparam>
        /// <returns></returns>
        protected T CheckChain<T>(Timing timing, object[] param) where T : struct
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


        public void Apply(BuffData buff)
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
        
        
        
    }

    public enum HealType
    {
        Heal,
        Rest,
        Blood
    }
}
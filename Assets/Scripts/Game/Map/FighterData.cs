﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Managers;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Object = System.Object;
using Random = UnityEngine.Random;

namespace Game
{
    public abstract class FighterData : MapData
    {
        public BattleStatus Status;

        [JsonIgnore]
        public int Gold
        {
            get => Status.Gold;
            protected set => Status.Gold = value;
        }
        [ShowInInspector] public SkillAgent Skills;
        [ShowInInspector] public BuffAgent Buffs;

        public abstract FighterData Enemy { get; }

        [JsonIgnore] public int CurHp => Status.CurHp;
        
        public int Shield;
        
        private Attack InitAttack(SkillData skill = null, CostInfo costInfo = default)
        {
            return new Attack(Status.PAtk, costInfo : costInfo); //{Skill = skill};
        }

        [JsonIgnore] public bool IsPlayer => this == GameManager.Instance.PlayerData;
        [JsonIgnore] public bool IsAlive => Status.CurHp > 0;

        public Attack Defend(Attack attack, FighterData enemy)
        {
            attack.PDmg = math.max(0, (int)(attack.PAtk * attack.Multi) - Status.PDef);
            attack.MDmg = math.max(0, (int)(attack.MAtk * attack.Multi) - Status.MDef);
            attack.CDmg = (int) (attack.CAtk * attack.Multi);
            
            attack = CheckChain<Attack>(Timing.OnDefend, new object[] {attack, this, enemy});
            Status.CurHp -= attack.SumDmg;

            Status.CurHp = math.max(0, Status.CurHp);
            
            DelayUpdate();
            
            if ((Status.CurHp <= 0)&&(this is EnemySaveData))
            {
                DelayUpdate();
            }
            
            return attack;
        }


        public CostInfo GetSkillCost(SkillData skill)
        {
            if (skill == null) return CostInfo.Zero;
            var cost = skill.Bp.CostInfo;
            cost = CheckChain<CostInfo>(Timing.OnGetSkillCost, new object[]{cost, this});
            return cost;
        }
        

        public Attack Suffer(Attack attack)
        {
            return attack;
        }
        


        public Attack ForgeAttack(FighterData target, SkillData skillData = null)
        {
            var cost = GetSkillCost(skillData);
            
            if (skillData == null)
            {
                var atk = InitAttack();
                atk = CheckChain<Attack>(Timing.OnAttack, new object[] {atk, this, target});
                return atk;
            }
            else
            {
                skillData.Sealed = false;
                var atk = InitAttack(skillData, cost);
                atk = CheckChain<Attack>(Timing.OnAttack, new object[] {atk, this, target});
                skillData.Sealed = true;
                //skillData.SetCooldown();
                return atk;
            }
        }



        public Attack OperateAttack(FighterData target, Attack attack)
        {
            //当前连击段数
            int i = 0;
            var tmp = attack;
            while (attack.Combo > 0)
            {
                tmp = CheckChain<Attack>(Timing.OnStrike, new object[] {tmp, this, target, i});
                tmp = target.Defend(tmp, this);
                Settle(tmp, Enemy);
                attack.Include(tmp);
                
                if (!Enemy.IsAlive)
                {
                    Kill(attack, Enemy);
                    break;
                }

                i += 1;
            }

            return tmp;
            //AudioPlayer.Instance.PlaySoundEffect();
        }


        public void Purify(BuffData buff)
        {
            buff = CheckChain<BuffData>(Timing.OnPurify, new object[] {buff, this});
            Buffs.Remove(buff);
        }
        
        

        public CostInfo GetActualCostInfo(CostInfo costInfo, string kw = "")
        {
            return CheckChain<CostInfo>(Timing.OnCost, new object[] {costInfo, this, kw});
        }
        
        
        /// <summary>
        /// 检查条件施放资源是否足够
        /// </summary>
        /// <param name="originalCost"></param>
        /// <param name="info"></param>
        /// <param name="kw"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public bool CanAfford(CostInfo originalCost, out Info info, string kw = "")
        {
            var actualCost = GetActualCostInfo(originalCost);
            switch (actualCost.CostType)
            {
                case CostType.Hp:
                    if (Status.CurHp > actualCost.ActualValue)
                    {
                        info = new SuccessInfo();
                        return true;
                    }
                    else
                    {
                        info = new FailureInfo(FailureReason.NotEnoughHp, IsPlayer);
                        return false;
                    }
                case CostType.Mp:
                    if (Status.CurMp >= actualCost.ActualValue)
                    {
                        info = new SuccessInfo();
                        return true;
                    }
                    else
                    {
                        info = new FailureInfo(FailureReason.NotEnoughMp, IsPlayer);
                        return false;
                    }
                case CostType.Gold:
                    if (Status.Gold > actualCost.ActualValue)
                    {
                        info = new SuccessInfo();
                        return true;
                    }
                    else
                    {
                        info = new FailureInfo(FailureReason.NotEnoughGold, IsPlayer);
                        return false;
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// 检查是否cd足够，资源是否足够, 是否满足BattleOnly条件
        /// </summary>
        /// <param name="skill"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool CanCast(SkillData skill, out Info info)
        {
            return skill.CanCast(out info) && CanAfford(GetSkillCost(skill), out info);
        }
        
        /// <summary>
        /// 跳过条件检查直接使用
        /// </summary>
        /// <param name="skill"></param>
        public abstract void UseSkill(SkillData skill);
        
        
        /// <summary>
        /// 经过检查，再使用
        /// </summary>
        /// <param name="skill"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public abstract bool TryUseSkill(SkillData skill, out Info info);
        

        public void SwapSkill(int index01,int index02) 
        {
            if (index01 >= 0 && index01 < Skills.Count && index02 >= 0 && index02 < Skills.Count) 
            {
                var temp = Skills[index01];
                Skills[index01] = Skills[index02];
                Skills[index02] = temp;
                DelayUpdate();
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

            var rr = CheckChain<Attack>(Timing.OnDefend, new object[] {r, this, enemy});
            
            Cost(rr.CostInfo);
            
            //CoolDown();
            Buffs.RemoveZeroStackBuff();
            return r;
        }


        public Attack Kill(Attack r, FighterData enemy)
        {
            r = CheckChain<Attack>(Timing.OnKill, new object[] {r, this, enemy});
            //Debug.Log("killed");
            
            return r;
        }


        /// <summary>
        /// 只有获得金币走这个，消耗金币还是走Cost
        /// </summary>
        /// <param name="gold"></param>
        /// <param name="kw"></param>
        public void Gain(int gold, string kw = null)
        {
            var g = CheckChain<int>(Timing.OnGain, new object[] {gold, this, kw});
            
            Gold += g;
            DelayUpdate();
        }


        public void Cost(CostInfo modify, string kw = null)
        {
            modify = CheckChain<CostInfo>(Timing.OnCost, new object[] {modify, this, kw});
            Status -= modify;
            Status.CurHp = math.min(Status.MaxHp, Status.CurHp);
            Status.CurMp = math.min(Status.MaxMp, Status.CurMp);
            DelayUpdate();
        }


        public void CounterCharge(BattleStatus modify, string kw = null)
        {
            modify = CheckChain<BattleStatus>(Timing.OnCounterCharge, new object[] {modify, this, kw});
            Status += modify;
            Status.CurHp = math.min(Status.MaxHp, Status.CurHp);
            Status.CurMp = math.min(Status.MaxMp, Status.CurMp);
            DelayUpdate();
        }
        
        
        protected void OnGet(IEffectContainer c)
        {
            if (c.MayAffect(Timing.OnGet, out _))
            {
                c.Affect(Timing.OnGet, new object[]{this});
            }
        }
        
        
        /*protected void Equip(SkillData sk)
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
            }#1#

            if (sk.Bp.Fs.TryGetValue(Timing.OnSkillEquip, out var f))
            {
                f = sk.Bp.Fs[Timing.OnSkillEquip];
                f.Invoke(sk, new object[] {this});
                DelayUpdate();
            }

            if ((sk.Bp.Positive))
            {
                sk.Sealed = true;
            }
        }
        
        public void OnUnEquip(SkillData sk)
        {
            var f = sk.Bp.Fs[Timing.OnSkillUnEquip];
            f.Invoke(this, new object[] {sk, this});
            DelayUpdate();
        }
        */
        
        
        protected void Load(SkillData sk)
        {
            foreach (var pi in typeof(SkillData).GetMethods())
            {
                var msg = pi.GetCustomAttribute<EffectAttribute>();
                if ((msg == null)||(msg.id != sk.Id)) continue;
            }
            DelayUpdate();
        }
        
        
        
        [Button]
        public void Strengthen(BattleStatus modify)
        {
            modify = CheckChain<BattleStatus>(Timing.OnStrengthen, new object[] {modify, this});
            Status += modify;
            DelayUpdate();
        }


        public void RandomStrengthen(int v = 1)
        {
            var r = Random.Range(0, 6);
            switch (r)
            {
                case 0:
                    Strengthen(new BattleStatus{MaxHp = 5 * v});
                    break;
                case 1:
                    Strengthen(new BattleStatus{MaxMp = 5 * v});
                    break;
                case 2:
                    Strengthen(new BattleStatus{PAtk = v});
                    break;
                case 3:
                    Strengthen(new BattleStatus{PDef = v});
                    break;
                case 4:
                    Strengthen(new BattleStatus{MAtk = v});
                    break;
                case 5:
                    Strengthen(new BattleStatus{MDef = v});
                    break;
            }
        }
        

        [Button]
        public void Recover(BattleStatus modify, FighterData enemy)
        {
            modify = CheckChain<BattleStatus>(Timing.OnRecover, new object[] {modify, this, enemy});
            this.Status += modify;
            Status.CurHp = math.min(Status.MaxHp, Status.CurHp);
            DelayUpdate();
        }

        public void Heal(BattleStatus modify)
        {
            modify = CheckChain<BattleStatus>(Timing.OnHeal, new object[] {modify, this});
            this.Status += modify;
            Status.CurHp = math.min(Status.MaxHp, Status.CurHp);
            Status.CurMp = math.min(Status.MaxMp, Status.CurMp);

            DelayUpdate();
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
            var tmp = Skills.
                Where(skill => (skill != null) && !skill.IsEmpty && (skill.MayAffect(timing, out _)))
                .Cast<IEffectContainer>().ToList();

            tmp.AddRange(Buffs.Where(buff => (buff != null) && (buff.MayAffect(timing, out _))));


            if (this is PlayerData player)
            {
                tmp.AddRange(player.Relics.Where(relic => relic != null && relic.MayAffect(timing, out _)));
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
        
        
        protected void CheckChain(Timing timing, object[] param)
        {
            var tmp = new List<IEffectContainer>();
            
            foreach (var skill in Skills)
            {
                if ((skill != null)&& !skill.IsEmpty &&(skill.MayAffect(timing, out _)))
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
            
            if (this is PlayerData player)
            {
                foreach (var relic in player.Relics)
                {
                    if (relic.MayAffect(timing, out _))
                    {
                        tmp.Add(relic);
                    }
                }
            }
            
            
            tmp.Sort(
                (x, y) =>
                {
                    x.MayAffect(timing, out var xx);
                    y.MayAffect(timing, out var yy);
                    return xx - yy;
                });
            
            foreach (var sk in tmp)
            {
                sk.Affect(timing, param);
            }
        }
        
        
        /// <summary>
        /// 用于给敌人施加buff，获取最终生成的buff
        /// </summary>
        /// <param name="buff"></param>
        /// <returns></returns>
        public BuffData ApplyBuff(BuffData buff)
        {
            buff = CheckChain<BuffData>(Timing.OnApply, new object[] {buff, this });
            DelayUpdate();
            return buff;
        }
        
        
        
        /// <summary>
        /// 用于被添加buff时，获取最终生成的buff
        /// </summary>
        /// <param name="buff"></param>
        public void AppliedBuff(BuffData buff)
        {
            buff = CheckChain<BuffData>(Timing.OnApplied, new object[] {buff, this });
            Buffs.Add(buff);
            DelayUpdate();
        }

        /// <summary>
        /// 用于给自己添加buff
        /// </summary>
        /// <param name="buff"></param>
        [Button]
        public void ApplySelfBuff(BuffData buff)
        {
            buff = CheckChain<BuffData>(Timing.OnApply, new object[] {buff, this });
            buff = CheckChain<BuffData>(Timing.OnApplied, new object[] {buff, this });

            Buffs.Add(buff);
            DelayUpdate();

        }
        
        
        [Button]
        public void CastNonAimingSkill(int index)
        {
            if (Skills[index] != null && Skills[index] != SkillData.Empty)
            {
                CastNonAimingSkill(Skills[index]);
            }
        }

        public void CastNonAimingSkill(SkillData skill)
        {
            if ((skill.Bp.Positive)&&(skill.Bp.Fs.ContainsKey(Timing.SkillEffect)))
            {
                
                skill.Bp.Fs[Timing.SkillEffect].Invoke(skill, new object[]{this});
                
                AfterSkillUsed(skill);
                DelayUpdate();
            }
            else
            {
                throw new Exception();
            }

        }
        


        public void CastAimingSkill(int index, FighterData enemy)
        {
            
        }


        private void SingleStrike(Attack attack, FighterData enemy)
        {
            
        }
        
        
        
        
        
        /// <summary>
        /// 参数为使用的技能
        /// </summary>
        /// <param name="skill"></param>
        /// <returns></returns>
        public Attack? ManageAttackRound(SkillData skill = null)
        {

            CheckChain(Timing.OnPreAttack, new object[] {this, Enemy});

            if (!IsAlive)
            {
                return null;
            }
            
            
            if ((skill!=null)&&(!skill.Bp.BattleOnly))
            {
                CastNonAimingSkill(skill);
                
                AfterSkillUsed(skill);
                //skill.SetCooldown(skill.Bp.Cooldown);
                //skill = (CheckChain<SkillData>(Timing.OnSetCoolDown, new object[] {skill, this}));
                
                DelayUpdate();
                return null;
            }
            else
            {
                var pa = ForgeAttack(Enemy, skill);

                if (skill != null) skill.Sealed = false;
                
                pa = OperateAttack(Enemy, pa);
                
                //Settle(pa, Enemy);
                if (skill != null) skill.Sealed = true;
                
                AfterSkillUsed(skill);
                DelayUpdate();
                return pa;
            }
        }


        private void AfterSkillUsed([CanBeNull] SkillData skill)
        {
            if (GameManager.Instance.InBattle)
            {
                CoolDown();
            }
            
            if (skill == null)
            {
                return;
            }
            
            skill.SetCooldown(skill.Bp.Cooldown);
            skill = (CheckChain<SkillData>(Timing.OnSetCoolDown, new object[] {skill, this}));
        }
        

        public void CoolDown(int x = 1)
        {
            for (int i = 0; i < Skills.Count; i++)
            {
                if (Skills[i] == null) continue;

                if ((Skills[i].IsEmpty)||(Skills[i].Bp == null))
                {
                    continue;
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

        public void Upgrade(SkillData skillData, int lv = 1)
        {
            skillData.CurLv += lv;
            CheckChain<SkillData>(Timing.OnLvUp, new object[] {skillData, this});
        }
        

        /*public void Attack(FighterData target, Attack attack)
        {
            var result = target.Defend(attack, this);
            var r = Settle(result, target);

            if (r.Death)
            {
                Kill(r, target);
            }
        }
        */
        
        
        
        
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
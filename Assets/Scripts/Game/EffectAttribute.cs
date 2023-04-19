using System;

namespace Game
{
    public class EffectAttribute : Attribute
    {
        public EffectAttribute(string id, Timing timing, int priority = 1, bool alwaysActive = false)
        {
            this.id = id;
            this.timing = timing;
            this.priority = priority;
            this.alwaysActive = alwaysActive;
        }

        public string id;
        public Timing timing;
        public int priority;
        public bool alwaysActive;
    }
    
    
    public enum Timing
    {
        /// <summary>
        /// info, skillData, fighter
        /// </summary>
        OnHandleSkillInfo,
        /// <summary>
        /// costInfo, skillData, fighter, Try
        /// </summary>
        OnGetSkillCost,
        /// <summary>
        /// attack, fighter, enemy
        /// </summary>
        BeforeAttack,
        /// <summary>
        /// attack, fighter, enemy
        /// </summary>
        OnAttack,
        /// <summary>
        /// attack, fighter, enemy, time
        /// </summary>
        OnStrike,
        /// <summary>
        /// attack, fighter, enemy 注意enemy可能为空
        /// </summary>
        OnDefend,
        /// <summary>
        /// attack, attacker, enemy
        /// </summary>
        OnAttackSettle,
        /// <summary>
        /// attack, defender, enemy 注意enemy可能为空
        /// </summary>
        OnDefendSettle,
        /// <summary>
        /// attack, fighter, enemy
        /// </summary>
        OnKill,
        /*/// <summary>
        /// attack, fighter, enemy
        /// </summary>
        OnDrawBack,*/
        /// <summary>
        /// Heal Data, fighter, enemy, kw
        /// </summary>
        OnRecover,
        /// <summary>
        /// skill data, fighter
        /// </summary>
        OnLvUp,
        /// <summary>
        /// modifier, fighter, kw
        /// </summary>
        OnHeal,
        /// <summary>
        /// modifier, fighter
        /// </summary>
        OnStrengthen,
        /// <summary>
        /// fighter
        /// </summary>
        PotionEffect,
        /// <summary>
        /// fighter
        /// </summary>
        SkillEffect,
        /// <summary>
        /// potion, fighter
        /// </summary>
        OnUsePotion,
        /// <summary>
        /// key, fighter
        /// </summary>
        OnGetKey,
        /// <summary>
        /// key, fighter
        /// </summary>
        OnUseKey,
        /// <summary>
        /// fighter
        /// </summary>
        OnMarch,
        /// <summary>
        /// coin, fighter, kw
        /// </summary>
        OnGain,
        /// <summary>
        /// 被施加buff buff, fighter
        /// </summary>
        OnApplied,
        /// <summary>
        /// 施加buff buff, fighter
        /// </summary>
        OnApply,
        /// <summary>
        /// 净化时 buffData, fighter
        /// </summary>
        OnPurify,
        
        /// <summary>
        /// 冷却时 SkillData, fighter
        /// </summary>
        OnSetCoolDown,
        
        /*/// <summary>
        /// buff, fighter
        /// </summary>
        OnBuffLvChange,*/
        
        /// <summary>
        /// battle status, fighter, kw
        /// </summary>
        OnCounterCharge,
        /// <summary>
        /// Cost Info, fighter, kw
        /// </summary>
        OnCost,

        /// <summary>
        /// fighter 获取
        /// </summary>
        OnGet,
        
        /// <summary>
        /// fighter
        /// </summary>
        OnUnGet
    }
}
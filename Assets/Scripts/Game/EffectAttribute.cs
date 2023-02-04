using System;

namespace Game
{
    public class EffectAttribute : Attribute
    {
        public EffectAttribute(string id, Timing timing, int priority = 1, bool activated = true)
        {
            this.id = id;
            this.timing = timing;
            this.priority = priority;
            this.activated = activated;
        }

        public string id;
        public Timing timing;
        public int priority;
        public bool activated;
    }
    
    
    public enum Timing
    {
        /// <summary>
        /// fighter, enemy
        /// </summary>
        OnEngage,
        /// <summary>
        /// attack, fighter, enemy
        /// </summary>
        OnPreAttack,
        /// <summary>
        /// attack, fighter, enemy
        /// </summary>
        OnAttack,
        /// <summary>
        /// attack, fighter, enemy, time
        /// </summary>
        OnStrike,
        /// <summary>
        /// attack, fighter, enemy
        /// </summary>
        OnDefend,
        /// <summary>
        /// attack, fighter, enemy
        /// </summary>
        OnSettle,
        /// <summary>
        /// attack, fighter, enemy
        /// </summary>
        OnKill,
        /// <summary>
        /// attack, fighter, enemy
        /// </summary>
        OnDrawBack,
        /// <summary>
        /// Heal Data, fighter, enemy
        /// </summary>
        OnRecover,
        /// <summary>
        /// skill data, fighter
        /// </summary>
        OnEquip,
        /// <summary>
        /// skill data, fighter
        /// </summary>
        OnUnEquip,
        /// <summary>
        /// skill data, fighter
        /// </summary>
        OnLvUp,
        /// <summary>
        /// modifier, fighter
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
        /// coin, fighter
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
        /// 净化时 fighter
        /// </summary>
        OnPurify,
        /// <summary>
        /// buff, fighter
        /// </summary>
        OnBuffLvChange,
        /// <summary>
        /// battle status, fighter, kw
        /// </summary>
        OnCounterCharge,
        /// <summary>
        /// battle status, fighter, kw
        /// </summary>
        OnCost,
        /// <summary>
        /// battle status, fighter, kw
        /// </summary>
        OnLoss
    }
}
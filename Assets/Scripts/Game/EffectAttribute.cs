using System;

namespace Game
{
    public class EffectAttribute : Attribute
    {
        public EffectAttribute(string id, Timing timing, int priority = 1)
        {
            this.id = id;
            this.timing = timing;
            this.priority = priority;
        }

        public string id;
        public Timing timing;
        public int priority;
    }
    
    
    public enum Timing
    {
        /// <summary>
        /// attack, fighter, enemy
        /// </summary>
        OnAttack,
        /// <summary>
        /// attack, fighter, enemy
        /// </summary>
        OnDefend,
        /// <summary>
        /// result, fighter, enemy
        /// </summary>
        OnSettle,
        /// <summary>
        /// result, fighter, enemy
        /// </summary>
        OnKill,
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
        OnCast,
        /// <summary>
        /// fighter
        /// </summary>
        OnMarch,
        /// <summary>
        /// coin, fighter
        /// </summary>
        OnGain,
        /// <summary>
        /// 获得正面buff buff, fighter
        /// </summary>
        OnBuff,
        /// <summary>
        /// 获得负面buff buff, fighter
        /// </summary>
        OnDeBuff,
        /// <summary>
        /// 净化时 fighter
        /// </summary>
        OnPurify,
        /// <summary>
        /// buff, fighter
        /// </summary>
        OnBuffLvChange
    }
}
using System;

namespace Game
{
    public class EffectAttribute : Attribute
    {
        public EffectAttribute(string id, Timing timing, int priority = 1)
        {
            this.id = id;
            this.timing = timing;
        }

        public string id;
        public Timing timing;
        public int priority;
    }
    
    
    public enum Timing
    {
        /// <summary>
        /// attack, attacker, defender
        /// </summary>
        OnAttack,
        /// <summary>
        /// attack, defender, attacker
        /// </summary>
        OnDefend,
        /// <summary>
        /// 
        /// </summary>
        OnSettle,
        /// <summary>
        /// fighter
        /// </summary>
        OnEquip,
        /// <summary>
        /// fighter
        /// </summary>
        OnUnEquip,
        /// <summary>
        /// fighter
        /// </summary>
        OnLvUp,
        /// <summary>
        /// attack, attacker, defender
        /// </summary>
        OnKill,
        /// <summary>
        /// modifier, fighter
        /// </summary>
        OnHeal,
        /// <summary>
        /// modifier, fighter
        /// </summary>
        OnStrengthen,
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
        /// skill, fighter
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
        /// 
        /// </summary>
        OnUse,
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
using System;

namespace Game
{
    public class SkillEffectAttribute : Attribute
    {
        public SkillEffectAttribute(string id, Timing timing, int priority = 1)
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
        OnAttack,
        OnDefend,
        OnSettle,
        OnEquip,
        OnUnEquip,
        OnLvUp,
        OnKill,
        OnHeal,
        OnStrengthen,
        OnUsePotion,
        OnGainSkill,
        OnGetKey,
        OnUseKey,
        OnCast,
        OnMarch,
        OnGain
    }
}
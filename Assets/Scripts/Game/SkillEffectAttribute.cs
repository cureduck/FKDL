using System;

namespace Game
{
    public class SkillEffectAttribute : Attribute
    {
        public SkillEffectAttribute(string id, Timing timing)
        {
            this.id = id;
            this.timing = timing;
        }

        public string id;
        public Timing timing;
    }
    
    
    public enum Timing
    {
        Attack,
        Settle,
        Equip,
        UnEquip,
        LvUp,
        Kill,
        Heal
    }
}
using System;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace Game
{
    public struct Attack
    {
        public int PAtk;
        public int MAtk;
        public int CAtk;
        public string Id;

        public int PDmg;
        public int MDmg;
        public int CDmg;

        public float Multi;
        public int Combo;
        
        public bool Death;

        public SkillData Skill;

        public Attack(int pAtk = 0, int mAtk = 0, int cAtk =0, float multi = 1f, int combo = 2)
        {
            Combo = combo;
            Multi = multi;
            PAtk = pAtk;
            MAtk = mAtk;
            CAtk = cAtk;
            Id = null;
            PDmg = 0;
            MDmg = 0;
            CDmg = 0;
            Skill = null;

            Death = false;
        }


        [JsonIgnore] public int Sum => PDmg + MDmg + CDmg;

        [JsonIgnore]
        public Attack SubAttack
        {
            get
            {
                var tmp = this;
                tmp.PDmg = 0;
                tmp.MDmg = 0;
                tmp.CDmg = 0;
                return tmp;
            }
        }


        public void Include(Attack subAttack)
        {
            this.PDmg += subAttack.PDmg;
            this.MDmg += subAttack.MDmg;
            this.CDmg += subAttack.CDmg;
            this.Combo -= 1;
        }
        
        

        public override string ToString()
        {
            return $"({PAtk}, {MAtk}, {CAtk}), ({PDmg}, {MDmg}, {CDmg})";
        }
    }
}
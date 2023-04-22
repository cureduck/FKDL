﻿using System;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace Game
{
    public struct Attack
    {
        public int PAtk;
        public int MAtk;
        public int CAtk;
        public string Kw;

        public int PDmg;
        public int MDmg;
        public int CDmg;

        public float Multi;
        public int Combo;
        public CostInfo CostInfo;
        
        public bool Death;
        
        /*public Attack(int pAtk = 0, int mAtk = 0, int cAtk =0, float multi = 1f, int combo = 1,string id = "", CostInfo costInfo = default)
        {
            Combo = combo;
            Multi = multi;
            PAtk = pAtk;
            MAtk = mAtk;
            CAtk = cAtk;
            Id = id;
            PDmg = 0;
            MDmg = 0;
            CDmg = 0;
            //Skill = null;
            CostInfo = costInfo;

            Death = false;
        }*/
        
        
        public Attack(int pAtk = 0, int mAtk = 0, int cAtk =0, float multi = 1f, int combo = 1, string kw ="", CostInfo costInfo = default)
        {
            Combo = combo;
            Multi = multi;
            PAtk = pAtk;
            MAtk = mAtk;
            CAtk = cAtk;
            Kw = kw;
            PDmg = 0;
            MDmg = 0;
            CDmg = 0;
            //Skill = null;
            CostInfo = costInfo;

            Death = false;
        }
        


        [JsonIgnore] public int SumDmg => PDmg + MDmg + CDmg;

        [JsonIgnore]
        private Attack SubAttack
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


        /// <summary>
        /// 伤害累计
        /// </summary>
        /// <param name="subAttack"></param>
        public void Include(Attack subAttack)
        {
            this.PDmg += subAttack.PDmg;
            this.MDmg += subAttack.MDmg;
            this.CDmg += subAttack.CDmg;
        }
        
        

        public override string ToString()
        {
            return $"({PAtk}|{MAtk}|{CAtk}) *{Multi} *{Combo} : ({PDmg}|{MDmg}|{CDmg})";
        }
    }
}
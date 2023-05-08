using System;
using Managers;
using Unity.Mathematics;

namespace Game
{
    public struct BattleStatus
    {
        public int MaxHp;
        public int CurHp;
        public int MaxMp;
        public int CurMp;
        public int MAtk;
        public int PAtk;
        public int MDef;
        public int PDef;

        public int Gold;


        public BattleStatus LvUp(int lv)
        {
            if (MaxHp > 0) MaxHp += lv;
            if (MaxMp > 0) MaxMp += lv;
            if (MAtk > 0) MAtk += lv;
            if (MDef > 0) MDef += lv;
            if (PAtk > 0) PAtk += lv;
            if (PDef > 0) PDef += lv;
            return this;
        }


        public static BattleStatus operator +(BattleStatus s1, BattleStatus s2)
        {
            return new BattleStatus
            {
                MaxHp = s1.MaxHp + s2.MaxHp,
                CurHp = s1.CurHp + s2.CurHp,
                MaxMp = s1.MaxMp + s2.MaxMp,
                CurMp = s1.CurMp + s2.CurMp,
                MAtk = s1.MAtk + s2.MAtk,
                PAtk = s1.PAtk + s2.PAtk,
                MDef = s1.MDef + s2.MDef,
                PDef = s1.PDef + s2.PDef,
                Gold = s1.Gold + s2.Gold
            };
        }

        public static BattleStatus operator +(BattleStatus s1, CostInfo s2)
        {
            switch (s2.CostType)
            {
                case CostType.Hp:
                    s1.CurHp += s2.ActualValue;
                    break;
                case CostType.Mp:
                    s1.CurMp += s2.ActualValue;
                    break;
                case CostType.Gold:
                    s1.Gold += s2.ActualValue;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return s1;
        }

        public static BattleStatus operator -(BattleStatus s1, CostInfo s2)
        {
            switch (s2.CostType)
            {
                case CostType.Hp:
                    s1.CurHp -= s2.ActualValue;
                    break;
                case CostType.Mp:
                    s1.CurMp -= s2.ActualValue;
                    break;
                case CostType.Gold:
                    s1.Gold -= s2.ActualValue;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return s1;
        }


        public static BattleStatus operator -(BattleStatus s1)
        {
            return new BattleStatus
            {
                MaxHp = -s1.MaxHp,
                CurHp = -s1.CurHp,
                MaxMp = -s1.MaxMp,
                CurMp = -s1.CurMp,
                MAtk = -s1.MAtk,
                PAtk = -s1.PAtk,
                MDef = -s1.MDef,
                PDef = -s1.PDef
            };
        }


        public static BattleStatus operator -(BattleStatus s1, BattleStatus s2)
        {
            return new BattleStatus
            {
                MaxHp = s1.MaxHp - s2.MaxHp,
                CurHp = s1.CurHp - s2.CurHp,
                MaxMp = s1.MaxMp - s2.MaxMp,
                CurMp = s1.CurMp - s2.CurMp,
                MAtk = s1.MAtk - s2.MAtk,
                PAtk = s1.PAtk - s2.PAtk,
                MDef = s1.MDef - s2.MDef,
                PDef = s1.PDef - s2.PDef
            };
        }


        public static BattleStatus operator *(BattleStatus s1, float v)
        {
            return new BattleStatus
            {
                MaxHp = (int)(s1.MaxHp * v),
                CurHp = (int)(s1.CurHp * v),
                MaxMp = (int)(s1.MaxMp * v),
                CurMp = (int)(s1.CurMp * v),
                MAtk = (int)(s1.MAtk * v),
                PAtk = (int)(s1.PAtk * v),
                MDef = (int)(s1.MDef * v),
                PDef = (int)(s1.PDef * v)
            };
        }


        public void Heal(int value)
        {
            CurHp = math.min(CurHp + value, MaxHp);
        }


        public void ReduceMaxHp(int value)
        {
            MaxHp -= value;
            CurHp = math.min(MaxHp, CurHp);
        }

        public static BattleStatus HP(int value)
        {
            return new BattleStatus
            {
                CurHp = value
            };
        }

        public static BattleStatus StrengthenAtk(int value)
        {
            return new BattleStatus
            {
                PAtk = value
            };
        }


        /// <summary>
        /// 自动将输入转为负值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static BattleStatus Mp(int value)
        {
            return new BattleStatus
            {
                CurMp = +value
            };
        }


        public override string ToString()
        {
            return $"({CurHp}/{MaxHp},{CurMp}/{MaxMp})";
        }
    }
}
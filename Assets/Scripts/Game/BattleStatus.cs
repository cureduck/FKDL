using System;
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


        public BattleStatus(int maxHp = 0, int curHp = 0, int maxMp = 0, int curMp = 0, int mAtk = 0, int pAtk = 0,
            int mDef = 0, int pDef = 0, int gold = 0)
        {
            MaxHp = maxHp;
            CurHp = curHp;
            MaxMp = maxMp;
            CurMp = curMp;
            MAtk = mAtk;
            PAtk = pAtk;
            MDef = mDef;
            PDef = pDef;
            Gold = gold;
        }


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
                MaxHp = (int)math.round(s1.MaxHp * v),
                CurHp = (int)math.round(s1.CurHp * v),
                MaxMp = (int)math.round(s1.MaxMp * v),
                CurMp = (int)math.round(s1.CurMp * v),
                MAtk = (int)math.round(s1.MAtk * v),
                PAtk = (int)math.round(s1.PAtk * v),
                MDef = (int)math.round(s1.MDef * v),
                PDef = (int)math.round(s1.PDef * v)
            };
        }

        /// <summary>
        /// 将BattleStatus隐式转换成CostInfo
        /// </summary>
        /// <param name="s"></param>
        public static implicit operator CostInfo(BattleStatus s)
        {
            if (s.CurMp < 0)
            {
                return new CostInfo(
                    -s.CurMp,
                    CostType.Mp
                );
            }

            if (s.CurHp < 0)
            {
                return new CostInfo(
                    -s.CurHp,
                    CostType.Hp
                );
            }

            if (s.Gold < 0)
            {
                return new CostInfo(
                    -s.Gold,
                    CostType.Gold
                );
            }

            return new CostInfo();
        }


        public static BattleStatus operator /(BattleStatus s1, float v)
        {
            return s1 * (1 / v);
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


        public static BattleStatus Mp(int value)
        {
            return new BattleStatus
            {
                CurMp = value
            };
        }

        private const int K = 3;

        public static BattleStatus GetProfessionUpgrade(string prof)
        {
            prof = prof.ToLower();
            switch (prof)
            {
                case "bar":
                    return new BattleStatus
                    {
                        PAtk = 1,
                        MAtk = -1
                    };
                case "mag":
                    return new BattleStatus
                    {
                        MAtk = 1,
                        MDef = 1,
                        PAtk = -1,
                        MaxMp = K
                    };
                case "ass":
                    return new BattleStatus
                    {
                        PAtk = 1,
                        MaxHp = -2 * K
                    };
                case "kni":
                    return new BattleStatus
                    {
                        MDef = 1,
                        PDef = 1
                    };
                case "alc":
                    return new BattleStatus
                    {
                        MaxHp = 4 * K
                    };
                case "bli":
                    return new BattleStatus
                    {
                        PAtk = 1,
                    };
                case "cur":
                    return new BattleStatus
                    {
                        PAtk = 1,
                        MAtk = 1,
                        MaxHp = -2 * K,
                        MaxMp = -2 * K
                    };
                case "com":
                    return new BattleStatus()
                    {
                        MaxHp = K,
                        MaxMp = K
                    };

                default:
                    return new BattleStatus();
            }
        }


        public string ToString(string format = "detail", IFormatProvider formatProvider = null)
        {
            if (format == "short")
            {
                return $"{math.max(0, CurHp)}/{MaxHp}";
            }
            else
            {
                return $"{CurHp}/{MaxHp},{CurMp}/{MaxMp}";
            }
        }
    }
}
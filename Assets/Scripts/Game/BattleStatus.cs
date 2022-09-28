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
                PDef = s1.PDef + s2.PDef
            };
        }
        
        
        public static BattleStatus operator *(BattleStatus s1, float v)
        {
            return new BattleStatus
            {
                MaxHp = (int)(s1.MaxHp*v),
                CurHp = (int)(s1.CurHp*v),
                MaxMp = (int)(s1.MaxMp*v),
                CurMp = (int)(s1.CurMp*v),
                MAtk = (int)(s1.MAtk*v),
                PAtk = (int)(s1.PAtk*v),
                MDef = (int)(s1.MDef*v),
                PDef = (int)(s1.PDef*v)
            };
        }
        
    }

    public struct PlayerStatus
    {
        public int MaxSp;
        public int CurSp;
    }
}
using System;

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

        public int Sum => PDmg + MDmg + CDmg;

        public Action<FighterData, Attack> OnComplete;
        public Action<FighterData, Attack> OnKill;
        
        public override string ToString()
        {
            return $" ({PAtk}, {MAtk}, {CAtk}), ({PDmg}, {MDmg}, {CDmg})";
        }
    }
}
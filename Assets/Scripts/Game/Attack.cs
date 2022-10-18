namespace Game
{
    public struct Attack
    {
        public int PAtk;
        public int MAtk;
        public int CAtk;
        public string Id;
    }

    public struct Result
    {
        public int PAtk;
        public int MAtk;
        public int CAtk;

        public int Sum => PAtk + MAtk + CAtk;
    }
}
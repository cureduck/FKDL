namespace Game
{
    public struct CostInfo
    {
        public int Value;
        public CostType CostType;
        
        public static readonly CostInfo Zero = new CostInfo
        {
            CostType = CostType.Hp,
            Value = 0
        };
    }
    public enum CostType
    {
        Hp,
        Mp,
        Gold
    }
}
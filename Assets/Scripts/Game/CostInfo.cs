using Newtonsoft.Json;

namespace Game
{
    public struct CostInfo
    {
        public int Value;
        public CostType CostType;
        public float Discount;

        [JsonIgnore] public int ActualValue => (int)(Value * Discount);
        
        /// <summary>
        /// 注意， value值
        /// </summary>
        /// <param name="value"></param>
        /// <param name="costType"></param>
        /// <param name="discount"></param>
        public CostInfo(int value = 0, CostType costType = CostType.Gold, float discount = 1f)
        {
            Value = value;
            CostType = costType;
            Discount = discount;
        }

        public static CostInfo MpCost(int value)
        {
            return new CostInfo(value, CostType.Mp);
        }
        
        public static CostInfo HpCost(int value)
        {
            return new CostInfo(value, CostType.Hp);
        }

        public static readonly CostInfo Zero = new CostInfo();
    }
    
    public enum CostType
    {
        Hp,
        Mp,
        Gold
    }
}
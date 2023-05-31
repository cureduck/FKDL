using Newtonsoft.Json;

namespace Game
{
    public struct CostInfo
    {
        private int _value;

        public int Value
        {
            get => _value;
            set => _value = value < 0 ? 0 : value;
        }

        public readonly CostType CostType;

        /// <summary>
        /// 折扣，0.8就是8折
        /// </summary>
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
            _value = value;
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

        public override string ToString()
        {
            return $"{CostType}: {Value}";
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
namespace Game
{
    public class SupplySaveData : MapData
    {
        public SupplyType Type;
        public Rank Rank;

        public SupplySaveData(SupplyType supplyType, Rank rank) : base()
        {
            Type = supplyType;
            Rank = rank;
        }
    }
    public enum SupplyType
    {
        Spring,
        Grassland,
        Camp
    }
}
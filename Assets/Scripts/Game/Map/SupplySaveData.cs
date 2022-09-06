namespace Game
{
    public class SupplySaveData : MapData
    {
        public SupplyType Type;
    }
    public enum SupplyType
    {
        Spring,
        Grassland,
        Camp
    }
}
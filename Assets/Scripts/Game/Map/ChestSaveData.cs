namespace Game
{
    public sealed class ChestSaveData : MapData
    {
        public ChestType Type;
        
        
        public enum ChestType
        {
            White,
            Red,
            Yellow,
            Class
        }
    }
}
namespace Game
{
    public class ShopSaveData : MapData
    {
        public override void OnReact()
        {
            base.OnReact();
            Destroyed();
        }
    }
}
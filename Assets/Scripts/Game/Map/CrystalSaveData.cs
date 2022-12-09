namespace Game
{
    public class CrystalSaveData : MapData
    {
        public override void OnReact()
        {
            base.OnReact();
            Destroyed();
        }
    }
}
namespace Game
{
    public class TotemSaveData : MapData
    {
        public override void OnReact()
        {
            base.OnReact();
            Destroyed();
        }
    }
}
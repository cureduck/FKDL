namespace Game
{
    public class StartSaveData : MapData
    {
        public override void OnReact()
        {
            base.OnReact();
            Destroyed();
        }
    }
}
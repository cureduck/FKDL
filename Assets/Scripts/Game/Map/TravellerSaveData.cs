namespace Game
{
    public class TravellerSaveData : MapData
    {
        public override void OnReact()
        {
            base.OnReact();
            Destroyed();
        }
    }
}
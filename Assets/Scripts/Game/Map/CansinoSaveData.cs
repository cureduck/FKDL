namespace Game
{
    public class CasinoSaveData : MapData
    {
        public const int MaxTimes = 10;
        
        public int TimesLeft;

        public override void Init()
        {
            base.Init();
            TimesLeft = MaxTimes;
        }
    }
}
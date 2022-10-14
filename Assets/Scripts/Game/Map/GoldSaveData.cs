using Managers;

namespace Game
{
    public class GoldSaveData : MapData
    {
        public int Count;
        
        public override void Init()
        {
            base.Init();
        }

        public GoldSaveData(int count) : base()
        {
            Count = count;
        }

        public override void OnReact()
        {
            base.OnReact();
            GameManager.Instance.PlayerData.OnGain(Count);
            Destroyed();
        }
    }
}
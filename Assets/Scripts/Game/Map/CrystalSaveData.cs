using Managers;
using UI;

namespace Game
{
    public class CrystalSaveData : MapData
    {
        public Rank Rank;
        public string Id;

        public CrystalSaveData(Rank r)
        {
            Rank = r;
        }

        public override void Init()
        {
            base.Init();
            Id = CrystalManager.Instance.Lib.ChooseRandom(Rank)[0].Id;
        }

        public override void OnReact()
        {
            base.OnReact();
            Destroyed();
        }
    }
}
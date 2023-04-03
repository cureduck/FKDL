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
            Id = CrystalManager.Instance.Lib.ChooseRandom(r)[0].Id;
        }
        
        public override void OnReact()
        {
            var r = (Area > 9) ? Rank.Uncommon : Rank.Normal;

            var panel = WindowManager.Instance.CrystalPanel;
            panel.gameObject.SetActive(true);
            panel.Load(Id);
            base.OnReact();
            Destroyed();
        }
    }
}
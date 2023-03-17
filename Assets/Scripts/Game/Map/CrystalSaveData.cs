using Managers;
using UI;

namespace Game
{
    public class CrystalSaveData : MapData
    {
        public Rank Rank;

        public CrystalSaveData(Rank r)
        {
            Rank = r;
        }
        
        public override void OnReact()
        {
            var r = (Area > 9) ? Rank.Uncommon : Rank.Normal;

            var panel = WindowManager.Instance.CrystalPanel;
            panel.gameObject.SetActive(true);
            panel.Load((CrystalManager.Instance.Lib.ChooseRandom(r)[0].Id));
            
            base.OnReact();
            Destroyed();
        }
    }
}
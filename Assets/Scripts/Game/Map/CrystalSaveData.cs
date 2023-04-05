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

            AudioPlayer.Instance.Play(AudioPlayer.AudioCrystal);

            var panel = WindowManager.Instance.CrystalPanel;
            WindowManager.Instance.CrystalPanel.Open(new CrystalPanel.Args { crystal = CrystalManager.Instance.Lib[Id], playerData = GameManager.Instance.PlayerData });
            panel.gameObject.SetActive(true);
            base.OnReact();
            Destroyed();
        }
    }
}
using Game;
using Managers;
using TMPro;

namespace UI
{
    public class PlayerPanel : FighterPanel<PlayerPanel>
    {
        public PotionPanel PotionPanel;
        public GoldPanel GoldPanel;
        
        private void Start()
        {

            GameManager.Instance.GameLoaded += () =>
            {
                Master = GameManager.Instance.PlayerData;
            };
        }


        protected override void SetMaster(FighterData master)
        {
            base.SetMaster(master);
            GoldPanel.SetMaster(master);
            PotionPanel.SetMaster(master);
        }
    }
}
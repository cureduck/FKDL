using Game;
using Managers;

namespace UI
{
    public class PlayerPanel : FighterPanel<PlayerPanel>
    {
        public PotionPanel PotionPanel;
        
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
        }
    }
}
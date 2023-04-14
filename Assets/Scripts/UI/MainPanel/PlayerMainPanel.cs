using Game;
using Managers;
using TMPro;
using UnityEngine;

namespace UI
{
    public class PlayerMainPanel : FighterPanel<PlayerMainPanel>
    {
        public PotionGroupView PotionPanel;
        public GoldPanel GoldPanel;
        [SerializeField]
        private CareerInformationView profInformationView;
        [SerializeField]
        private PlayerRelicListView relicListView;
        private void Start()
        {
            GameManager.Instance.GameLoaded += () =>
            {
                Init();
                relicListView.Init();
                PlayerData playerData = GameManager.Instance.PlayerData;
                playerData.Buffs.Add_Test(new BuffData("Blood", 1));
                playerData.Buffs.Add_Test(new BuffData("Attack_Increase", 4));

                playerData.Relics.Add(new RelicData("1203",2));
                playerData.Relics.Add(new RelicData("Luck",1));

                Master = playerData;
                Debug.Log(playerData.Buffs.Count);

            };
        }


        protected override void SetMaster(FighterData master)
        {
            base.SetMaster(master);
            GoldPanel.SetMaster(master);
            PotionPanel.SetMaster(master);
            PlayerData playerData = master as PlayerData;
            if (playerData != null) 
            {
                profInformationView.SetData(playerData.profInfo);
                relicListView.SetData(playerData.Relics);
            }

        }
    }
}
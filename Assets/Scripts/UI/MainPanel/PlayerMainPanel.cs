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
                if (GameDataManager.Instance.SecondaryData.Prof == null|| GameDataManager.Instance.SecondaryData.Prof.Length<3)
                {
                    GameDataManager.Instance.SecondaryData.Prof = new string[3] { "CUR", "BAR", "KNI" };
                }
                Debug.LogWarning(GameDataManager.Instance.SecondaryData.Prof.Length);
                for (int i = 0; i < GameDataManager.Instance.SecondaryData.Prof.Length; i++)
                {
                    Debug.Log(GameDataManager.Instance.SecondaryData.Prof[i]);
                }
                playerData.profInfo = GameDataManager.Instance.SecondaryData.Prof;

                playerData.Buffs.Add_Test(new BuffData("Blood", 1));
                playerData.Buffs.Add_Test(new BuffData("Attack_Increase", 4));

                playerData.Relics.Add(new RelicData("1203",2));
                playerData.Relics.Add(new RelicData("Luck",1));

                Master = playerData;
                Debug.Log(playerData.Buffs.Count);

            };
        }

        public void PlayGetItemEffect(Offer offer,Vector2 screenPosiion) 
        {
            if (offer.Kind == Offer.OfferKind.Key) 
            {
                GoldPanel.PlayGetKeyEffect(screenPosiion, offer.Rank);
            }

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
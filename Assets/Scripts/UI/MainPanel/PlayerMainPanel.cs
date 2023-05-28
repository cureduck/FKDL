using System;
using Game;
using I2.Loc;
using Managers;
using Tools;
using UnityEngine;

namespace UI
{
    public class PlayerMainPanel : FighterPanel<PlayerMainPanel>
    {
        public PotionGroupView PotionPanel;
        public GoldPanel GoldPanel;
        [SerializeField] private CareerInformationView profInformationView;
        [SerializeField] private PlayerRelicListView relicListView;
        [SerializeField] private TMP_TextAnimation paStateViewTransform;
        [SerializeField] private TMP_TextAnimation pdStateViewTransform;
        [SerializeField] private TMP_TextAnimation maStateViewTransform;
        [SerializeField] private TMP_TextAnimation mdStateViewTransform;
        [SerializeField] private Localize FloorInfo;

        private void Start()
        {
            GameManager.Instance.GameLoaded += () =>
            {
                Init();
                relicListView.Init();
                PlayerData playerData = GameManager.Instance.Player;
                if (GameDataManager.Instance.SecondaryData.Profs == null ||
                    GameDataManager.Instance.SecondaryData.Profs.Length < 3)
                {
                    GameDataManager.Instance.SecondaryData.Profs = new string[3] { "CUR", "BAR", "KNI" };
                }

                for (int i = 0; i < GameDataManager.Instance.SecondaryData.Profs.Length; i++)
                {
                    Debug.Log(GameDataManager.Instance.SecondaryData.Profs[i]);
                }

                playerData.profInfo = GameDataManager.Instance.SecondaryData.Profs;

                GameManager.Instance.Marched += UpdateFloorInfo;
                UpdateFloorInfo(GameManager.Instance.CurFloor);

                Master = playerData;
            };
        }

        private void UpdateFloorInfo(string v)
        {
            try
            {
                if (v.StartsWith("A"))
                {
                    var s = v.Remove(0, 1).Remove(1, 1);
                    FloorInfo.SetLocalizeParam("P1", int.Parse(s).ToChineseOrdinal());
                }
                else
                {
                    FloorInfo.SetLocalizeParam("P1", int.Parse(v).ToChineseOrdinal());
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"楼层错误 {v} {e.Message}");
            }
        }


        public void PlayGetItemEffect(Offer offer, Vector2 screenPosition)
        {
            if (offer.Kind == Offer.OfferKind.Key)
            {
                GoldPanel.PlayGetKeyEffect(screenPosition, offer.Rank);
            }
        }

        /// <summary>
        /// 获得对应的属性点提升
        /// </summary>
        /// <param name="type">0表示物攻，1表示魔攻，2表示物防，3表示魔防</param>
        /// <param name="screenPosiion"></param>
        public void PlayGetCharacterPointEffect(int type)
        {
            TMP_TextAnimation targetTrans;
            if (type == 0)
            {
                targetTrans = paStateViewTransform;
            }
            else if (type == 1)
            {
                targetTrans = maStateViewTransform;
            }
            else if (type == 2)
            {
                targetTrans = pdStateViewTransform;
            }
            else if (type == 3)
            {
                targetTrans = mdStateViewTransform;
            }
            else
            {
                return;
            }

            targetTrans.SetTempColor(Color.yellow);
            targetTrans.SetTempFontSize(2);
        }

        private void OnStrengthen(BattleStatus status)
        {
            if (status.PAtk > 0)
            {
                PlayGetCharacterPointEffect(0);
            }

            if (status.MAtk > 0)
            {
                PlayGetCharacterPointEffect(1);
            }

            if (status.PDef > 0)
            {
                PlayGetCharacterPointEffect(2);
            }

            if (status.MDef > 0)
            {
                PlayGetCharacterPointEffect(3);
            }
        }


        protected override void SetMaster(FighterData master)
        {
            if (Master != null)
            {
                Master.OnUpdated -= UpdateView;
                Master.Strengthened -= OnStrengthen;
            }

            base.SetMaster(master);
            master.OnUpdated += UpdateView;
            master.Strengthened += OnStrengthen;


            GoldPanel.SetMaster(master);
            PotionPanel.SetMaster(master);
            if (master is PlayerData playerData)
            {
                profInformationView.SetData(playerData.profInfo);
                relicListView.SetData(playerData.Relics);
            }

            UpdateView();
        }


        public void UpdateView()
        {
            PlayerData playerData = Master as PlayerData;
            buffListView.SetData(playerData.Buffs);
            if (playerData != null)
            {
                profInformationView.SetData(playerData.profInfo);
                relicListView.SetData(playerData.Relics);
            }
        }
    }
}
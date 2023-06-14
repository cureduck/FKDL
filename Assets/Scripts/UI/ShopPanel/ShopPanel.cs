using CH.ObjectPool;
using Game;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ShopPanel : BasePanel<ShopSaveData>
    {
        [SerializeField] private CellGoodView relicView;
        [SerializeField] private CellGoodView skillAndPotionPrefab;
        [SerializeField] private CellGoodView keyPrefab;
        [SerializeField] private Transform skillListView;
        [SerializeField] private Transform potionListView;
        [SerializeField] private Transform keyListView;
        [SerializeField] private Button sellPoition_btn;

        //[SerializeField] private Button levelUp_btn;

        //[SerializeField] private RectTransform levelUpCostRect;
        [SerializeField] private TMP_Text levelUpCost_txt;
        [SerializeField] private Button reflash_btn;
        [SerializeField] private TMP_Text reflashCost_txt;
        private UIViewObjectPool<CellGoodView, Args> keyListObjectPool;
        private UIViewObjectPool<CellGoodView, Args> potionListObjectPool;

        private UIViewObjectPool<CellGoodView, Args> skillListObjectPool;

        public override void Init()
        {
            skillListObjectPool = new UIViewObjectPool<CellGoodView, Args>(skillAndPotionPrefab, null);
            potionListObjectPool = new UIViewObjectPool<CellGoodView, Args>(skillAndPotionPrefab, null);
            keyListObjectPool = new UIViewObjectPool<CellGoodView, Args>(keyPrefab, null);

            sellPoition_btn.onClick.AddListener(SellPotionClick);
            //levelUp_btn.onClick.AddListener(LevelUpClick);
            reflash_btn.onClick.AddListener(ReflashClick);
            gameObject.SetActive(false);
        }

        protected override void UpdateUI()
        {
            //Data.Goods.SkillList = new Offer[2] { 
            //    new Offer { Id = "DZXY_ALC".ToLower(), Kind = Offer.OfferKind.Skill, Cost = new CostInfo(10) },
            //    new Offer { Id = "YWLZ_ALC".ToLower(), Kind = Offer.OfferKind.Skill, Cost = new CostInfo(10) },
            //};
            //LayoutRebuilder.ForceRebuildLayoutImmediate(levelUpCostRect);
            Args[] args = new Args[Data.Goods.SkillList.Length];
            for (int i = 0; i < args.Length; i++)
            {
                args[i] = new Args
                {
                    offer = Data.Goods.SkillList[i], curHaveGold = GameManager.Instance.Player.Gold, index = i,
                    onClick = OnCellGoodsClick
                };
            }

            Debug.Log(args.Length);
            skillListObjectPool.SetDatas(args, CellSet, skillListView);

            args = new Args[Data.Goods.PotionList.Length];
            for (int i = 0; i < args.Length; i++)
            {
                args[i] = new Args
                {
                    offer = Data.Goods.PotionList[i], curHaveGold = GameManager.Instance.Player.Gold, index = i,
                    onClick = OnCellGoodsClick
                };
            }

            potionListObjectPool.SetDatas(args, CellSet, potionListView);

            args = new Args[Data.Goods.KeyList.Length];
            for (int i = 0; i < args.Length; i++)
            {
                args[i] = new Args
                {
                    offer = Data.Goods.KeyList[i], curHaveGold = GameManager.Instance.Player.Gold, index = i,
                    onClick = OnCellGoodsClick
                };
            }

            keyListObjectPool.SetDatas(args, CellSet, keyListView);
            PlayerData playerData = GameManager.Instance.Player;
            //if (Data.UpgradeCost.Value > playerData.Gold)
            //{
            //    levelUpCost_txt.text = $"<color=red>{Data.UpgradeCost.Value}</color>";
            //}
            //else
            //{
            //    levelUpCost_txt.text = $"<color=yellow>{Data.UpgradeCost.Value}</color>";
            //}

            reflash_btn.interactable = Data.RefreshCost.Value <= playerData.Gold;
            if (Data.RefreshCost.Value > playerData.Gold)
            {
                reflashCost_txt.text = $"<color=red>{Data.RefreshCost.Value}</color>";
            }
            else
            {
                reflashCost_txt.text = $"<color=yellow>{Data.RefreshCost.Value}</color>";
            }
        }


        public override void Close()
        {
            base.Close();
        }

        private void CellSet(CellGoodView arg1, Args arg2)
        {
            arg1.SetData(arg2.index, arg2.curHaveGold, arg2.offer, arg2.onClick);
        }

        private void OnCellGoodsClick(CellGoodView cellGoodView, Offer offer, int index)
        {
            //FailureInfo会在创建的时候就自动播报，不用在这里再播报一次
            //会有资源不够的info返回
            if (PlayerData.CanAfford(offer.Cost, out var info))
            {
                //可能会有技能槽位不够、药水槽位不够等info返回
                if (PlayerData.TryTakeOffer(offer, out info))
                {
                    if (offer.Kind == Offer.OfferKind.Skill)
                    {
                        Data.Goods.SkillList[index].isSold = true;
                    }
                    else if (offer.Kind == Offer.OfferKind.Potion)
                    {
                        Data.Goods.PotionList[index].isSold = true;
                    }
                    else if (offer.Kind == Offer.OfferKind.Key)
                    {
                        Data.Goods.KeyList[index].isSold = true;
                    }

                    PlayerMainPanel.Instance.PlayGetItemEffect(offer, cellGoodView.transform.position);
                    UpdateUI();
                }
                else
                {
                    WindowManager.Instance.warningInfoPanel.Open(info.ToString());
                    info.BroadCastInfo();
                }
            }
            else
            {
                info.BroadCastInfo();
            }

            /*if (playerData.Gold < offer.Cost.ActualValue)
            {
                Debug.LogWarning("金币不足");
            }
            else 
            {
                playerData.

                if (offer.Kind == Offer.OfferKind.Skill)
                {
                    
                }

            }*/
        }

        private void SellPotionClick()
        {
            Debug.LogError("出售药水按钮被点击");
        }

        private void LevelUpClick()
        {
            //Data.UpGradeCost
            if (Data.UpgradeCost.Value <= GameManager.Instance.Player.Gold)
            {
                Data.Upgrade();
            }
            else
            {
                WindowManager.Instance.warningInfoPanel.Open("金币不足");
            }
        }

        private void ReflashClick()
        {
            Data.Goods = Data.Refresh();
            UpdateUI();
            //Debug.LogError("刷新按钮被点击");
        }

        private class Args
        {
            public int curHaveGold;
            public int index;
            public Offer offer;
            public System.Action<CellGoodView, Offer, int> onClick;
        }
    }
}
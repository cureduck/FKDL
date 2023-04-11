﻿using System.Linq;
using Game;
using I2.Loc;
using Managers;
using Sirenix.OdinInspector;
using UnityEngine;
using CH.ObjectPool;
using UI;
using System;
using UnityEngine.UI;

namespace UI
{
    public class ShopPanel : BasePanel<ShopSaveData>
    {
        private class Args
        {
            public Offer offer;
            public int index;
            public int curHaveGold;
            public System.Action<CellGoodView, Offer, int> onClick;
        }

        [SerializeField]
        private CellGoodView relicView;
        [SerializeField]
        private CellGoodView skillAndPotionPrefab;
        [SerializeField]
        private CellGoodView keyPrefab;
        [SerializeField]
        private Transform skillListView;
        [SerializeField]
        private Transform potionListView;
        [SerializeField]
        private Transform keyListView;
        [SerializeField]
        private Button sellPoition_btn;
        [SerializeField]
        private Button levelUp_btn;
        [SerializeField]
        private Button reflash_btn;

        private UIViewObjectPool<CellGoodView, Args> skillListObjectPool;
        private UIViewObjectPool<CellGoodView, Args> potionListObjectPool;
        private UIViewObjectPool<CellGoodView, Args> keyListObjectPool;

        public override void Init()
        {
            skillListObjectPool = new UIViewObjectPool<CellGoodView, Args>(skillAndPotionPrefab, null);
            potionListObjectPool = new UIViewObjectPool<CellGoodView, Args>(skillAndPotionPrefab, null);
            keyListObjectPool = new UIViewObjectPool<CellGoodView, Args>(keyPrefab, null);

            sellPoition_btn.onClick.AddListener(SellPotionClick);
            levelUp_btn.onClick.AddListener(LevelUpClick);
            reflash_btn.onClick.AddListener(ReflashClick);

        }

        protected override void UpdateUI()
        {
            Data.Goods.SkillList = new Offer[2] { 
                new Offer { Id = "DZXY_ALC".ToLower(), Kind = Offer.OfferKind.Skill, Cost = new CostInfo(10) },
                new Offer { Id = "YWLZ_ALC".ToLower(), Kind = Offer.OfferKind.Skill, Cost = new CostInfo(10) },
            };

            Args[] args = new Args[Data.Goods.SkillList.Length];
            for (int i = 0; i < args.Length; i++)
            {
                args[i] = new Args { offer = Data.Goods.SkillList[i], curHaveGold = GameManager.Instance.PlayerData.Gold, index = i, onClick = OnCellGoodsClick };
            }
            Debug.Log(args.Length);
            skillListObjectPool.SetDatas(args, CellSet, skillListView);

            args = new Args[Data.Goods.PotionList.Length];
            for (int i = 0; i < args.Length; i++)
            {
                args[i] = new Args { offer = Data.Goods.PotionList[i], curHaveGold = GameManager.Instance.PlayerData.Gold,index = i, onClick = OnCellGoodsClick };
            }
            potionListObjectPool.SetDatas(args, CellSet, potionListView);

            args = new Args[Data.Goods.KeyList.Length];
            for (int i = 0; i < args.Length; i++)
            {
                args[i] = new Args { offer = Data.Goods.KeyList[i], curHaveGold = GameManager.Instance.PlayerData.Gold,index = i, onClick = OnCellGoodsClick };
            }
            keyListObjectPool.SetDatas(args, CellSet, keyListView);


        }


        private void CellSet(CellGoodView arg1, Args arg2)
        {
            arg1.SetData(arg2.index, arg2.curHaveGold, arg2.offer, arg2.onClick);
        }

        private void OnCellGoodsClick(CellGoodView cellGoodView, Offer offer,int index) 
        {

            //FailureInfo会在创建的时候就自动播报，不用在这里再播报一次
            Info info;
            //会有资源不够的info返回
            if (PlayerData.CanAfford(offer.Cost, out info))
            {
                //可能会有技能槽位不够、药水槽位不够等info返回
                PlayerData.TryTakeOffer(offer, out info);
            }
            else
            {
                
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
            Debug.LogError("升级按钮被点击");
        }

        private void ReflashClick() 
        {
            Data.Refresh();
            UpdateUI();
            //Debug.LogError("刷新按钮被点击");
        }

    }
}
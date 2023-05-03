using System;
using System.Linq;
using Managers;
using Sirenix.OdinInspector;
using UI;

namespace Game
{
    public class ShopGoods
    {
        public Offer[] SkillList;
        public Offer[] PotionList;
        public Offer[] KeyList;
    }
    
    public class ShopSaveData : MapData
    {
        public ShopGoods Goods;
        
        public CostInfo RefreshCost { get; }

        public override void Init()
        {
            base.Init();
            Goods = GenerateGoods();
        }

        public override void OnReact()
        {
            base.OnReact();
        }

        public override void OnLeave()
        {
            base.OnLeave();
        }

        private const int SkillNum = 3;
        private const int PotionNum = 3;
        private const int KeyNum = 3;
        
        [Button]
        private ShopGoods GenerateGoods()
        {
            var skillGoods = SkillManager.Instance.GenerateT(Rank.Normal, Player.LuckyChance, SkillNum)
                .Select((good => 
                    new Offer(good, GetPrice(good))))
                .ToArray();
            
            var potionGoods = PotionManager.Instance.GenerateT(Rank.Normal, Player.LuckyChance, PotionNum)
                .Select((good => new Offer(good, GetPrice(good))))
                .ToArray();
            
            var keyGoods = new Offer[3]
            {
                new Offer(Rank.Normal, GetKeyPrice(Rank.Normal)),
                new Offer(Rank.Uncommon, GetKeyPrice(Rank.Uncommon)),
                new Offer(Rank.Rare, GetKeyPrice(Rank.Rare)),
            };

            var goods = new ShopGoods {SkillList = skillGoods, PotionList = potionGoods, KeyList = keyGoods};
            return goods;
        }

        public ShopGoods Refresh()
        {
            return GenerateGoods();
        }


        public CostInfo UpGradeCost => new CostInfo(100);
        
        
        
        
        private int GetPrice(Offer.OfferKind kind, Rank rank)
        {
            return 10;
        }

        private int GetPrice(CsvData good)
        {
            int basePrice;
            switch (good)
            {
                case Potion potion:
                    basePrice = 10;
                    break;
                case Relic relic:
                    basePrice = 30;
                    break;
                case Skill skill:
                    basePrice = 20;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(good));
            }

            float multi = 5f;
            
            switch (good.Rank)
            {
                case Rank.Normal:
                    multi = 1;
                    break;
                case Rank.Uncommon:
                    multi = 2;
                    break;
                case Rank.Rare:
                    multi = 3;
                    break;
                case Rank.Ultra:
                    break;
                case Rank.Prof:
                    break;
                case Rank.God:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var r = 0.8f + .4f * SData.CurGameRandom.NextDouble();
            
            return (int)(basePrice * multi *r);
        }

        private int GetKeyPrice(Rank rank)
        {
            switch (rank)
            {
                case Rank.Normal:
                    return 10;
                    break;
                case Rank.Uncommon:
                    break;
                case Rank.Rare:
                    break;
                case Rank.Ultra:
                    break;
                case Rank.Prof:
                    break;
                case Rank.God:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(rank), rank, null);
            }
            return 10;
        }

    }
}
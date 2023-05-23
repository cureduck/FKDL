using System;
using System.Linq;
using Managers;
using Sirenix.OdinInspector;

namespace Game
{
    public class ShopGoods
    {
        public Offer[] KeyList;
        public Offer[] PotionList;
        public Offer[] SkillList;
    }

    public class ShopSaveData : MapData
    {
        private const int SkillNum = 3;
        private const int PotionNum = 3;
        private const int KeyNum = 3;

        private static readonly float[] PotionPriceMulti = new[] { 1f, 2f, 4f, 7f };
        private static readonly float[] SkillPriceMulti = new[] { 2f, 3f, 4f };
        private static readonly float[] RelicPriceMulti = new[] { 2f, 3f, 4f };
        public ShopGoods Goods;
        public int Level;

        public CostInfo RefreshCost => new CostInfo(10);


        public CostInfo UpgradeCost => new CostInfo(100, CostType.Gold);

        public void Upgrade()
        {
            Player.Cost(UpgradeCost);
            AdjustGoodQuality();
            Level++;
        }

        private void AdjustGoodQuality()
        {
        }


        public override void Init()
        {
            base.Init();
            Level = 0;
            Goods = GenerateGoods();
        }

        public override void OnReact()
        {
            RevealAround();
            base.OnReact();
        }

        public override void OnLeave()
        {
            base.OnLeave();
        }

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

            var goods = new ShopGoods { SkillList = skillGoods, PotionList = potionGoods, KeyList = keyGoods };
            return goods;
        }

        public ShopGoods Refresh()
        {
            Player.Cost(RefreshCost);
            return GenerateGoods();
        }


        private int GetPrice(Offer.OfferKind kind, Rank rank)
        {
            return 10;
        }

        private int GetPrice(CsvData good)
        {
            int basePrice;
            float multi;
            switch (good)
            {
                case Potion potion:
                    basePrice = 15;
                    multi = PotionPriceMulti[(int)potion.Rank];
                    break;
                case Relic relic:
                    basePrice = 30;
                    multi = RelicPriceMulti[(int)relic.Rank];
                    break;
                case Skill skill:
                    basePrice = 20;
                    multi = SkillPriceMulti[(int)skill.Rank];
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(good));
            }

            var r = 0.8f + .4f * SData.CurGameRandom.NextDouble();


            return (int)(basePrice * multi * r);
        }

        private int GetKeyPrice(Rank rank)
        {
            switch (rank)
            {
                case Rank.Normal:
                    return 10;
                    break;
                case Rank.Uncommon:
                    return 15;
                    break;
                case Rank.Rare:
                    return 20;
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
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
            return 10;
        }

        private int GetKeyPrice(Rank rank)
        {
            return 10;
        }

    }
}
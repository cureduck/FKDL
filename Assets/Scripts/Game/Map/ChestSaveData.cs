using System.Linq;
using Managers;
using Newtonsoft.Json;

namespace Game
{
    public sealed class ChestSaveData : MapData
    {
        private const float SkillChance = .22f;
        public Offer[] Offers;
        public Rank Rank;

        public ChestSaveData(Rank rank) : base()
        {
            Rank = rank;
        }

        [JsonIgnore] public Offer.OfferKind RewardType => Offers[0].Kind; //0表示技能，1表示药水，2表示遗物
        [JsonIgnore] public int SkipGold => 10;

        public override void Init()
        {
            base.Init();

            Offers = new Offer[3];

            if (Rank != Rank.Normal || SData.CurGameRandom.NextDouble() < SkillChance)
            {
                var skills = SkillManager.Instance.RollT(Rank, 3);

                Offers = skills.Select((s => new Offer(s))).ToArray();
            }
            else
            {
                var potions = PotionManager.Instance.GetAttrPotion(3);
                Offers = potions.Select((s => new Offer(s))).ToArray();
            }
        }

        public override void OnReact()
        {
            base.OnReact();
            Destroyed();
        }

        public override SquareInfo GetSquareInfo()
        {
            var info = base.GetSquareInfo();
            info.P1 = Rank.ToString().ToLower();
            return info;
        }
    }
}
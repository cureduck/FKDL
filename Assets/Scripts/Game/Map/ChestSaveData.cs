using System.Linq;
using Managers;

namespace Game
{
    public sealed class ChestSaveData : MapData
    {
        private const float SkillChance = .22f;
        public Offer[] Offers;
        public Rank Rank;
        public int rewardType; //0表示技能，1表示药水，2表示遗物

        public ChestSaveData(Rank rank) : base()
        {
            Rank = rank;
        }

        public override void Init()
        {
            base.Init();

            Offers = new Offer[3];

            if (Rank != Rank.Normal || SData.CurGameRandom.NextDouble() < SkillChance)
            {
                var skills = SkillManager.Instance.RollT(Rank, 3);

                Offers = skills.Select((s => new Offer(s))).ToArray();
                rewardType = 0;
            }
            else
            {
                var potions = PotionManager.Instance.GetAttrPotion(3);
                Offers = potions.Select((s => new Offer(s))).ToArray();
                rewardType = 1;
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
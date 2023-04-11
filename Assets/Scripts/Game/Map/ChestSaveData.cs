using System;
using System.Linq;
using Managers;

namespace Game
{
    public sealed class ChestSaveData : MapData
    {
        public Rank Rank;
        public Offer[] Offers;

        public ChestSaveData(Rank rank) : base()
        {
            Rank = rank;
        }
        

        public override void Init()
        {
            base.Init();

            Offers = new Offer[3];
            var skills = SkillManager.Instance.RollT(Rank, 3);
            Offers = skills.Select((s => new Offer(s))).ToArray();
        }

        public override void OnReact()
        {
            base.OnReact();
            Destroyed();
        }
    }
}
using System;
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
            var skills = SkillManager.Instance.Roll(Rank, 3);
            for (int i = 0; i < 3; i++)
            {
                Offers[i] = new Offer()
                {
                    Id = skills[i],
                    Kind = Offer.OfferKind.Skill
                };
            }
        }

        public override void OnReact()
        {
            base.OnReact();
            WindowManager.Instance.OffersWindow.Load(Offers);
            Destroyed();
        }
    }
}
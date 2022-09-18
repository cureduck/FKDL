using System.Collections.Generic;
using Managers;
using UnityEngine;

namespace Game
{
    public class CasinoSaveData : MapData
    {
        public const int MaxTimes = 3;
        
        public int TimesLeft;
        public Rank Rank;
        public int Cost;

        private readonly Dictionary<Rank, int> _cost = new Dictionary<Rank, int>
            {{Rank.Normal, 3}, {Rank.Rare, 6}, {Rank.Uncommon, 9}, {Rank.Ultra, 12}};

        public override void Init()
        {
            base.Init();
            Rank = _rank;
            TimesLeft = MaxTimes;
            Cost = _cost[Rank];
        }


        public override void OnReact()
        {
            base.OnReact();
            if (GameManager.Instance.PlayerData.Gold >= Cost)
            {
                GameManager.Instance.PlayerData.Gold -= Cost;
                if (Random.Range(0f, 1f)> .5f)
                {
                    var offers = new Offer[3];
                    var potions = PotionManager.Instance.Roll(Rank, 3);
                    for (int i = 0; i < 3; i++)
                    {
                        offers[i] = new Offer()
                        {
                            Id = potions[i],
                            Kind = Offer.OfferKind.Potion
                        };
                    }
                    WindowManager.Instance.OffersWindow.Load(offers);
                }
                else
                {
                    WindowManager.Instance.Warn("You Lose");
                }
                
                TimesLeft -= 1;
                if (TimesLeft <= 0)
                {
                    Destroy();
                }
                Update();
            }
            else
            {
                WindowManager.Instance.Warn("Not Enough Gold");
            }
        }
    }
}
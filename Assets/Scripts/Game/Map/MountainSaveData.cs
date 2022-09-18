using System.Collections.Generic;
using Managers;
using UnityEngine;

namespace Game
{
    public class MountainSaveData : MapData
    {
        public const int MaxTimes = 3;
        
        public int TimesLeft;
        public Rank Rank;
        public int Cost;

        private readonly Dictionary<Rank, int> _cost = new Dictionary<Rank, int>
            {{Rank.Normal, 3}, {Rank.Uncommon, 6}, {Rank.Rare, 9}, {Rank.Ultra, 12}};

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
            if (GameManager.Instance.PlayerData.PlayerStatus.CurSp >= Cost)
            {
                GameManager.Instance.PlayerData.PlayerStatus.CurSp -= Cost;
                if (Random.Range(0f, 1f)> .5f)
                {
                    var offers = new Offer[3];
                    var skills = SkillManager.Instance.Roll(Rank, 3);
                    for (int i = 0; i < 3; i++)
                    {
                         offers[i] = new Offer()
                        {
                            Id = skills[i],
                            Kind = Offer.OfferKind.Skill
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
                    OnDestroy?.Invoke();
                }
                OnUpdated?.Invoke();
            }
            else
            {
                WindowManager.Instance.Warn("Not Enough Sp");
            }
            
            
        }
    }
}
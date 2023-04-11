using System.Collections.Generic;
using System.Linq;
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
                GameManager.Instance.PlayerData.Gain(-Cost);
                if (Random.Range(0f, 1f)> .5f)
                {
                    var potions = PotionManager.Instance.RollT(Rank, 3);
                    var offers = potions.
                        Select((s => new Offer(s)));
                    
                    InformReactResult(new CasinoArgs()
                    {
                        CanReact = true,
                        Info = new SuccessInfo(),
                        Win = true,
                        Offers = offers.ToArray()
                    });
                }
                else
                {
                    InformReactResult(new CasinoArgs()
                    {
                        CanReact = true,
                        Info = new SuccessInfo(),
                        Win = false
                    });
                }
                
                TimesLeft -= 1;
                if (TimesLeft <= 0)
                {
                    Destroyed();
                }
                Updated();
            }
            else
            {
                InformReactResult(new CasinoArgs()
                {
                    CanReact = false,
                    Info = new FailureInfo(FailureReason.NotEnoughGold)
                });
            }
        }
    }
}
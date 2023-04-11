using System.Collections.Generic;
using Managers;
using UnityEngine;

namespace Game
{
    public class CemeterySaveData : MapData
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
            if (GameManager.Instance.PlayerData.CurHp >= Cost)
            {
                GameManager.Instance.PlayerData.Cost(new CostInfo(_cost[Rank], CostType.Mp));
                if (Random.Range(0f, 1f)> .5f)
                {
                    InformReactResult(new CasinoArgs(){CanReact = true, Win = true});
                }
                else
                {
                    InformReactResult(new CasinoArgs(){CanReact = true, Win = false});
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
                    Win = false,
                    Info = new FailureInfo(FailureReason.NotEnoughHp)
                });
            }
        }
    }
}
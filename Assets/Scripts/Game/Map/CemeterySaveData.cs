using System.Collections.Generic;
using System.Linq;
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
            if (Player.CurHp >= Cost)
            {
                Player.Cost(new CostInfo(_cost[Rank], CostType.Mp));
                if (SecondaryData.CurGameRandom.NextDouble() < Player.LuckyChance)
                {
                    
                    var skills = SkillManager.Instance.RollT(Rank, 3);
                    var offers = skills.
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
                    InformReactResult(new CasinoArgs(){CanReact = true, Win = false});
                }
                
                TimesLeft -= 1;
                if (TimesLeft <= 0)
                {
                    Destroyed();
                }
                DelayUpdate();
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
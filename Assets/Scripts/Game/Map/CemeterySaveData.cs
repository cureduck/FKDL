using System.Collections.Generic;
using System.Linq;
using Managers;

namespace Game
{
    public class CemeterySaveData : MapData
    {
        public const int MaxTimes = 3;

        private readonly Dictionary<Rank, int> _cost = new Dictionary<Rank, int>
            { { Rank.Normal, 10 }, { Rank.Uncommon, 15 }, { Rank.Rare, 20 }, { Rank.Ultra, 12 } };

        public float BaseBias = 0f;

        public int Cost;
        public Rank Rank;

        public int TimesLeft;

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
                Player.Cost(new CostInfo(_cost[Rank], CostType.Hp));
                if (SData.CurGameRandom.NextDouble() < Player.LuckyChance)
                {
                    var skills = SkillManager.Instance.RollT(Rank, 3);
                    var offers = skills.Select((s => new Offer(s)));
                    Player.LuckyChance -= .15f;
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
                    Player.LuckyChance += .05f;
                    InformReactResult(new CasinoArgs() { CanReact = true, Win = false });
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

        public override SquareInfo GetSquareInfo()
        {
            var info = base.GetSquareInfo();
            info.P1 = _cost[_rank].ToString();
            info.P2 = .3f.ToString("P0");
            return info;
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using Managers;

namespace Game
{
    public class CasinoSaveData : MapData
    {
        public const int MaxTimes = 3;

        private readonly Dictionary<Rank, int> _cost = new Dictionary<Rank, int>
            { { Rank.Normal, 3 }, { Rank.Rare, 6 }, { Rank.Uncommon, 9 }, { Rank.Ultra, 12 } };

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

        public override SquareInfo GetSquareInfo()
        {
            var info = base.GetSquareInfo();
            info.P1 = Cost.ToString();
            info.P2 = Player.LuckyChance.ToString("P0");
            return info;
        }

        public override void OnReact()
        {
            base.OnReact();
            if (Player.Gold >= Cost)
            {
                Player.Gain(-Cost);
                if (SData.CurGameRandom.NextDouble() > .5f)
                {
                    var potions = PotionManager.Instance.RollT(Rank, 3);
                    var offers = potions.Select((s => new Offer(s)));

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

                DelayUpdate();
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
using System.Collections.Generic;
using System.Linq;
using Managers;

namespace Game
{
    public class CasinoSaveData : MapData
    {
        public const int MaxTimes = 3;

        private readonly Dictionary<Rank, int> _cost = new Dictionary<Rank, int>
            { { Rank.Normal, 6 }, { Rank.Rare, 12 }, { Rank.Uncommon, 24 }, { Rank.Ultra, 48 } };

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

        public override SquareInfo GetSquareInfo()
        {
            var info = base.GetSquareInfo();
            info.P1 = Cost.ToString();
            info.P2 = .3f.ToString("P0");
            return info;
        }

        public override void OnReact()
        {
            base.OnReact();
            if (Player.Gold >= Cost)
            {
                Player.Gain(-Cost);
                if (SData.CurGameRandom.NextDouble() < Player.LuckyChance)
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
                    Player.LuckyChance -= .15f;
                }
                else
                {
                    InformReactResult(new CasinoArgs()
                    {
                        CanReact = true,
                        Info = new SuccessInfo(),
                        Win = false
                    });
                    Player.LuckyChance += .05f;
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
using System;
using System.Collections.Generic;
using Tools;

namespace Game
{
    public class DoorSaveData : MapData
    {
        private static readonly Dictionary<Rank, int> hpCost = new Dictionary<Rank, int>()
        {
            { Rank.Normal, 10 },
            { Rank.Uncommon, 20 },
            { Rank.Rare, 30 },
            { Rank.Ultra, 40 },
        };

        public readonly Rank Rank;
        public float OpenChance = 0.5f;

        public DoorSaveData(Rank rank)
        {
            Rank = rank;
        }

        public void Open()
        {
            InformReactResult(new DoorArgs() { CanReact = true });
            Destroyed();
        }


        public override void OnReact()
        {
            base.OnReact();
            try
            {
                if (Player.Keys[Rank] > 0)
                {
                    Player.Keys[Rank] -= 1;
                    Open();
                }
                else
                {
                    var cost = hpCost[Rank];

                    if (!Player.CanAfford(CostInfo.HpCost(cost), out var info, "door"))
                    {
                        info.BroadCastInfo();
                        return;
                    }

                    Player.Cost(CostInfo.HpCost(cost), "door");

                    if (SData.CurGameRandom.NextDouble() < OpenChance + Player.LuckyChance / 5)
                    {
                        Open();
                    }
                    else
                    {
                        InformReactResult(new DoorArgs() { CanReact = false });
                    }
                }
            }
            catch (Exception e)
            {
                Destroyed();
                throw new Exception();
            }
        }


        public override SquareInfo GetSquareInfo()
        {
            var info = base.GetSquareInfo();
            info.P1 = hpCost[Rank].ToString();
            info.P2 = Rank.ToString(RankDescType.Key);
            return info;
        }
    }
}
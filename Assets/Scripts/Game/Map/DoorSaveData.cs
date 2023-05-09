﻿using System;
using Tools;

namespace Game
{
    public class DoorSaveData : MapData
    {
        private const int HpCost = 3;
        public Rank Rank;

        public DoorSaveData(Rank rank)
        {
            Rank = rank;
        }

        public override void OnReact()
        {
            Destroyed();

            base.OnReact();
            try
            {
                if (Player.Keys[Rank] > 0)
                {
                    Player.Keys[Rank] -= 1;
                    InformReactResult(new DoorArgs() { CanReact = true });
                    Destroyed();
                }
                else
                {
                    var cost = CostInfo.HpCost(HpCost);

                    Player.Cost(cost, "door");

                    if (SData.CurGameRandom.NextDouble() < .5f)
                    {
                        InformReactResult(new DoorArgs() { CanReact = true });
                        Destroyed();
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
            info.P1 = HpCost.ToString();
            info.P2 = Rank.ToString(RankDescType.Key);
            return info;
        }
    }
}
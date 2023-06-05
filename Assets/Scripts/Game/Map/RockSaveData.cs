﻿using UnityEngine;

namespace Game
{
    public class RockSaveData : MapData
    {
        public int Cost;

        public override void Init()
        {
            base.Init();
            var c = Area;
            Cost = Random.Range(2 * c, 3 * c);
        }

        public override void OnReact()
        {
            base.OnReact();

            var cost = CostInfo.MpCost(Cost);

            if (Player.CanAfford(cost, out _))
            {
                Player.Cost(cost, "rock");
                InformReactResult(new RockArgs() { CanReact = true });
                Destroyed();
            }
            else
            {
                InformReactResult(new RockArgs()
                {
                    CanReact = false,
                    Info = new FailureInfo(FailureReason.NotEnoughMp)
                });
            }
        }

        public override SquareInfo GetSquareInfo()
        {
            var info = base.GetSquareInfo();
            info.P1 = Cost.ToString();
            return info;
        }
    }
}
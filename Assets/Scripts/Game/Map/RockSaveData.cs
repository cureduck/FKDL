using Managers;
using UnityEngine;

namespace Game
{
    public class RockSaveData : MapData
    {
        public int Cost;

        public override void Init()
        {
            base.Init();
            var c = Placement.Height * Placement.Width;
            Cost = Random.Range((int) c , (int) 2*c);
        }

        public override void OnReact()
        {
            base.OnReact();

            var cost = CostInfo.MpCost(Cost);
            
            if (Player.CanAfford(cost, out _))
            {
                Player.Cost(cost, "rock");
                InformReactResult(new RockArgs(){CanReact = true});
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
    }
}
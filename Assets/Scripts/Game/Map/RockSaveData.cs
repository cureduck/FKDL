using UnityEngine;

namespace Game
{
    public class RockSaveData : MapData
    {
        public readonly int Level;
        public int Cost;

        public RockSaveData(int level)
        {
            Level = level;
        }

        public override void Init()
        {
            base.Init();
            var c = Area;
            Cost = Random.Range((int)1.5f * c, (int)(2.5 * c + Level));
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
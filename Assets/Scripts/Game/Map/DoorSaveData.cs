using System;
using Managers;
using Random = UnityEngine.Random;

namespace Game
{
    public class DoorSaveData : MapData
    {
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
                if (GameManager.Instance.PlayerData.Keys[Rank] > 0)
                {
                    GameManager.Instance.PlayerData.Keys[Rank] -= 1;
                    InformReactResult(new DoorArgs(){CanReact = true});
                    Destroyed();
                }
                else
                {
                    var cost = CostInfo.HpCost(3);
                    
                    GameManager.Instance.PlayerData.Cost(cost, "door");

                    if (Random.Range(0f, 1f) < .3f)
                    {
                        InformReactResult(new DoorArgs(){CanReact = true});
                        Destroyed();
                    }
                    else
                    {
                        InformReactResult(new DoorArgs(){CanReact = false});
                    }
                }
            }
            catch (Exception e)
            {
                Destroyed();
                throw new Exception();
            }

        }
    }
}
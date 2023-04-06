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
                    OnReactInfo(new DoorArgs(){CanReact = true});
                    Destroyed();
                }
                else
                {
                    //WindowManager.Instance.Warn("No Key");
                    GameManager.Instance.PlayerData.Cost(BattleStatus.HP(3), "door");

                    if (Random.Range(0f, 1f) < .3f)
                    {
                        OnReactInfo(new DoorArgs(){CanReact = true});
                        Destroyed();
                    }
                    else
                    {
                        OnReactInfo(new DoorArgs(){CanReact = false});
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
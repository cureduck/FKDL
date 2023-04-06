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
            
            if (GameManager.Instance.PlayerData.Status.CurMp >= Cost)
            {
                GameManager.Instance.PlayerData.Cost(new BattleStatus{CurMp = -Cost});
                OnReactInfo(new RockArgs(){CanReact = true});
                Destroyed();
            }
            else
            {
                OnReactInfo(new RockArgs(){CanReact = false});
            }
        }
    }
}
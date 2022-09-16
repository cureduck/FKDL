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
            Cost = Random.Range((int) c / 2, (int) c);
        }

        public override void OnReact()
        {
            base.OnReact();
            
            if (GameManager.Instance.PlayerData.PlayerStatus.CurSp >= Cost)
            {
                GameManager.Instance.PlayerData.PlayerStatus.CurSp -= Cost;
                OnDestroy?.Invoke();
            }
            else
            {
                WindowManager.Instance.Warn("Not Enough Sp");
            }
        }
    }
}
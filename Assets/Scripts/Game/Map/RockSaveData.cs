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
        
        
    }
}
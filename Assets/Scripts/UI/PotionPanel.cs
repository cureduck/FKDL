using Game;
using Unity.Mathematics;
using UnityEngine.UI;

namespace UI
{
    public class PotionPanel : FighterUIPanel
    {
        public PotionBtn[] SkillItems;
        
        public int MinLen => math.min(((PlayerData) _master).Potions.Length, SkillItems.Length);

        
        public override void SetMaster(FighterData master)
        {
            if (master == _master) return;
            
            base.SetMaster(master);
        }




        private void UseMasterPotion(int index)
        {
            ((PlayerData) _master).UsePotion(index);
        }
        
        protected override void UpdateData()
        {
            
        }
    }
}
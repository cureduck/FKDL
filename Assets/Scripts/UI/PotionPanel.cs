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
            
            UnbindPrevious();
            base.SetMaster(master);
            Bind();
        }

        private void Bind()
        {
            for (int i = 0; i < MinLen; i++)
            {
                _master.Skills[i].Activate += SkillItems[i].Activate;
            }
        }

        private void Start()
        {
            for (int i = 0; i < SkillItems.Length; i++)
            {
                var i1 = i;
                SkillItems[i].GetComponent<Button>().onClick.AddListener((() =>
                {
                    UseMasterPotion(i1);
                }));
            }
        }


        private void UnbindPrevious()
        {
            if (_master == null) return;
            for (int i = 0; i < MinLen; i++)
            {
                _master.Skills[i].Activate -= SkillItems[i].Activate;
            }
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
using Game;
using Unity.Mathematics;
using UnityEngine;

namespace UI
{
    public class SkillPanel : FighterUIPanel
    {
        public SkillItem[] SkillItems;

        public int MinLen => math.min(_master.Skills.Length, SkillItems.Length);
        
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
        
        private void UnbindPrevious()
        {
            if (_master == null) return;
            for (int i = 0; i < MinLen; i++)
            {
                _master.Skills[i].Activate -= SkillItems[i].Activate;
            }
        }
        

        protected override void UpdateData()
        {
            for (int i = 0; i < MinLen; i++)
            {
                SkillItems[i].Load(_master.Skills[i]);
            }

            for (int i = MinLen; i < SkillItems.Length; i++)
            {
                SkillItems[i].Load(SkillData.Empty);
            }
        }
    }
}
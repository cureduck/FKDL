﻿using System;
using Game;
using Managers;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class SkillPanel : FighterUIPanel
    {
        public SkillItem[] SkillItems;

        public int MinLen => math.min(_master.Skills.Count, SkillItems.Length);
        
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
                    ClickSkillBtn(i1);
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
        
        
        private void ClickSkillBtn(int index)
        {
            var skill = _master.Skills[index];
            if ((skill.Bp.Positive)&&(!skill.Bp.NeedTarget))
            {
                _master.CastNonAimingSkill(index);
            }
            if ((skill.Bp.Positive)&&(skill.Bp.NeedTarget))
            {
                InputSystem.Instance.ArrangeFight((EnemySaveData)GameManager.Instance.Focus.Data, skill);
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
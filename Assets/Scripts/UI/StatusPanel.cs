using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class StatusPanel : FighterUIPanel
    {
        public bool showName = false;
        public TMP_Text Hp;
        public TMP_Text Mp;
        public TMP_Text PA;
        public TMP_Text MA;
        public TMP_Text PD;
        public TMP_Text MD;

        public Slider HpBar;
        public Slider MpBar;

        private float _targetHpPercent;
        private float _targetMpPercent;

        protected override void UpdateData()
        {
            if (showName)
            {
                Hp.text = $"生命 <color=#01F5A9>{_master.Status.CurHp}</color>/{_master.Status.MaxHp}";
            }
            else
            {
                Hp.text = $"<color=#01F5A9>{_master.Status.CurHp}</color>/{_master.Status.MaxHp}";
            }

            if (showName)
            {
                Mp.text = $"魔力 <color=#01F5A9>{_master.Status.CurMp}</color>/{ _master.Status.MaxMp}";
            }
            else
            {
                Mp.text = $"<color=#01F5A9>{_master.Status.CurMp}</color>/{ _master.Status.MaxMp}";
            }


            if (HpBar != null)
            {
                _targetHpPercent = (float)_master.Status.CurHp / _master.Status.MaxHp;
            }
            if (MpBar != null)
            {
                _targetMpPercent = (float)_master.Status.CurMp / _master.Status.MaxMp;
            }

            if (showName)
            {
                PA.text = $"物攻 {_master.Status.PAtk}";
            }
            else
            {
                PA.text = _master.Status.PAtk.ToString();
            }

            if (showName)
            {
                MA.text = $"魔攻 {_master.Status.MAtk}";
            }
            else
            {
                MA.text = _master.Status.MAtk.ToString();
            }

            if (showName)
            {
                PD.text = $"物防 { _master.Status.PDef}";
            }
            else
            {
                PD.text = _master.Status.PDef.ToString();
            }

            if (showName)
            {
                MD.text = $"魔防 {_master.Status.MDef}";
            }
            else 
            {
                MD.text = _master.Status.MDef.ToString();
            }


        }


        private void Update()
        {
            if (HpBar != null)
            {
                HpBar.value += (_targetHpPercent - HpBar.value) / 30f;
            }
            if (MpBar != null)
            {
                MpBar.value += (_targetMpPercent - MpBar.value) / 30f;
            }
        }
    }
}
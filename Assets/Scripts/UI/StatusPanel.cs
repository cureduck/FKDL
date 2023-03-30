using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class StatusPanel : FighterUIPanel
    {
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
            Hp.text = $"<color=#01F5A9>{_master.Status.CurHp}</color>/{_master.Status.MaxHp}";
            Mp.text = $"<color=#01F5A9>{_master.Status.CurMp}</color>/{ _master.Status.MaxMp}";

            if (HpBar != null)
            {
                _targetHpPercent = (float)_master.Status.CurHp / _master.Status.MaxHp;
            }
            if (MpBar != null)
            {
                _targetMpPercent = (float)_master.Status.CurMp / _master.Status.MaxMp;
            }

            PA.text = _master.Status.PAtk.ToString();
            MA.text = _master.Status.MAtk.ToString();

            PD.text = _master.Status.PDef.ToString();
            MD.text = _master.Status.MDef.ToString();
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
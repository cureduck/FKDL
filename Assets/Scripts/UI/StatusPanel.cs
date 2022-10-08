using TMPro;
using UnityEngine;

namespace UI
{
    public class StatusPanel : FighterUIPanel
    {
        public TMP_Text Hp;
        public TMP_Text Mp;
        public TMP_Text PAD;
        public TMP_Text MAD;


        protected override void UpdateData()
        {
            Hp.text = _master.Status.CurHp + "/" + _master.Status.MaxHp;
            Mp.text = _master.Status.CurMp + "/" + _master.Status.MaxMp;

            PAD.text = _master.Status.PAtk + "/" + _master.Status.PDef;
            MAD.text = _master.Status.MAtk + "/" + _master.Status.MDef;
        }
    }
}
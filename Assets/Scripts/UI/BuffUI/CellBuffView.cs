using Game;
using I2.Loc;
using TMPro;
using UnityEngine;

namespace UI.BuffUI
{
    public class CellBuffView : CellView<BuffData>
    {
        public Localize Id;
        public TMP_Text Stack;
        
        public override void UpdateUI()
        {
            Id.SetTerm(Data.Id);
            Stack.text = Data.CurLv.ToString();
            Debug.Log(Data);
        }
    }
}
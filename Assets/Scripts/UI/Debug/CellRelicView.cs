using Game;
using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.BuffUI
{
    public class CellRelicView : CellView<RelicData>
    {
        public Localize Id;
        public TMP_Text Stack;
        public Image Icon;
        
        public override void UpdateUI()
        {
            Id.SetTerm(Data.Id);
            Icon.sprite = Data?.Bp?.Icon;
        }
    }
}
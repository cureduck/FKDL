using Game;
using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class CellRelicView : CellView<RelicData>
    {
        public TMP_Text Stack;
        public Image Icon;
        
        public override void UpdateUI()
        {
            if (Data?.Bp == null) return;
            Stack.gameObject.SetActive(Data.Bp.UseCounter);
            if (Data.Bp.UseCounter)
            {
                Stack.text = Data.Counter.ToString();
            }
            Icon.sprite = Data?.Bp?.Icon;
        }

        public override string Id => Data.Id;
        public override string Desc => $"{Data.Id}_desc";
    }
}
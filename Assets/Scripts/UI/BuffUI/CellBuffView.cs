using Game;
using UnityEngine;

namespace UI.BuffUI
{
    public class CellBuffView : CellView<BuffData>
    {
        public override void UpdateUI()
        {
            Debug.Log(Data);
        }
    }
}
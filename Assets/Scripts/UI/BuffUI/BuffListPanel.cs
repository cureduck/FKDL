using System;
using System.Collections.Generic;
using Game;

namespace UI.BuffUI
{
    public class BuffListPanel : ListPanel<BuffData>
    {
        private void Awake()
        {
            Cells = new List<CellView<BuffData>>();
        }
    }
}
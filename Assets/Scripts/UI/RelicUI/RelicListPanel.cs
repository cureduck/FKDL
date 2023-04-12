using System;
using System.Collections.Generic;
using Game;

namespace UI.BuffUI
{
    public class RelicListPanel : ListPanel<RelicData>
    {
        private void Awake()
        {
            Cells = new List<CellView<RelicData>>();
        }
    }
}
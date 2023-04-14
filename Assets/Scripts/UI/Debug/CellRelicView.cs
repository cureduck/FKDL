﻿using Game;
using I2.Loc;
using TMPro;
using UnityEngine;

namespace UI.BuffUI
{
    public class CellRelicView : CellView<RelicData>
    {
        public Localize Id;
        public TMP_Text Stack;
        
        public override void UpdateUI()
        {
            Id.SetTerm(Data.Id);
            Debug.Log(Data);
        }
    }
}
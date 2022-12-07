using System;
using Game;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

namespace UI
{
    public class GoldPanel : FighterUIPanel
    {
        public TMP_Text GoldText;

        private int _targetGoldCount;
        private int _currentGoldCount;

        private int _pause;

        private void Update()
        {
            if (_targetGoldCount != _currentGoldCount)
            {
                if (_pause>0)
                {
                    _pause -= 1;
                    return;
                }
                else
                {
                    var delta = (_targetGoldCount - _currentGoldCount) / 50;
                    delta += _targetGoldCount - _currentGoldCount > 0 ? 1 : -1;
                    _currentGoldCount += delta;
                
                    GoldText.text = _currentGoldCount.ToString();
                    _pause = 10;
                }
            }
        }


        public override void SetMaster(FighterData master)
        {
            base.SetMaster(master);
        }

        protected override void UpdateData()
        {
            base.UpdateData();
            _targetGoldCount = ((PlayerData) _master).Gold;
        }
    }
}
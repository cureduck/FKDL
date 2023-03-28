﻿using System;
using Game;
using Managers;

namespace UI
{
    public class NewEnemyPanel : BasePanel<EnemySaveData>
    {
        private Square CurrentFocus;
        
        private void Start()
        {
            GameManager.Instance.FocusChanged += OnSquareChanged;
        }


        private void OnSquareChanged(Square square)
        {
            if (CurrentFocus != null)
            {
                CurrentFocus.Data.OnUpdated -= UpdateUI;
            }

            if (square.Data != null)
            {
                if (square.Data is EnemySaveData e)
                {
                    Data = e;
                    UpdateUI();
                    e.OnUpdated += UpdateUI;
                }
            }
        }
        

        protected override void UpdateUI()
        {
            throw new System.NotImplementedException();
        }


        protected override void OnOpen()
        {
            base.OnOpen();
        }
    }
}
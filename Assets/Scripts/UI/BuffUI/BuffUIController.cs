using System;
using Game;
using Managers;

namespace UI.BuffUI
{
    public class BuffUIController : UpdateablePanel<FighterData>
    {
        private BuffListView _buffListPanel;
        public bool IsPlayerBuffUI;
        
        private Square CurrentFocus;

        private void Start()
        {
            _buffListPanel = GetComponent<BuffListView>();

            if (IsPlayerBuffUI)
            {
                GameManager.Instance.GameLoaded += () =>
                {
                    SetData(GameManager.Instance.PlayerData);
                };
            }
            else
            {
                GameManager.Instance.FocusChanged += OnSquareChanged;
            }

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
                    SetData(e);
                }
            }
        }
        

        protected override void UpdateUI()
        {
            //_buffListPanel.UpdateUI(Data.Buffs);
        }
    }
}
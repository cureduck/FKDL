using System;
using Game;
using Managers;

namespace UI.BuffUI
{
    public class BuffUIController : UpdateablePanel<FighterData>
    {
        private BuffListPanel _buffListPanel;
        public bool IsPlayerBuffUI;
        
        private void Start()
        {
            _buffListPanel = GetComponent<BuffListPanel>();
            GameManager.Instance.GameLoaded +=
                () => { SetData(GameManager.Instance.PlayerData); };
        }

        protected override void UpdateUI()
        {
            _buffListPanel.UpdateUI(Data.Buffs);
        }
    }
}
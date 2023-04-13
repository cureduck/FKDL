using System;
using Game;
using Managers;

namespace UI.BuffUI
{
    public class RelicUIController : UpdateablePanel<PlayerData>
    {
        private RelicListPanel _relicListPanel;
        
        private void Start()
        {
            _relicListPanel = GetComponent<RelicListPanel>();

            GameManager.Instance.GameLoaded += () =>
            {
                SetData(GameManager.Instance.PlayerData);
            };

        }
        
        protected override void UpdateUI()
        {
            _relicListPanel.UpdateUI(Data.Relics);
        }
    }
}
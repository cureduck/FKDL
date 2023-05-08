using System;
using Game;
using Managers;

namespace UI
{
    public class EnemyBasePanel : UpdateablePanel<EnemySaveData>
    {
        private Square CurrentFocus;

        public StatusPanel StatusPanel;
        public BuffPanel BuffPanel;
        public EnemySkillInfoView EnemySkillInfoView;

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
                    Open(e);
                    //Data = e;
                    UpdateUI();
                    e.OnUpdated += UpdateUI;
                }
            }
        }


        protected override void UpdateUI()
        {
        }
    }
}
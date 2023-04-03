using System;
using Game;
using Managers;
using TMPro;

namespace UI
{
    public class NewGoldPanel : UpdateablePanel<PlayerData>
    {
        public TMP_Text gold;

        protected void Start()
        {
            //Data = GameManager.Instance.PlayerData;
            Open(GameManager.Instance.PlayerData);
            GameManager.Instance.GameLoaded +=
                () => { Open(GameManager.Instance.PlayerData); };
        }

        protected override void UpdateUI()
        {
            if (Data != null)
            {
                gold.text = Data.Gold.ToString();
            }
        }
    }
}
using System;
using Game;
using Managers;
using TMPro;

namespace UI
{
    public class NewGoldPanel : BasePanel<PlayerData>
    {
        public TMP_Text gold;

        protected void Start()
        {
            Data = GameManager.Instance.PlayerData;
            GameManager.Instance.GameLoaded +=
                () => { Data = GameManager.Instance.PlayerData; };
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
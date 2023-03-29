using System;
using Game;
using Managers;

namespace UI
{
    public abstract class PlayerInfoPanel : BasePanel<PlayerData>
    {
        protected void Start()
        {
            Data = GameManager.Instance.PlayerData;
            GameManager.Instance.GameLoaded += () =>
            {
                if (Data != null)
                {
                    Data.OnUpdated -= UpdateUI;
                }
                
                Data = GameManager.Instance.PlayerData;
            };
        }
        
        
    }
}
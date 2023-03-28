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
                Data = GameManager.Instance.PlayerData;
            };
        }
    }
}
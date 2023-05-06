using System;
using System.Collections;
using Game;
using I2.Loc;
using UI;
using UnityEngine;

namespace Managers
{
    public class WindowManager : Singleton<WindowManager>
    {
        public OfferWindow OffersWindow;
        public ShopPanel ShopPanel;
        public CrystalPanel CrystalPanel;
        public GameObject MenuWindow;
        public GameObject CheatWindow;
        public EnemyInfoPanel EnemyPanel;
        public ConfirmPanel confirmPanel;
        public FightPredictPanel FightPredictPanel;
        public WarningInfoDebugPanel warningInfoPanel;
        public SettingPanel settingPanel;

        public SquareInfoPanel SquareInfoPanel;
        public SkillInfoPanel skillInfoPanel;
        public SimpleInfoItemPanel simpleInfoItemPanel;

        public Transform dragViewParent;
        
        protected override void Awake()
        {
            base.Awake();

        }

        private void Start()
        {
            confirmPanel?.gameObject.SetActive(false);
            if (dragViewParent != null)
            {
                dragViewParent.transform.SetAsLastSibling();
            }

            CrystalPanel?.Init();
            ShopPanel?.Init();
            EnemyPanel?.Init();
            simpleInfoItemPanel?.Init();
            settingPanel?.Init();
        }

    }
}
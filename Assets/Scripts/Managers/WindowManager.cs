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
        public EnemyPanel EnemyPanel;
        public ConfirmPanel confirmPanel;
        

        public SkillInfoPanel skillInfoPanel;
        public SimpleInfoItemPanel simpleInfoItemPanel;

        public Transform dragViewParent;
        
        protected override void Awake()
        {
            base.Awake();
            confirmPanel?.gameObject.SetActive(false);
            if (dragViewParent != null)
            {
                dragViewParent.transform.SetAsLastSibling();
            }

            CrystalPanel?.Init();
            ShopPanel?.Init();
            EnemyPanel?.Init();
            simpleInfoItemPanel?.Init();
        }

        public void Display(EnemySaveData data)
        {
            EnemyPanel.Load(data);
        }
    }
}
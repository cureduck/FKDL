﻿using System;
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
        
        public Localize WarnWindow;

        public void Display(EnemySaveData data)
        {
            EnemyPanel.Load(data);
        }
        
        
        public void Warn(string log)
        {
            WarnWindow.gameObject.SetActive(true);
            WarnWindow.SetTerm(log);
            StartCoroutine(WarnDisappear());
        }

        private IEnumerator WarnDisappear()
        {
            yield return new WaitForSeconds(3f);
            WarnWindow.gameObject.SetActive(false);
        }
    }
}
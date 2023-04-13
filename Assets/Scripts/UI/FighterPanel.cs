﻿using System;
using Game;
using Managers;
using Sirenix.OdinInspector;
using UnityEngine;

namespace UI
{
    public abstract class FighterPanel<T> : Singleton<T> where T : Singleton<T>
    {
        [ShowInInspector]
        public FighterData Master
        {
            get => _master;
            set => SetMaster(value);
        }

        private FighterData _master;

        public StatusPanel StatusPanel;
        //public BuffPanel BuffPanel;
        public BuffListView buffListView;

        public void Init() 
        {
            buffListView.Init();
        }

        protected virtual void SetMaster(FighterData master)
        {
            //Debug.Log("set Master " + master.ToString());
            if (master == _master) return; 
            
            //Debug.Log(master == _master);
            
            _master = master;
            
            StatusPanel.SetMaster(master);
            buffListView.SetData(master.Buffs);

            //BuffPanel.gameObject.SetActive(false);
            //BuffPanel.SetMaster(master);
        }
    }
}
﻿using System;
 using Game;
 using I2.Loc;
 using TMPro;
 using UnityEngine;

namespace UI
{
    public class EnemyPanel : FighterPanel<EnemyPanel>
    {

        public EnemySkillInfoView skillView;

        public Localize RewardInfo;
        
        private void Start()
        {
            GetComponent<UnityEngine.Canvas>().worldCamera = Camera.main;
            //gameObject.SetActive(false);
        }

        public void Load(FighterData master)
        {
            Master = master;
        }

        protected override void SetMaster(FighterData master)
        {
            if (Master != null) 
            {
                Master.OnUpdated -= UpdateView;
            }
            base.SetMaster(master);
            if (Master != null)
            {
                Master.OnUpdated += UpdateView;
            }
            //获得主技能
            if (master.Skills.Count <= 0 || master.Skills[0] == null)
            {
                skillView.gameObject.SetActive(false);
            }
            else 
            {
                skillView.SetData(master, master.Skills[0]);
                skillView.gameObject.SetActive(true);
            }
        }


        private void UpdateView() 
        {
            buffListView.SetData(Master.Buffs);
        }

        private void Update()
        {

            RewardInfo.SetTerm("RewardInfo");
            var t = RewardInfo.GetComponent<TMP_Text>();
            t.text = t.text.Replace("{Gold}", Master.Status.Gold.ToString());
        }
    }
}
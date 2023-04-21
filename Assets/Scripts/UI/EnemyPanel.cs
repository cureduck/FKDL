﻿using System;
 using Game;
 using I2.Loc;
 using TMPro;
 using UnityEngine;

namespace UI
{
    public class EnemyPanel : FighterPanel<EnemyPanel>
    {
        [SerializeField]
        private Localize skillName;
        [SerializeField]
        private Localize skillDescribe;
        [SerializeField] private LocalizationParamsManager DescParamsManager;
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

            UpdateView();
        }


        private void UpdateView() 
        {


            buffListView.SetData(Master.Buffs);

            RewardInfo.SetTerm("RewardInfo");
            var t = RewardInfo.GetComponent<TMP_Text>();
            t.text = t.text.Replace("{Gold}", Master.Status.Gold.ToString());

            //获得主技能
            if (Master.Skills.Count <= 0 || Master.Skills[0] == null)
            {
                skillView.gameObject.SetActive(false);
            }
            else
            {
                SkillData skillData = Master.Skills[0];
                skillName.SetTerm(skillData.Id);
                skillDescribe.SetTerm($"{skillData.Id}_desc");
                Skill curSkill = skillData.Bp;
                DescParamsManager.SetParameterValue("P1", curSkill.Param1.ToString());
                DescParamsManager.SetParameterValue("P2", curSkill.Param2.ToString());
                skillView.SetData(Master, skillData);
                skillView.gameObject.SetActive(true);
            }



        }

        private void Update()
        {

            //RewardInfo.SetTerm("RewardInfo");
            //var t = RewardInfo.GetComponent<TMP_Text>();
            //t.text = t.text.Replace("{Gold}", Master.Status.Gold.ToString());
        }
    }
}
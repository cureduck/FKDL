using System;
using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;
using Game;
using I2.Loc;
using TMPro;

public class SkillInfoPanel : BasePanel<SkillInfoPanel.Args>
{
    public class Args
    {
        public Vector2 screenPosition;
        public SkillData skillData;
    }

    public class Args02 : Args
    {
        public Skill skill;
    }

    [SerializeField]
    private Localize skillName;
    [SerializeField]
    private Localize costInfo;
    [SerializeField] private LocalizationParamsManager CostParamsManager;
    [SerializeField]
    private Localize colddownInfo;
    [SerializeField] private LocalizationParamsManager ColdDownParamsManager;
    [SerializeField]
    private Localize describe;
    [SerializeField] private LocalizationParamsManager DescParamsManager;
    [SerializeField]
    private Localize maxLevel;
    [SerializeField] private LocalizationParamsManager MaxParamsManager;
    [SerializeField]
    private Localize curBelongProf;
    [SerializeField] private LocalizationParamsManager CurBelongProfManager;

    [SerializeField]
    private Localize positiveInfo;


    private void Start()
    {
        DescParamsManager = describe.GetComponent<LocalizationParamsManager>();
    }


    protected override void UpdateUI()
    {
        //if (DescParamsManager == null) 
        //{
        //    DescParamsManager = 
        //}

        transform.position = Data.screenPosition;

        if (Data is Args02)
        {
            //Skill curSkilInfo = (Data as Args02).skill;

            //skillName.SetTerm(curSkilInfo.Id);
            //if (curSkilInfo.Positive)
            //{
            //    costInfo.text = $"{curSkilInfo.CostInfo.CostType}:{curSkilInfo.CostInfo.Value}";
            //}
            //else
            //{
            //    costInfo.text = $"无消耗";
            //}

            //colddownInfo.text = $"CoolDown:{curSkilInfo.Cooldown}";

            //describe.SetTerm($"{curSkilInfo.Id}_desc");

            //maxLevel.text = $"最大等级{curSkilInfo.MaxLv}";
            //positiveInfo.SetTerm(curSkilInfo.Positive ? "positive" : "passive");
        }
        else 
        {
            Skill curSkilInfo = Data.skillData.Bp;

            skillName.SetTerm(curSkilInfo.Id);
            if (curSkilInfo.Positive)
            {
                if (curSkilInfo.CostInfo.CostType == CostType.Hp)
                {
                    costInfo.SetTerm("HPCostInfo");
                }
                else if (curSkilInfo.CostInfo.CostType == CostType.Gold) 
                {
                    costInfo.SetTerm("GoldCostInfo");
                }
                else
                {
                    costInfo.SetTerm("MPCostInfo");
                }
                CostParamsManager.SetParameterValue("VALUE", curSkilInfo.CostInfo.Value.ToString());
            }
            else
            {
                costInfo.SetTerm("NoCost");
                //costInfo.text = $"无消耗";
            }

            describe.SetTerm($"{curSkilInfo.Id}_desc");
            DescParamsManager.SetParameterValue("P1", curSkilInfo.Param1.ToString());
            DescParamsManager.SetParameterValue("P2", curSkilInfo.Param2.ToString());
            colddownInfo.SetTerm("CooldownInfo");
            ColdDownParamsManager.SetParameterValue("VALUE", curSkilInfo.Cooldown.ToString());

            maxLevel.SetTerm("MaxLvInfo");
            MaxParamsManager.SetParameterValue("MaxLv", curSkilInfo.MaxLv.ToString());
            MaxParamsManager.SetParameterValue("CurLv", Data.skillData.CurLv.ToString());

            curBelongProf.SetTerm("SkillBelong");
            CurBelongProfManager.SetParameterValue("VALUE", curSkilInfo.Pool);


            positiveInfo.SetTerm(curSkilInfo.Positive ? "positive" : "passive");
        }
    }


}

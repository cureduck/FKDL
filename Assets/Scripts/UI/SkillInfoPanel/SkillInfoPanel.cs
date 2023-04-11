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
    private TMP_Text costInfo;
    [SerializeField]
    private TMP_Text colddownInfo;
    [SerializeField]
    private Localize describe;
    [SerializeField]
    private Localize maxLevel;
    [SerializeField]
    private Localize positiveInfo;

    [SerializeField] private LocalizationParamsManager DescParamsManager;
    [SerializeField] private LocalizationParamsManager MaxParamsManager;
    
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
                costInfo.text = $"{curSkilInfo.CostInfo.CostType}:{curSkilInfo.CostInfo.ActualValue}";
            }
            else
            {
                costInfo.text = $"无消耗";
            }

            describe.SetTerm($"{curSkilInfo.Id}_desc");
            //DescParamsManager.SetParameterValue("P1", curSkilInfo.Param1.ToString());
            //DescParamsManager.SetParameterValue("P2", curSkilInfo.Param2.ToString());
            colddownInfo.text = $"CoolDown:{curSkilInfo.Cooldown}";

            maxLevel.SetTerm("MaxLvInfo");
            MaxParamsManager.SetParameterValue("MaxLv", curSkilInfo.MaxLv.ToString());
            MaxParamsManager.SetParameterValue("CurLv", Data.skillData.CurLv.ToString());

        
            positiveInfo.SetTerm(curSkilInfo.Positive ? "positive" : "passive");
        }
    }


}

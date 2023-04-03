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
    [SerializeField]
    private Localize skillName;
    [SerializeField]
    private TMP_Text costInfo;
    [SerializeField]
    private TMP_Text colddownInfo;
    [SerializeField]
    private Localize describe;
    [SerializeField]
    private TMP_Text maxLevel;
    [SerializeField]
    private Localize positiveInfo;

    protected override void UpdateUI()
    {
        transform.position = Data.screenPosition;
        Skill curSkilInfo = Data.skillData.Bp;

        skillName.SetTerm(curSkilInfo.Id);
        if (curSkilInfo.Positive)
        {
            costInfo.text = $"{curSkilInfo.CostInfo.CostType}:{curSkilInfo.CostInfo.Value}";
        }
        else 
        {
            costInfo.text = $"无消耗";
        }

        colddownInfo.text = $"CoolDown:{curSkilInfo.Cooldown}";

        describe.SetTerm($"{curSkilInfo.Id}_desc");

        maxLevel.text = $"最大等级{curSkilInfo.MaxLv} 当前等级<color=yellow>{Data.skillData.CurLv}</color>";
        positiveInfo.SetTerm(curSkilInfo.Positive ? "positive" : "passive");




    }


}

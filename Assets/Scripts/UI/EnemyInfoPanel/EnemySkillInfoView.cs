using I2.Loc;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Game;
using Managers;

public class EnemySkillInfoView : MonoBehaviour
{
    [SerializeField]
    private CellSkillView skillView;
    [SerializeField]
    private Localize skillName;
    [SerializeField]
    private Localize skillDescribe;

    private SkillData skillData;

    public void SetData(FighterData fighterData,SkillData skillData) 
    {
        this.skillData = skillData;

        skillView.SetData(fighterData, skillData,0,null,null,null);
        Skill curSkill = SkillManager.Instance.GetSkillByStringID(skillData.Id);
        if (curSkill != null)
        {
            skillName.SetTerm(curSkill.Pool);
        }
        else 
        {
            skillName.SetTerm("Unknow");
            skillDescribe.SetTerm("Unknow");
        }




    }
}

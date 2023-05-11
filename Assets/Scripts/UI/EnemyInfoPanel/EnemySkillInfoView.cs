using Game;
using I2.Loc;
using Managers;
using UnityEngine;

public class EnemySkillInfoView : MonoBehaviour
{
    [SerializeField] private CellSkillView skillView;
    [SerializeField] private Localize skillName;
    [SerializeField] private Localize skillDescribe;

    private SkillData skillData;

    public void SetData(FighterData fighterData, SkillData skillData)
    {
        this.skillData = skillData;

        skillView.SetData(fighterData, skillData, 0, null, null, null);
        Skill curSkill = SkillManager.Instance.GetById(skillData.Id);
        if (curSkill != null)
        {
            skillName.SetTerm(curSkill.Prof);
        }
        else
        {
            skillName.SetTerm("Unknown");
            skillDescribe.SetTerm("Unknown");
        }
    }
}
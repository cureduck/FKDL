using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Managers;
using Game;
using I2.Loc;
using TMPro;

public class CellSkillView : MonoBehaviour
{
    private static CellSkillView curSelectSkill;
    [SerializeField]
    private Transform mainView;
    [SerializeField]
    private GameObject heightlightView;
    [SerializeField]
    private GameObject haveSkillGroup;
    [SerializeField]
    private GameObject emptySkillGroup;
    [SerializeField]
    private Image icon;
    [SerializeField]
    private TextMeshProUGUI levelInfo;
    [SerializeField]
    private Localize skillName;
    [SerializeField]
    private GameObject coldDown;
    [SerializeField]
    private TextMeshProUGUI coldDown_txt;
    [SerializeField]
    private Image coldDown_mask;
    [SerializeField]
    private Button main_btn;

    private SkillData skillData;

    [SerializeField]
    public float targetPrecent;

    void Start()
    {
        main_btn.onClick.AddListener(SelectCurSkill);
        heightlightView.SetActive(false);
    }

    public void SetData(SkillData skillData) 
    {
        if (this.skillData != null)
        {
            this.skillData.onValueChange -= SetData;
        }

        if (skillData == null)
        {
            haveSkillGroup.gameObject.SetActive(false);
            emptySkillGroup.gameObject.SetActive(true);
            return;
        }

        Skill curSkill = SkillManager.Instance.GetSkillByStringID(skillData.Id);
        Debug.Log(curSkill);

        if (curSkill == null)
        {
            haveSkillGroup.gameObject.SetActive(false);
            emptySkillGroup.gameObject.SetActive(true);
        }
        else 
        {
            if (curSkill.Icon != null) 
            {
                icon.sprite = curSkill.Icon;
            }
            if (skillData.CurLv >= curSkill.MaxLv)
            {
                levelInfo.text = $"<color=yellow>{skillData.CurLv}/{curSkill.MaxLv}</color>";
            }
            else 
            {
                levelInfo.text = $"{skillData.CurLv}/{curSkill.MaxLv}";
            }

            skillName.SetTerm(curSkill.Pool);


            if (skillData.Cooldown > 0)
            {
                coldDown_txt.text = skillData.Cooldown.ToString();
                if (curSkill.Cooldown <= 0)
                {
                    targetPrecent = 1;
                }
                else 
                {
                    targetPrecent = skillData.Cooldown/ (float)curSkill.Cooldown;
                }

                coldDown.gameObject.SetActive(true);
            }
            else 
            {
                coldDown.gameObject.SetActive(false);
            }
            this.skillData = skillData;
            skillData.onValueChange += SetData;
        }


    }

    public void SelectCurSkill() 
    {
        Debug.Log(curSelectSkill == this);
        if (curSelectSkill == this)
        {
            curSelectSkill.heightlightView.SetActive(false);
            curSelectSkill = null;
            return;
        }
        if (curSelectSkill != null)
        {
            curSelectSkill.heightlightView.SetActive(false);
        }
        curSelectSkill = this;
        curSelectSkill.heightlightView.SetActive(true);
    }

    private void Update()
    {
        if (skillData == null) return;
        coldDown_mask.fillAmount = Mathf.Lerp(coldDown_mask.fillAmount, targetPrecent,  10 * Time.deltaTime);
    }


    private void OnDisable()
    {
        if (skillData != null)
        {
            skillData.onValueChange -= SetData;
        }
    }

}

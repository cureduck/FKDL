using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Managers;
using Game;
using I2.Loc;
using TMPro;
using Sirenix.OdinInspector;

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
    private PlayerData playerData;
    public string value;
    [SerializeField]
    public float targetPrecent;

    void Start()
    {
        main_btn.onClick.AddListener(SelectCurSkill);
        heightlightView.SetActive(false);
    }
    [Button]
    public void UpdateView() 
    {
        SetData(playerData, skillData);
    }

    public void SetData(PlayerData playerData,SkillData skillData) 
    {
        this.skillData = skillData;
        this.playerData = playerData;
        //Debug.Log(skillData);
        if (skillData == null)
        {
            haveSkillGroup.gameObject.SetActive(false);
            emptySkillGroup.gameObject.SetActive(true);
            return;
        }
        //Debug.Log(skillData.Id);
        Skill curSkill = SkillManager.Instance.GetSkillByStringID(skillData.Id);
        //

        if (curSkill == null)
        {
            haveSkillGroup.gameObject.SetActive(false);
            emptySkillGroup.gameObject.SetActive(true);
            Debug.LogWarning("Empty");
        }
        else 
        {
            haveSkillGroup.gameObject.SetActive(true);
            emptySkillGroup.gameObject.SetActive(false);
            value = curSkill.Pool;
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

            //skillData.onValueChange += SetData;
        }


    }

    public void SelectCurSkill() 
    {
        Skill curSkill = skillData.Bp;
        Debug.Log(skillData.Cooldown);
        if (skillData.Cooldown > 0) return;
        if (!curSkill.Positive) return;

        //if(curSelectSkill.skillData.Bp.)

        Debug.Log(curSelectSkill == this);
        if (curSelectSkill == this)
        {
            curSelectSkill.heightlightView.SetActive(false);
            curSelectSkill = null;
            //return;
        }
        if (!curSkill.BattleOnly)
        {
            playerData.CastNonAimingSkill(skillData);
        }
        else 
        {
            if (curSelectSkill != null)
            {
                curSelectSkill.heightlightView.SetActive(false);
            }
            curSelectSkill = this;
            curSelectSkill.heightlightView.SetActive(true);
        }

    }

    private void Update()
    {
        if (skillData == null) return;
        coldDown_mask.fillAmount = Mathf.Lerp(coldDown_mask.fillAmount, targetPrecent,  10 * Time.deltaTime);
    }



}

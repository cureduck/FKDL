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
    private GameObject passtiveBG;
    [SerializeField]
    private GameObject unPasstiveBG;
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
    private GameObject coolDownCompleteSign;
    [SerializeField]
    private Button main_btn;

    private SkillData skillData;
    private PlayerData playerData;
    public string value;

    public bool canDebug = false;

    [SerializeField]
    private float targetPrecent;
    [SerializeField]
    private float lastPrecent;

    void Start()
    {
        main_btn.onClick.AddListener(SelectCurSkill);
        heightlightView.SetActive(false);
        coolDownCompleteSign.gameObject.SetActive(false);
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
        }
        else 
        {
            haveSkillGroup.gameObject.SetActive(true);
            emptySkillGroup.gameObject.SetActive(false);
            value = curSkill.Pool;

            passtiveBG.SetActive(curSkill.Positive);
            unPasstiveBG.SetActive(!curSkill.Positive);

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
                    targetPrecent = skillData.Cooldown / (float)curSkill.Cooldown;
                }

                coldDown_txt.gameObject.SetActive(true);
            }
            else 
            {
                coldDown_txt.gameObject.SetActive(false);
                targetPrecent = 0;
            }

            //skillData.onValueChange += SetData;
        }


    }

    public void SelectCurSkill() 
    {
        Skill curSkill = skillData.Bp;
        //Debug.Log(skillData.Cooldown);
        if (skillData.Cooldown > 0) return;
        if (!curSkill.Positive) return;

        //if(curSelectSkill.skillData.Bp.)

        //Debug.Log(curSelectSkill == this);
        if (curSelectSkill == this)
        {
            curSelectSkill.heightlightView.SetActive(false);
            curSelectSkill = null;
            //return;
        }
        //Debug.Log(curSkill.BattleOnly);
        if (!curSkill.BattleOnly)
        {
            coldDown_mask.fillAmount = 1;
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
        //if (canDebug) 
        //{
        //    Debug.Log(targetPrecent);
        //}

        //coldDown_mask.fillAmount = targetPrecent;
        //return;




        lastPrecent = coldDown_mask.fillAmount;
        if (coldDown_mask.fillAmount < targetPrecent)
        {
            coldDown_mask.fillAmount = targetPrecent;
        }
        else 
        {
            coldDown_mask.fillAmount -= Time.deltaTime * 0.75f;
            if (coldDown_mask.fillAmount <= 0 && lastPrecent > 0) 
            {
                coolDownCompleteSign.gameObject.SetActive(false);
                coolDownCompleteSign.gameObject.SetActive(true);
                //Debug.Log("冷却完毕");
            }
        }
    }
}

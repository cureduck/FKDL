using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Managers;
using Game;
using I2.Loc;
using TMPro;
using Sirenix.OdinInspector;
using Unity.Mathematics;

public class CellSkillView : MonoBehaviour
{
    private static CellSkillView curSelectSkill;
    [SerializeField]
    private Transform mainView;
    [Header("详细显示")]
    [SerializeField]
    private Transform showDetailPoint;
    [SerializeField]
    private PointEnterAndExit pointEvent;
    [Header("其他组件")]
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
    private FighterData playerData;

    [SerializeField]
    private float targetPrecent;
    [SerializeField]
    private float lastPrecent;

    public bool canInteractive = true;

    void Start()
    {
        main_btn.onClick.AddListener(SelectCurSkill);
        pointEvent.onPointEnter.AddListener(OnPointEnter);
        pointEvent.onPointExit.AddListener(OnPointExit);


        heightlightView.SetActive(false);
        coolDownCompleteSign.gameObject.SetActive(false);
    }

    [Button]
    public void UpdateView() 
    {
        SetData(playerData, skillData);
    }

    public void SetData(FighterData playerData,SkillData skillData) 
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

            skillName.SetTerm(curSkill.Id);


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
                                    
                var dt = Time.time - t0;
                t0 = Time.time;
                if (dt > 0) SetNextRoundSpeed(dt);

                coldDown_txt.gameObject.SetActive(false);
                targetPrecent = 0;
            }

            //skillData.onValueChange += SetData;
        }


    }

    public void SelectCurSkill() 
    {
        if (!canInteractive) return;

        Skill curSkill = skillData.Bp;


        /*//Debug.Log(skillData.Cooldown);
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
        */
        if (!curSkill.Positive)
        {
            UnpositiveSkillTrigger();

        }
        else 
        {
            if (curSelectSkill == this)
            {
                curSelectSkill.heightlightView.SetActive(false);
                curSelectSkill = null;
                //return;
            }

            if (playerData.CanCast(skillData))
            {
                coldDown_mask.fillAmount = 1;
                playerData.UseSkill(skillData);
            }
        }


    }

    private void UnpositiveSkillTrigger() 
    {
        GameObject cur = ObjectPoolManager.Instance.SpawnUnPasstiveSkillSignEffect(icon.sprite);
        cur.transform.SetParent(transform);
        cur.transform.localScale = Vector3.one;
        cur.transform.localPosition = Vector3.zero;
    }
    #region 鼠标交互
    private void OnPointExit()
    {
        WindowManager.Instance.skillInfoPanel.Close();
        
    }

    private void OnPointEnter()
    {
        WindowManager.Instance.skillInfoPanel.Open(new SkillInfoPanel.Args { screenPosition = showDetailPoint.position, skillData = skillData });
    }
    #endregion



    public float Speed;

    private float t0;
    private void Update()
    {
        if (skillData == null || skillData.IsEmpty) return;
        //if (canDebug) 
        //{
        //    Debug.Log(targetPrecent);
        //}

        //coldDown_mask.fillAmount = targetPrecent;
        //return;




        lastPrecent = coldDown_mask.fillAmount;
        if (coldDown_mask.fillAmount <= targetPrecent)
        {
            coldDown_mask.fillAmount = targetPrecent;
        }
        else
        {
            coldDown_mask.fillAmount -= Time.deltaTime * Speed;
            if (coldDown_mask.fillAmount <= 0 && lastPrecent > 0) 
            {
                coolDownCompleteSign.gameObject.SetActive(false);
                coolDownCompleteSign.gameObject.SetActive(true);
                //Debug.Log("冷却完毕");
            }
        }
    }

    private const float SpeedParam = 1.1f;
    
    private void SetNextRoundSpeed(float dt)
    {
        Speed = math.max(0.8f,  math.min( Speed * 1.2f, SpeedParam / dt));
    }

    public override string ToString()
    {
        return $"Cell Skill Button {transform.GetSiblingIndex().ToString()}";
    }
}

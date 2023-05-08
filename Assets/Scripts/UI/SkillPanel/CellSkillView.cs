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
    [SerializeField] private Transform mainView;
    [Header("详细显示")] [SerializeField] private Transform showDetailPoint;
    [SerializeField] private PointEnterAndExit pointEvent;
    [SerializeField] private ObjectColliderPointEnterAndExit objectPointEnterAndExit;
    [SerializeField] private bool isWorldObject = false;
    [Header("拖拽与交互组件")] [SerializeField] private CellUIDragView cellUIDragView;
    [SerializeField] private CellSkillViewDragReceive dragReceive;

    public int Index
    {
        get { return dragReceive.index; }
    }

    [Header("消耗颜色")] [SerializeField] private TMP_Text cost_txt;
    [SerializeField] private Color magicCost_color;
    [SerializeField] private Color healthCost_color;
    [SerializeField] private Color goldCost_color;
    [SerializeField] private Color notEnough_Color;
    [Header("技能升级按钮")] [SerializeField] private GameObject levelUpObject;
    [SerializeField] private Button levelUp_btn;
    [Header("其他组件")] [SerializeField] private GameObject heightlightView;
    [SerializeField] private GameObject haveSkillGroup;
    [SerializeField] private GameObject emptySkillGroup;
    [SerializeField] private GameObject passtiveBG;
    [SerializeField] private GameObject unPasstiveBG;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI levelInfo;
    [SerializeField] private Localize skillName;
    [SerializeField] private TextMeshProUGUI coldDown_txt;
    [SerializeField] private Image coldDown_mask;
    [SerializeField] private GameObject coolDownCompleteSign;

    private SkillData skillData;

    public SkillData Data
    {
        get { return skillData; }
    }

    private FighterData playerData;

    [SerializeField] private float targetPrecent;
    [SerializeField] private float lastPrecent;

    public bool canInteractive = true;

    void Start()
    {
        cellUIDragView.onLeftClick.AddListener(SelectCurSkill);
        if (pointEvent)
        {
            pointEvent.onPointEnter.AddListener(OnPointEnter);
            pointEvent.onPointExit.AddListener(OnPointExit);
        }

        if (objectPointEnterAndExit)
        {
            objectPointEnterAndExit.onPointEnter.AddListener(OnPointEnter);
            objectPointEnterAndExit.onPointExit.AddListener(OnPointExit);
        }


        levelUp_btn.onClick.AddListener(UpgradeSkill);

        heightlightView.SetActive(false);
        coolDownCompleteSign.gameObject.SetActive(false);
        cellUIDragView.Init(this);
        cellUIDragView.dragParent = WindowManager.Instance.dragViewParent;
    }

    [Button]
    public void UpdateView()
    {
        SetData(playerData, skillData, dragReceive.index, this.cellUIDragView.beginDrag, this.cellUIDragView.endDrag,
            dragReceive.onEndDrag);
    }

    public void SetData(FighterData playerData, SkillData skillData, int index, System.Action<object> onStartDrag,
        System.Action<object> onEndDrag, System.Action<CellSkillView, CellSkillViewDragReceive> onEndDragComplete)
    {
        if (this.skillData != null)
        {
            this.skillData.Activated -= UnpositiveSkillTrigger;
        }

        this.skillData = skillData;
        if (this.skillData != null)
        {
            this.skillData.Activated += UnpositiveSkillTrigger;
        }

        this.playerData = playerData;
        this.cellUIDragView.beginDrag = onStartDrag;
        this.cellUIDragView.endDrag = onEndDrag;

        this.dragReceive.index = index;
        this.dragReceive.onEndDrag = onEndDragComplete;
        //Debug.Log(skillData);
        if (skillData == null)
        {
            haveSkillGroup.gameObject.SetActive(false);
            emptySkillGroup.gameObject.SetActive(true);
            return;
        }

        //Debug.Log(skillData.Id);
        Skill curSkill = SkillManager.Instance.GetById(skillData.Id);
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

            //被动技能且基础冷却为0不会触发特效
            if (!Data.Bp.Positive && Data.Bp.Cooldown <= 0)
            {
                coldDown_mask.fillAmount = 0;
            }

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
            //消耗显示
            cost_txt.text = skillData.Bp.Positive ? skillData.Bp.CostInfo.Value.ToString() : string.Empty;
            if (skillData.Bp.CostInfo.CostType == CostType.Gold)
            {
                cost_txt.color = goldCost_color;
            }
            else if (skillData.Bp.CostInfo.CostType == CostType.Hp)
            {
                cost_txt.color = healthCost_color;
            }
            else
            {
                cost_txt.color = magicCost_color;
            }

            //冷却显示
            if (skillData.CooldownLeft > 0)
            {
                coldDown_txt.text = skillData.CooldownLeft.ToString();
                if (curSkill.Cooldown <= 0)
                {
                    targetPrecent = 1;
                }
                else
                {
                    targetPrecent = skillData.CooldownLeft / (float)skillData.InitCoolDown;
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
            //levelUpObject

            PlayerData player = playerData as PlayerData;
            if (player != null)
            {
                if (skillData.CurLv >= curSkill.MaxLv)
                {
                    levelUpObject.SetActive(false);
                }
                else
                {
                    levelUpObject.SetActive(player.skillPoint[curSkill.Rank] > 0);
                }
            }
            else
            {
                levelUpObject.SetActive(false);
            }

            //skillData.onValueChange += SetData;
        }
    }

    private void SelectCurSkill()
    {
        if (!canInteractive) return;

        Skill curSkill = skillData.Bp;

        //if (skillData.CooldownLeft > 0) return;
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
            //UnpositiveSkillTrigger();
        }
        else
        {
            if (curSelectSkill == this)
            {
                curSelectSkill.heightlightView.SetActive(false);
                curSelectSkill = null;
                //return;
            }

            Info cantCostInfo;
            if (playerData.CanCast(skillData, out cantCostInfo))
            {
                coldDown_mask.fillAmount = 1;
                playerData.UseSkill(skillData);
            }
            else
            {
                WindowManager.Instance.warningInfoPanel.Open(cantCostInfo != null ? cantCostInfo.ToString() : "未知警告信息");
            }
        }
    }

    private void UpgradeSkill()
    {
        PlayerData temp = playerData as PlayerData;
        if (temp != null)
        {
            temp.UpgradeWithPoint(skillData);
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
        //WindowManager.Instance.FightPredictPanel.Close();
        if (GameManager.Instance.InBattle && Data.Bp.BattleOnly)
        {
            //var arena = Arena.ArrangeFight(GameManager.Instance.PlayerData, (EnemySaveData)GameManager.Instance.Focus.Data, Data);
            WindowManager.Instance.EnemyPanel.SetPlayerUseSkill(null);
        }
    }

    private void OnPointEnter()
    {
        if (isWorldObject)
        {
            Debug.Log(Camera.main.WorldToScreenPoint(transform.position));
            WindowManager.Instance.skillInfoPanel.Open(new SkillInfoPanel.Args
                { worldTrans = transform, skillData = skillData });
        }
        else
        {
            WindowManager.Instance.skillInfoPanel.Open(new SkillInfoPanel.Args
                { screenPosition = showDetailPoint.position, skillData = skillData });
        }

        //被动技能不会显示对应的效果
        if (!Data.Bp.Positive)
        {
            return;
        }

        if (GameManager.Instance.InBattle && Data.Bp.BattleOnly)
        {
            //var arena = Arena.ArrangeFight(GameManager.Instance.PlayerData, (EnemySaveData)GameManager.Instance.Focus.Data, Data);
            WindowManager.Instance.EnemyPanel.SetPlayerUseSkill(Data);
            Debug.Log(Data);
        }
    }

    #endregion


    public float Speed;

    private float t0;

    private void Update()
    {
        if (skillData == null || skillData.IsEmpty) return;
        //Debug.Log(skillData.IsEmpty);
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
        Speed = math.max(0.8f, math.min(Speed * 1.2f, SpeedParam / dt));
    }

    public override string ToString()
    {
        return $"Cell Skill Button {transform.GetSiblingIndex().ToString()}";
    }
}
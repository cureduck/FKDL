using Game;
using I2.Loc;
using Managers;
using TMPro;
using Tools;
using UI;
using UnityEngine;
using UnityEngine.Serialization;

public class SkillInfoPanel : BasePanel<SkillInfoPanel.Args>
{
    [SerializeField] private Localize skillName;
    [SerializeField] private Localize skillRank;
    [SerializeField] private TMP_Text skillRank_tmp;
    [SerializeField] private Localize costInfo;
    [SerializeField] private LocalizationParamsManager CostParamsManager;

    [FormerlySerializedAs("colddownInfo")] [SerializeField]
    private Localize coolDownInfo;

    [SerializeField] private LocalizationParamsManager ColdDownParamsManager;
    [SerializeField] private Localize describe;
    [SerializeField] private LocalizationParamsManager DescParamsManager;
    [SerializeField] private Localize maxLevel;
    [SerializeField] private LocalizationParamsManager MaxParamsManager;
    [SerializeField] private Localize curBelongProf;
    [SerializeField] private LocalizationParamsManager CurBelongProfManager;

    [SerializeField] private Localize positiveInfo;

    [Header("关键词")] [SerializeField] private NotBeyoundTheScreen notBeyoundTheScreen;

    [SerializeField] private GameObject m_keyWordObj;
    [SerializeField] private Localize m_keyWordDescribe_txt;
    [SerializeField] private LocalizationParamsManager _keyWordParamsManager;
    private Camera mainCamera;
    private RectTransform rectTransform;


    private DisplayMode displayMode => InputSystem.Instance.DisplayMode;

    private void Start()
    {
        mainCamera = Camera.main;
        DescParamsManager = describe.GetComponent<LocalizationParamsManager>();
        notBeyoundTheScreen.Init(transform.parent.GetComponent<Canvas>());
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetKeyUp(KeyCode.LeftAlt))
        {
            UpdateUI();
        }

        if (Data.worldTrans != null)
        {
            transform.position = mainCamera.WorldToScreenPoint(Data.worldTrans.transform.position);
        }
    }

    protected override void UpdateUI()
    {
        //if (DescParamsManager == null) 
        //{
        //    DescParamsManager = 
        //}

        transform.position = Data.screenPosition;
        Skill curSkillInfo;
        if (Data is Args02)
        {
            curSkillInfo = (Data as Args02).skill;

            skillName.SetTerm(curSkillInfo.Id);

            if (curSkillInfo.Rank == Rank.Normal)
            {
                skillRank.SetTerm("UI_Normal_RankInfo_01");
                skillRank_tmp.color = new Color(1, 1, 1);
            }
            else if (curSkillInfo.Rank == Rank.Uncommon)
            {
                skillRank.SetTerm("UI_Normal_RankInfo_02");
                skillRank_tmp.color = new Color(36 / 255.0f, 176 / 255.0f, 143 / 255.0f);
            }
            else
            {
                skillRank.SetTerm("UI_Normal_RankInfo_03");
                skillRank_tmp.color = new Color(255 / 255.0f, 163 / 255.0f, 0 / 255.0f);
            }

            if (curSkillInfo.Positive)
            {
                if (curSkillInfo.CostInfo.CostType == CostType.Hp)
                {
                    costInfo.SetTerm("HPCostInfo");
                }
                else if (curSkillInfo.CostInfo.CostType == CostType.Gold)
                {
                    costInfo.SetTerm("GoldCostInfo");
                }
                else
                {
                    costInfo.SetTerm("MPCostInfo");
                }

                CostParamsManager.SetParameterValue("VALUE", curSkillInfo.CostInfo.Value.ToString());
            }
            else
            {
                costInfo.SetTerm("NoCost");
                //costInfo.text = $"无消耗";
            }

            coolDownInfo.SetTerm("CooldownInfo");
            ColdDownParamsManager.SetParameterValue("VALUE", curSkillInfo.Cooldown.ToString());

            //describe.SetTerm($"{curSkilInfo.Id}_desc");
            describe.SetTerm($"{curSkillInfo.Id}_desc");
            describe.SetLocalizeParam(
                new[] { "P1", "P2", "CurLv" },
                new[]
                {
                    curSkillInfo.Param1.ToString(), curSkillInfo.Param2.ToString(), 1.ToString()
                });
            switch (displayMode)
            {
                case DisplayMode.Simple:
                    describe.Calculate();
                    var s = describe.GetComponent<TMP_Text>().text;
                    describe.RemoveBetween((@"\(", @"\)"));
                    describe.RemoveBetween(('（', '）'));
                    break;
                case DisplayMode.Detail:
                    describe.RemoveBetween();
                    break;
            }

            maxLevel.SetTerm("MaxLvInfo");
            MaxParamsManager.SetParameterValue("MaxLv", curSkillInfo.MaxLv.ToString());
            MaxParamsManager.SetParameterValue("CurLv", "0");

            curBelongProf.SetTerm("SkillBelong");
            CurBelongProfManager.SetParameterValue("VALUE", curSkillInfo.Prof);

            positiveInfo.SetTerm(curSkillInfo.Positive ? "positive" : "passive");
        }
        else
        {
            curSkillInfo = Data.skillData.Bp;

            skillName.SetTerm(curSkillInfo.Id);
            if (curSkillInfo.Rank == Rank.Normal)
            {
                skillRank.SetTerm("UI_Normal_RankInfo_01");
                skillRank_tmp.color = new Color(1, 1, 1);
            }
            else if (curSkillInfo.Rank == Rank.Uncommon)
            {
                skillRank.SetTerm("UI_Normal_RankInfo_02");
                skillRank_tmp.color = new Color(36 / 255.0f, 176 / 255.0f, 143 / 255.0f);
            }
            else
            {
                skillRank.SetTerm("UI_Normal_RankInfo_03");
                skillRank_tmp.color = new Color(255 / 255.0f, 163 / 255.0f, 0 / 255.0f);
            }

            if (curSkillInfo.Positive)
            {
                if (curSkillInfo.CostInfo.CostType == CostType.Hp)
                {
                    costInfo.SetTerm("HPCostInfo");
                }
                else if (curSkillInfo.CostInfo.CostType == CostType.Gold)
                {
                    costInfo.SetTerm("GoldCostInfo");
                }
                else
                {
                    costInfo.SetTerm("MPCostInfo");
                }

                CostParamsManager.SetParameterValue("VALUE", curSkillInfo.CostInfo.Value.ToString());
            }
            else
            {
                costInfo.SetTerm("NoCost");
                //costInfo.text = $"无消耗";
            }

            describe.SetTerm($"{curSkillInfo.Id}_desc");
            describe.SetLocalizeParam(
                new[] { "P1", "P2", "CurLv" },
                new[]
                {
                    curSkillInfo.Param1.ToString(), curSkillInfo.Param2.ToString(), Data.skillData.CurLv.ToString()
                });
            switch (displayMode)
            {
                case DisplayMode.Simple:
                    describe.Calculate();
                    var s = describe.GetComponent<TMP_Text>().text;
                    describe.RemoveBetween((@"\(", @"\)"));
                    describe.RemoveBetween(('（', '）'));
                    break;
                case DisplayMode.Detail:
                    describe.RemoveBetween();
                    break;
            }

            coolDownInfo.SetTerm("CooldownInfo");
            ColdDownParamsManager.SetParameterValue("VALUE", curSkillInfo.Cooldown.ToString());

            maxLevel.SetTerm("MaxLvInfo");
            MaxParamsManager.SetParameterValue("MaxLv", curSkillInfo.MaxLv.ToString());
            MaxParamsManager.SetParameterValue("CurLv", Data.skillData.CurLv.ToString());

            curBelongProf.SetTerm("SkillBelong");
            CurBelongProfManager.SetParameterValue("VALUE", curSkillInfo.Prof);


            positiveInfo.SetTerm(curSkillInfo.Positive ? "positive" : "passive");
        }


        //关键词
        string[] curParameters;
        string totalDescribe = StringDefines.GetKeyWordDescribe(curSkillInfo.Keywords, out curParameters);


        if (!string.IsNullOrEmpty(totalDescribe))
        {
            m_keyWordObj.SetActive(true);
            m_keyWordDescribe_txt.SetTerm(totalDescribe);
            m_keyWordObj.transform.localPosition = GetCounterpointPosition();
            m_keyWordObj.GetComponent<RectTransform>().pivot = GetComponent<RectTransform>().pivot;
            for (int i = 0; i < curParameters.Length; i++)
            {
                _keyWordParamsManager.SetParameterValue($"P{i + 1}", curParameters[i]);
            }
        }
        else
        {
            m_keyWordObj.SetActive(false);
        }
    }

    public Vector2 GetCounterpointPosition()
    {
        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();
        Vector2 curPivot = rectTransform.pivot;
        Vector2 offPivot = new Vector2(1 - rectTransform.pivot.x, rectTransform.pivot.y);
        Vector2 targetPosition = new Vector2(rectTransform.sizeDelta.x * (curPivot.x - offPivot.x),
            rectTransform.sizeDelta.y * (curPivot.y - offPivot.y));
        //offPivot.x

        return -targetPosition;
    }

    public class Args
    {
        public Vector2 screenPosition;
        public SkillData skillData;
        public Transform worldTrans;
    }

    public class Args02 : Args
    {
        public Skill skill;
    }
}
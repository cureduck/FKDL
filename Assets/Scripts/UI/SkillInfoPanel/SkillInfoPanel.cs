using Game;
using I2.Loc;
using TMPro;
using Tools;
using UI;
using UnityEngine;
using UnityEngine.Serialization;

public class SkillInfoPanel : BasePanel<SkillInfoPanel.Args>
{
    [SerializeField] private Localize skillName;
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

    private Camera mainCamera;


    private DisplayMode displayMode => Input.GetKey(KeyCode.LeftAlt) ? DisplayMode.Detail : DisplayMode.Brief;

    private void Start()
    {
        mainCamera = Camera.main;
        DescParamsManager = describe.GetComponent<LocalizationParamsManager>();
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

        if (Data is Args02)
        {
            Skill curSkilInfo = (Data as Args02).skill;

            skillName.SetTerm(curSkilInfo.Id);
            if (curSkilInfo.Positive)
            {
                if (curSkilInfo.CostInfo.CostType == CostType.Hp)
                {
                    costInfo.SetTerm("HPCostInfo");
                }
                else if (curSkilInfo.CostInfo.CostType == CostType.Gold)
                {
                    costInfo.SetTerm("GoldCostInfo");
                }
                else
                {
                    costInfo.SetTerm("MPCostInfo");
                }

                CostParamsManager.SetParameterValue("VALUE", curSkilInfo.CostInfo.Value.ToString());
            }
            else
            {
                costInfo.SetTerm("NoCost");
                //costInfo.text = $"无消耗";
            }

            coolDownInfo.SetTerm("CooldownInfo");
            ColdDownParamsManager.SetParameterValue("VALUE", curSkilInfo.Cooldown.ToString());

            //describe.SetTerm($"{curSkilInfo.Id}_desc");
            describe.SetTerm($"{curSkilInfo.Id}_desc");
            describe.SetLocalizeParam(
                new[] { "P1", "P2", "CurLv" },
                new[]
                {
                    curSkilInfo.Param1.ToString(), curSkilInfo.Param2.ToString(), 1.ToString()
                });
            switch (displayMode)
            {
                case DisplayMode.Brief:
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
            MaxParamsManager.SetParameterValue("MaxLv", curSkilInfo.MaxLv.ToString());
            MaxParamsManager.SetParameterValue("CurLv", "0");

            curBelongProf.SetTerm("SkillBelong");
            CurBelongProfManager.SetParameterValue("VALUE", curSkilInfo.Prof);

            positiveInfo.SetTerm(curSkilInfo.Positive ? "positive" : "passive");
        }
        else
        {
            Skill curSkillInfo = Data.skillData.Bp;

            skillName.SetTerm(curSkillInfo.Id);
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
                case DisplayMode.Brief:
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
    }

    private enum DisplayMode
    {
        Detail,
        Brief
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
using I2.Loc;
using Tools;
using UI;
using UnityEngine;

public class SimpleInfoItemPanel : BasePanel<SimpleInfoItemPanel.Args>
{
    [SerializeField] private Localize title_txt;
    [SerializeField] private Localize describe_txt;
    [SerializeField] private NotBeyoundTheScreen notBeyoundTheScreen;
    [SerializeField] private LocalizationParamsManager _paramsManager;

    [SerializeField] private GameObject m_keyWordObj;
    [SerializeField] private Localize m_keyWordDescribe_txt;

    [SerializeField] private LocalizationParamsManager _keyWordParamsManager;

    //[SerializeField] private NotBeyoundTheScreen m_keywordNotBeyoundScreen;
    private Camera mainCamera;
    private RectTransform rectTransform;

    private void Start()
    {
        mainCamera = Camera.main;
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (Data.worldTrans != null)
        {
            transform.position = mainCamera.WorldToScreenPoint(Data.worldTrans.transform.position);
        }

        Debug.Log(GetCounterpointPosition());
    }

    public override void Close()
    {
        base.Close();
    }

    public override void Init()
    {
        notBeyoundTheScreen.Init(transform.parent.GetComponent<Canvas>());
        //m_keywordNotBeyoundScreen.Init(transform.parent.GetComponent<Canvas>());
    }

    protected override void UpdateUI()
    {
        title_txt.SetTerm(Data.title);
        describe_txt.SetTerm(Data.describe);
        if (Data.curParams != null)
        {
            for (int i = 0; i < Data.curParams.Length; i++)
            {
                _paramsManager.SetParameterValue($"P{i + 1}", Data.curParams[i]);
            }
        }

        //_paramsManager.SetParameterValue("P2", Data.param2);
        describe_txt.Calculate();
        notBeyoundTheScreen.PanelFollowQuadrant(Data.screenPosition);
        transform.position = Data.screenPosition;

        if (!string.IsNullOrEmpty(Data.otherDescribe))
        {
            m_keyWordObj.SetActive(true);
            m_keyWordDescribe_txt.SetTerm(Data.otherDescribe);
            m_keyWordObj.transform.localPosition = GetCounterpointPosition();
            m_keyWordObj.GetComponent<RectTransform>().pivot = GetComponent<RectTransform>().pivot;
            for (int i = 0; i < Data.curOtherDescribeParams.Length; i++)
            {
                _keyWordParamsManager.SetParameterValue($"P{i + 1}", Data.curOtherDescribeParams[i]);
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
        public string[] curOtherDescribeParams;
        public string[] curParams;
        public string describe;

        public string otherDescribe;
        public Vector2 screenPosition;
        public string title;
        public Transform worldTrans;
    }
}
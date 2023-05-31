using I2.Loc;
using UI;
using UnityEngine;

public class SimpleInfoItemPanel : BasePanel<SimpleInfoItemPanel.Args>
{
    [SerializeField] private Localize title_txt;
    [SerializeField] private Localize describe_txt;
    [SerializeField] private NotBeyoundTheScreen notBeyoundTheScreen;
    [SerializeField] private LocalizationParamsManager _paramsManager;

    private Camera mainCamera;


    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Data.worldTrans != null)
        {
            transform.position = mainCamera.WorldToScreenPoint(Data.worldTrans.transform.position);
        }
    }

    public override void Close()
    {
        base.Close();
    }

    public override void Init()
    {
        notBeyoundTheScreen.Init(transform.parent.GetComponent<Canvas>());
    }

    protected override void UpdateUI()
    {
        title_txt.SetTerm(Data.title);
        describe_txt.SetTerm(Data.describe);
        _paramsManager.SetParameterValue("P1", Data.param1);
        _paramsManager.SetParameterValue("P2", Data.param2);
        notBeyoundTheScreen.PanelFollowQuadrant(Data.screenPosition);
        transform.position = Data.screenPosition;
    }

    public class Args
    {
        public string describe;
        public string param1;
        public string param2;
        public Vector2 screenPosition;
        public string title;
        public Transform worldTrans;
    }
}
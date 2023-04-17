using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI;
using I2.Loc;

public class SimpleInfoItemPanel : BasePanel<SimpleInfoItemPanel.Args>
{

    public class Args 
    {
        public Vector2 screenPosition;
        public string title;
        public string describe;
        public string param;
    }

    [SerializeField]
    private Localize title_txt;
    [SerializeField]
    private Localize describe_txt;
    [SerializeField]
    private NotBeyoundTheScreen notBeyoundTheScreen;
    [SerializeField]
    private LocalizationParamsManager _paramsManager;

    public override void Init()
    {
        notBeyoundTheScreen.Init(transform.parent.GetComponent<Canvas>());
    }

    protected override void UpdateUI()
    {
        title_txt.SetTerm(Data.title);
        describe_txt.SetTerm(Data.describe);
        _paramsManager.SetParameterValue("P1", Data.param);
        notBeyoundTheScreen.PanelFollowQuadrant(Data.screenPosition);
        transform.position = Data.screenPosition;
    }


}

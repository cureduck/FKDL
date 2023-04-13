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
    }

    [SerializeField]
    private Localize title_txt;
    [SerializeField]
    private Localize describe_txt;
    [SerializeField]
    private NotBeyoundTheScreen notBeyoundTheScreen;

    public override void Init()
    {
        notBeyoundTheScreen.Init(transform.parent.GetComponent<Canvas>());
    }

    protected override void UpdateUI()
    {
        title_txt.SetTerm(Data.title);
        describe_txt.SetTerm(Data.describe);

        notBeyoundTheScreen.PanelFollowQuadrant(Data.screenPosition);
        transform.position = Data.screenPosition;
    }


}

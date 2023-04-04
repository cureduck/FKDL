using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI;
using TMPro;
using UnityEngine.UI;

public class ConfirmPanel : BasePanel<ConfirmPanel.Args>
{
    public class Args 
    {
        public System.Action curEvent;
        public string info;
    }

    [SerializeField]
    private TMP_Text info;
    [SerializeField]
    private Button click_btn;
    [SerializeField]
    private Button cancel_btn;

    private void Start()
    {
        click_btn.onClick.AddListener(OnClick);
        cancel_btn.onClick.AddListener(Close);
    }

    protected override void OnOpen()
    {
        info.text = Data.info;
    }

    protected override void UpdateUI()
    {
        
    }

    private void OnClick()
    {
        Data.curEvent?.Invoke();
        Close();
    }

}

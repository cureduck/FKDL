﻿using Managers;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmPanel : BasePanel<ConfirmPanel.Args>
{
    [SerializeField] private TMP_Text info;
    [SerializeField] private Button click_btn;
    [SerializeField] private Button cancel_btn;

    private void Start()
    {
        click_btn.onClick.AddListener(OnClick);
        cancel_btn.onClick.AddListener(Close);
    }

    protected override void OnOpen()
    {
        info.text = Data.info;
        transform.SetAsLastSibling();
    }

    protected override void UpdateUI()
    {
    }

    public override void Close()
    {
        base.Close();
        AudioPlayer.Instance.Play(AudioPlayer.AuidoUIButtonClick);
    }

    private void OnClick()
    {
        Data.curEvent?.Invoke();
        Close();
    }

    public class Args
    {
        public System.Action curEvent;
        public string info;
    }
}
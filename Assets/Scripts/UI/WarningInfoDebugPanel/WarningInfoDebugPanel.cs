using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI;
using I2.Loc;

public class WarningInfoDebugPanel : BasePanel<string>
{
    [SerializeField] private GameObject targetObject;
    [SerializeField] private Localize warningInfo;

    private void Start()
    {
        targetObject.gameObject.SetActive(false);
    }

    protected override void OnOpen()
    {
        warningInfo.SetTerm(Data);
        targetObject.gameObject.SetActive(false);
        targetObject.gameObject.SetActive(true);
    }


    protected override void UpdateUI()
    {
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Game;
using Managers;
using System;

public class CellRelicView : MonoBehaviour
{
    private const string Title = "UI_Icon_Relic_";

    [SerializeField] private Image relicIcon;
    [SerializeField] private Image activeIcon;
    [SerializeField] private Text relic_count;
    [SerializeField] private PointEnterAndExit pointEnterAndExit;

    private RelicData relicData;

    private void Start()
    {
        pointEnterAndExit.onPointEnter.AddListener(OnPointEnter);
        pointEnterAndExit.onPointExit.AddListener(OnPointExit);
        activeIcon.gameObject.SetActive(false);
    }

    private void OnPointExit()
    {
        if (relicData != null)
        {
            WindowManager.Instance.simpleInfoItemPanel.Close();
        }
    }

    private void OnPointEnter()
    {
        if (relicData != null)
        {
            WindowManager.Instance.simpleInfoItemPanel.Open(new SimpleInfoItemPanel.Args
                { describe = "这是一个遗物", title = relicData.Id, screenPosition = transform.position });
        }
    }

    public void SetData(RelicData relicData)
    {
        if (this.relicData != null)
        {
            this.relicData.Activated -= OnTrigger;
        }

        this.relicData = relicData;
        if (this.relicData != null)
        {
            this.relicData.Activated += OnTrigger;
        }
        //Debug.Log($"{Title}{relicData.Id}");

        relic_count.gameObject.SetActive(relicData.Counter > 1);
        relic_count.text = $"{relicData.Counter}";
        relicIcon.sprite = relicData.Bp.Icon;
        activeIcon.sprite = relicData.Bp.Icon;

        //relicIcon.sprite = SpriteManager.Instance.GetIcon(SpriteManager.IconType.Relic, $"{Title}{relicData.Id}");
    }

    private void OnTrigger()
    {
        Debug.LogError("触发relic效果！并播放动画（这段代码如果影响测试，请删除，当出现需要删除该代码时，说明接入成功）");
        activeIcon.gameObject.SetActive(false);
        activeIcon.gameObject.SetActive(true);
    }
}
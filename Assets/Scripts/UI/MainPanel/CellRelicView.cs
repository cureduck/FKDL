using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Game;
using Managers;

public class CellRelicView : MonoBehaviour
{
    private const string Title = "UI_Icon_Relic_";

    [SerializeField]
    private Image relicIcon;
    [SerializeField]
    private Text relic_count;
    [SerializeField]
    private PointEnterAndExit pointEnterAndExit;

    private RelicData relicData;

    private void Start()
    {
        pointEnterAndExit.onPointEnter.AddListener(OnPointEnter);
        pointEnterAndExit.onPointExit.AddListener(OnPointExit);
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
            WindowManager.Instance.simpleInfoItemPanel.Open(new SimpleInfoItemPanel.Args { describe = "这是一个遗物", title = relicData.Id, screenPosition = transform.position });
        }
    }

    public void SetData(RelicData relicData)
    {
        this.relicData = relicData;
        //Debug.Log($"{Title}{relicData.Id}");

        relic_count.gameObject.SetActive(relicData.Counter > 1);
        relic_count.text = $"{relicData.Counter}";
        relicIcon.sprite = SpriteManager.Instance.GetIcon(SpriteManager.IconType.Relic, $"{Title}{relicData.Id}");

    }

}

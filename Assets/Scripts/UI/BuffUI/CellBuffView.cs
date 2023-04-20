using Game;
using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Managers;
using System;

public class CellBuffView : MonoBehaviour
{
    private const string IconTitle = "UI_Icon_Buff_";
    [SerializeField]
    private Text level_txt;
    [SerializeField]
    private Image icon_img;
    [SerializeField]
    private PointEnterAndExit pointEnterAndExit;

    [SerializeField] private Image OutLine;

    private BuffData buffData;
    private void Start()
    {
        pointEnterAndExit.onPointEnter.AddListener(OnPointEnter);
        pointEnterAndExit.onPointExit.AddListener(OnPointExit);
    }

    private void OnPointExit()
    {
        if (buffData != null)
        {
            WindowManager.Instance.simpleInfoItemPanel.Close();
        }

    }

    private void OnPointEnter()
    {
        if (buffData != null)
        {
            WindowManager.Instance.simpleInfoItemPanel.Open(new SimpleInfoItemPanel.Args { describe = $"{buffData.Id}_desc", param =buffData.CurLv.ToString(), title = buffData.Id, screenPosition = transform.position });
        }
    }

    public void SetData(BuffData buffData)
    {
        this.buffData = buffData;

        level_txt.gameObject.SetActive(buffData.CurLv > 1);
        level_txt.text = buffData.CurLv.ToString();
        icon_img.sprite = buffData.Bp.Icon;
        //icon_img.sprite = SpriteManager.Instance.GetIcon(SpriteManager.IconType.Buff, $"{IconTitle}{buffData.Id}");

        switch (buffData.Bp.BuffType)
        {
            case BuffType.Positive:
                OutLine.color = (Color.yellow);
                break;
            case BuffType.Negative:
                OutLine.color = (Color.red);
                break;
            case BuffType.Bless:
                OutLine.color = (Color.black);
                break;
            case BuffType.Curse:
                OutLine.color = (Color.white);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }



}
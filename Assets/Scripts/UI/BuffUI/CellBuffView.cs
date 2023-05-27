using System;
using Game;
using Managers;
using UnityEngine;
using UnityEngine.UI;

public class CellBuffView : MonoBehaviour
{
    private const string IconTitle = "UI_Icon_Buff_";
    [SerializeField] private Text level_txt;
    [SerializeField] private Image icon_img;
    [Header("鼠标交互")] [SerializeField] private PointEnterAndExit pointEnterAndExit;
    [SerializeField] private ObjectColliderPointEnterAndExit objectPointEnterAndExit;

    [SerializeField] private Image OutLine;
    private BuffData buffData;
    private bool isWorldObject = false;

    private void Start()
    {
        if (pointEnterAndExit)
        {
            pointEnterAndExit.onPointEnter.AddListener(OnPointEnter);
            pointEnterAndExit.onPointExit.AddListener(OnPointExit);
        }

        if (objectPointEnterAndExit)
        {
            objectPointEnterAndExit.onPointEnter.AddListener(OnPointEnter);
            objectPointEnterAndExit.onPointExit.AddListener(OnPointExit);
        }
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
            if (isWorldObject)
            {
                WindowManager.Instance.simpleInfoItemPanel.Open(new SimpleInfoItemPanel.Args
                {
                    describe = $"{buffData.Id}_desc", param1 = buffData.CurLv.ToString(), title = buffData.Id,
                    worldTrans = transform
                });
            }
            else
            {
                WindowManager.Instance.simpleInfoItemPanel.Open(new SimpleInfoItemPanel.Args
                {
                    describe = $"{buffData.Id}_desc", param1 = buffData.CurLv.ToString(), title = buffData.Id,
                    screenPosition = transform.position
                });
            }
        }
    }

    public void SetData(BuffData buffData, bool isWorldObject)
    {
        this.buffData = buffData;
        this.isWorldObject = isWorldObject;

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
            case BuffType.Blessing:
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
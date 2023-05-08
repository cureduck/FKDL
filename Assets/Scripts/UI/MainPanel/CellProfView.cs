using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Game;
using Managers;

public class CellProfView : MonoBehaviour
{
    public const string Title = "UI_Icon_Prof_";

    [SerializeField] private Image icon_img;
    [SerializeField] private PointEnterAndExit pointEnterAndExit;

    private string profIndex;

    private void Start()
    {
        icon_img.gameObject.SetActive(false);
        pointEnterAndExit.onPointEnter.AddListener(OnPointEnter);
        pointEnterAndExit.onPointExit.AddListener(OnPointExit);
    }

    public void SetData(string profIndex)
    {
        if (string.IsNullOrEmpty(profIndex))
        {
            icon_img.gameObject.SetActive(false);
        }
        else
        {
            this.profIndex = profIndex;
            icon_img.gameObject.SetActive(true);
            icon_img.sprite = SpriteManager.Instance.GetIcon(SpriteManager.IconType.Prof, $"{Title}{profIndex}");
        }
    }

    private void OnPointEnter()
    {
        if (string.IsNullOrEmpty(profIndex)) return;
        WindowManager.Instance.simpleInfoItemPanel.Open(new SimpleInfoItemPanel.Args
            { describe = $"{profIndex}职业描述", title = profIndex, screenPosition = transform.position });
    }

    private void OnPointExit()
    {
        if (string.IsNullOrEmpty(profIndex)) return;
        WindowManager.Instance.simpleInfoItemPanel.Close();
    }
}
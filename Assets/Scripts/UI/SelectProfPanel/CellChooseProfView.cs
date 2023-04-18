using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Managers;

public class CellChooseProfView : MonoBehaviour
{
    [SerializeField]
    private Image icon;
    [SerializeField]
    private PointEnterAndExit pointEnterAndExit;
    [SerializeField]
    private Toggle toggle;

    private string profIndex;
    private System.Action<CellChooseProfView, string,bool> cellViewClick;

    private System.Action<CellChooseProfView, string> onPointEnter;
    private System.Action<CellChooseProfView, string> onPointExit;

    private void Start()
    {
        pointEnterAndExit.onPointEnter.AddListener(OnPointEnter);
        pointEnterAndExit.onPointExit.AddListener(OnPointExit);
        toggle.onValueChanged.AddListener(OnClick);
    }

    public void SetData(string profIndex, System.Action<CellChooseProfView, string, bool> cellViewClick, System.Action<CellChooseProfView, string> onPointEnter, System.Action<CellChooseProfView, string> onPointExit) 
    {
        this.profIndex = profIndex;
        this.cellViewClick = cellViewClick;
        this.onPointEnter = onPointEnter;
        this.onPointExit = onPointExit;
        toggle.interactable = true;
        icon.sprite = SpriteManager.Instance.GetIcon(SpriteManager.IconType.Prof, $"{CellProfView.Title}{profIndex}");

    }

    public void SetToggleInteractable(bool interactable) 
    {
        toggle.interactable = interactable;
    }

    public void SetToggleOnState(bool isOn) 
    {
        toggle.isOn = isOn;
    }

    private void OnClick(bool isActive) 
    {
        cellViewClick?.Invoke(this, profIndex, isActive);
    }

    private void OnPointEnter()
    {
        if (string.IsNullOrEmpty(profIndex)) return;
        onPointEnter?.Invoke(this, profIndex);
        //WindowManager.Instance.simpleInfoItemPanel.Open(new SimpleInfoItemPanel.Args { describe = $"{profIndex}职业描述", title = profIndex, screenPosition = transform.position });
    }

    private void OnPointExit()
    {
        if (string.IsNullOrEmpty(profIndex)) return;
        onPointExit?.Invoke(this, profIndex);
        //WindowManager.Instance.simpleInfoItemPanel.Close();
    }

}

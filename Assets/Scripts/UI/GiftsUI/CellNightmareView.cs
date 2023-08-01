using I2.Loc;
using Managers;
using UnityEngine;
using UnityEngine.UI;

public class CellNightmareView : MonoBehaviour
{
    [SerializeField] private Localize title_loc;
    [SerializeField] private Localize describe_loc;
    [SerializeField] private Image icon_img;
    [SerializeField] private Toggle toggle;
    private string curDataIndex;

    private System.Action<string, bool> onToggleValueChange;

    public void Start()
    {
        toggle.onValueChanged.AddListener(OnToggleValueChange);
    }

    public void SetData(string curDataIndex, System.Action<string, bool> onToggleValueChange)
    {
        this.curDataIndex = curDataIndex;
        title_loc.SetTerm(this.curDataIndex);
        describe_loc.SetTerm(this.curDataIndex + "_desc");
        SpriteManager.Instance.BuffIcons.TryGetValue(curDataIndex, out var sprite);
        icon_img.sprite = sprite;
        this.onToggleValueChange = onToggleValueChange;
    }

    private void OnToggleValueChange(bool active)
    {
        onToggleValueChange?.Invoke(curDataIndex, active);
    }
}
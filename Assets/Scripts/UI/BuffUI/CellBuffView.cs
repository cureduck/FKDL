using Game;
using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Managers;

public class CellBuffView : MonoBehaviour
{
    private const string IconTitle = "UI_Icon_Buff_";
    [SerializeField]
    private Text level_txt;
    [SerializeField]
    private Image icon_img;

    public void SetData(BuffData buffData)
    {
        level_txt.text = buffData.CurLv.ToString();
        icon_img.sprite = SpriteManager.Instance.GetIcon(SpriteManager.IconType.Buff, $"{IconTitle}{buffData.Id}");
        Debug.Log(buffData);
    }
}
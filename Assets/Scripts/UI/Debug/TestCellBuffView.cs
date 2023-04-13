using Game;
using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class TestCellBuffView : CellView<BuffData>
    {
        private const string IconTitle = "UI_Icon_Buff_";
        [SerializeField]
        public Text level_txt;
        [SerializeField]
        public Image icon_img;
        

        public override void UpdateUI()
        {
            level_txt.gameObject.SetActive(true);
            icon_img.gameObject.SetActive(true);
            level_txt.text = Data.CurLv.ToString();
            icon_img.sprite = SpriteManager.Instance.GetIcon(SpriteManager.IconType.Buff, $"{IconTitle}{Data.Id}");
            Debug.Log(Data);
        }
    }
}
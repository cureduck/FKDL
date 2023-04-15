using DG.Tweening;
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

        protected override void OnActivated()
        {
            base.OnActivated();
            _seq.Kill();
            _seq = DOTween.Sequence();
            _seq.Append(icon_img.transform.DOScale(1.2f, 1f))
                .Insert(1f, icon_img.transform.DOScale(1f, .5f))
                .OnComplete(() => icon_img.transform.localScale = Vector3.one);
        }

        private Sequence _seq;
        

        public override void UpdateUI()
        {
            level_txt.gameObject.SetActive(true);
            icon_img.gameObject.SetActive(true);
            level_txt.text = Data.CurLv.ToString();
            icon_img.sprite = Data?.Bp?.Icon;
            Debug.Log(Data);
        }
    }
}
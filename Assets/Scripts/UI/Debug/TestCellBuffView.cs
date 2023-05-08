using System;
using DG.Tweening;
using Game;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class TestCellBuffView : CellView<BuffData>
    {
        private const string IconTitle = "UI_Icon_Buff_";
        [SerializeField] public TMP_Text level_txt;
        [SerializeField] public Image icon_img;
        public Image OutLine;

        protected override void OnActivated()
        {
            base.OnActivated();
            _seq.Kill();
            _seq = DOTween.Sequence();
            _seq.Append(icon_img.transform.DOScale(1.2f, 1f))
                .Insert(1f, icon_img.transform.DOScale(1f, .5f))
                .OnComplete(() => icon_img.transform.localScale = Vector3.one);
        }

        public override string Id => Data.Id;
        public override string Desc => $"{Data.Id}_desc";
        public override string Param => Data.CurLv.ToString();

        private Sequence _seq;


        public override void UpdateUI()
        {
            if (Data.Bp == null)
            {
                return;
            }

            icon_img.gameObject.SetActive(true);
            level_txt.gameObject.SetActive(Data.Bp.Stackable);
            if (Data.Bp.Stackable)
            {
                level_txt.text = Data.CurLv.ToString();
            }

            icon_img.sprite = Data?.Bp?.Icon;
            switch (Data.Bp.BuffType)
            {
                case BuffType.Positive:
                    OutLine.color = Color.yellow;
                    break;
                case BuffType.Negative:
                    OutLine.color = Color.red;
                    break;
                case BuffType.Bless:
                    OutLine.color = Color.black;
                    break;
                case BuffType.Curse:
                    OutLine.color = Color.cyan;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
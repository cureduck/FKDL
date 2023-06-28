using Game;
using I2.Loc;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    public class SquareInfoPanel : BasePanel<SquareInfo>
    {
        [SerializeField] private Localize Id;
        [SerializeField] private Localize Desc;
        [SerializeField] private Image Bg;

        protected override void UpdateUI()
        {
            Id.SetTerm(Data.Name);
            Desc.SetTerm(Data.Desc);

            Bg.color = GameManager.Instance.SquareColors.TryGetValue(Id.Term, out var color) ? color : Color.grey;

            //set bg color alpha to .6
            var color1 = Bg.color;
            color1.a = .6f;
            Bg.color = color1;

            Desc.GetComponent<LocalizationParamsManager>().SetParameterValue("P1", Data.P1);
            Desc.GetComponent<LocalizationParamsManager>().SetParameterValue("P2", Data.P2);
        }
    }
}
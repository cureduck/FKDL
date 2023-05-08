using System.Globalization;
using Game;
using I2.Loc;
using TMPro;
using UI;
using UnityEngine;

namespace Managers
{
    public class SquareInfoPanel : BasePanel<SquareInfo>
    {
        [SerializeField] private Localize Id;
        [SerializeField] private Localize Desc;


        protected override void UpdateUI()
        {
            Id.SetTerm(Data.Name);
            Desc.SetTerm(Data.Desc);

            //var term = LocalizationManager.GetTranslation(Data.P1);

            Desc.GetComponent<LocalizationParamsManager>().SetParameterValue("P1", Data.P1);
            Desc.GetComponent<LocalizationParamsManager>().SetParameterValue("P2", Data.P2);
        }
    }
}
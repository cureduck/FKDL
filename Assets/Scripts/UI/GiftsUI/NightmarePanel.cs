using System.Collections.Generic;
using System.Globalization;
using EasyTransition;
using Game;
using I2.Loc;
using Managers;
using Tools;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI
{
    public class NightmarePanel : MonoBehaviour
    {
        [FormerlySerializedAs("_cellNightmarePrefab")] [SerializeField]
        private CellNightmareView m_CellNightmarePrefab;

        [SerializeField] private Localize curSelectText;

        [SerializeField] private LocalizationParamsManager curSelectCountLizationManager;

        //[SerializeField] private ScrollViewAndBarOnVertical customSizeFitter;
        [SerializeField] private Button _startButton;
        [SerializeField] private Localize m_NightmareBonus;
        private List<string> _chosenNightmares;
        private Dictionary<string, CellNightmareView> _nightmareToggles = new Dictionary<string, CellNightmareView>();
        private SecondaryData SData => GameDataManager.Instance.SecondaryData;

        private void Start()
        {
            _chosenNightmares = new List<string>();
            _startButton.onClick.AddListener(StartGame);
            curSelectText.SetTerm("UI_GiftsAndNightmarePanel_CurSelectNightmareCountView");
            curSelectCountLizationManager.SetParameterValue("P1", _chosenNightmares.Count.ToString());
            PrepareNightmarePanel();
        }

        private void PrepareNightmarePanel()
        {
            var options = GetNightmareOptions();
            foreach (var nightmare in options)
            {
                var curCellObj = Instantiate(m_CellNightmarePrefab, transform);
                _nightmareToggles.Add(nightmare, curCellObj);
                curCellObj.SetData(nightmare, OnCellToggleValueChange);
                //curObj.GetComponentInChildren<TMPro.TMP_Text>().text = nightmare;
                //curObj.GetComponent<UnityEngine.UI.Toggle>().onValueChanged.AddListener((value) =>
                //{
                //    if (value)
                //    {
                //        _chosenNightmares.Add(nightmare);
                //    }
                //    else
                //    {
                //        _chosenNightmares.Remove(nightmare);
                //    }
                //});
            }

            SetNightmareBonus();
            //customSizeFitter.AdjustTheListLength();
        }

        private void OnCellToggleValueChange(string nightmare, bool value)
        {
            if (value)
            {
                _chosenNightmares.Add(nightmare);
            }
            else
            {
                _chosenNightmares.Remove(nightmare);
            }

            curSelectText.SetTerm("UI_GiftsAndNightmarePanel_CurSelectNightmareCountView");
            curSelectCountLizationManager.SetParameterValue("P1", _chosenNightmares.Count.ToString());
            SetNightmareBonus();
        }

        private string[] GetNightmareOptions()
        {
            return Nightmares.NightmareLib.ChooseRandom(5, SData.CurGameRandom);
        }

        private void SetNightmareBonus()
        {
            m_NightmareBonus.SetLocalizeParam("P1",
                Nightmares.GetAllNightmareBonus(_chosenNightmares).ToString(CultureInfo.InvariantCulture));
        }

        private void StartGame()
        {
            var panel = FindObjectOfType<GiftsPanel>();
            SData.Gifts = panel.GetSelectedGifts();
            panel.Save();
            SData.Nightmares = _chosenNightmares.ToArray();
            SData.Save();
            FindObjectOfType<TransitionManager>().LoadScene("MainScene", "DiagonalRectangleGrid", .2f);
        }
    }
}
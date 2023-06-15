using System.Collections.Generic;
using EasyTransition;
using Game;
using Managers;
using Tools;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class NightmarePanel : MonoBehaviour
    {
        [SerializeField] private GameObject _nightmareTogglePrefab;

        [SerializeField] private Button _startButton;
        private List<string> _chosenNightmares;
        private Dictionary<string, GameObject> _nightmareToggles = new Dictionary<string, GameObject>();
        private SecondaryData SData => GameDataManager.Instance.SecondaryData;

        private void Start()
        {
            _chosenNightmares = new List<string>();
            _startButton.onClick.AddListener(StartGame);
            PrepareNightmarePanel();
        }

        private void PrepareNightmarePanel()
        {
            var options = GetNightmareOptions();
            foreach (var nightmare in options)
            {
                var toggle = Instantiate(_nightmareTogglePrefab, transform);
                _nightmareToggles.Add(nightmare, toggle);
                toggle.SetActive(true);
                toggle.GetComponentInChildren<TMPro.TMP_Text>().text = nightmare;
                toggle.GetComponent<UnityEngine.UI.Toggle>().onValueChanged.AddListener((value) =>
                {
                    if (value)
                    {
                        _chosenNightmares.Add(nightmare);
                    }
                    else
                    {
                        _chosenNightmares.Remove(nightmare);
                    }
                });
            }
        }

        private string[] GetNightmareOptions()
        {
            return Nightmares.NightmareDict.Values.ChooseRandom(3, SData.CurGameRandom);
        }

        private void StartGame()
        {
            SData.Gifts = FindObjectOfType<GiftsPanel>().GetSelectedGifts();
            FindObjectOfType<TransitionManager>().LoadScene("MainScene", "DiagonalRectangleGrid", .2f);
        }
    }
}
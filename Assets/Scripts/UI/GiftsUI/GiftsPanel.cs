using System.Collections.Generic;
using System.Linq;
using Game;
using I2.Loc;
using Managers;
using Tools;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI
{
    public class GiftsPanel : MonoBehaviour
    {
        [SerializeField] private CellGiftView _giftTogglePrefab;

        [SerializeField] private Localize pointLeftText;

        [FormerlySerializedAs("pointLizationManager")] [SerializeField]
        private LocalizationParamsManager pointLocalizationManager;

        [SerializeField] private Localize soulsLeftText;

        [FormerlySerializedAs("soulLizationManager")] [SerializeField]
        private LocalizationParamsManager soulLocalizationManager;

        [SerializeField] private Localize curSelectText;

        [FormerlySerializedAs("curSelectCountLizationManager")] [SerializeField]
        private LocalizationParamsManager curSelectCountLocalizationManager;

        //[SerializeField] private ScrollViewAndBarOnVertical customSizeFitter;
        private Dictionary<Gift, Toggle> _giftToggles = new Dictionary<Gift, Toggle>();
        private Profile _profile;
        private SecondaryData SData => GameDataManager.Instance.SecondaryData;

        private void Start()
        {
            _profile = Profile.GetOrCreate();
            PrepareGiftPanel();
            UpdatePointLeft();
        }

        private void OnDestroy()
        {
            Save();
        }

        private void UpdatePointLeft()
        {
            int leftGiftPoint = (Gift.MaxPoint - GetSelectedGifts().Sum((gift => gift.PointCost)));
            pointLeftText.SetTerm("UI_GiftsAndNightmarePanel_LeftTalentPoints");
            pointLocalizationManager.SetParameterValue("P1", leftGiftPoint.ToString());

            soulsLeftText.SetTerm("UI_GiftsAndNightmarePanel_LeftSoulPoints");
            soulLocalizationManager.SetParameterValue("P1", _profile.CollectedSouls.ToString());

            //curSelectText.SetTerm(GetSelectedGifts().Length.ToString());
            curSelectText.SetTerm("UI_GiftsAndNightmarePanel_CurSelectGiftCountView");
            curSelectCountLocalizationManager.SetParameterValue("P1", GetSelectedGifts().Length.ToString());
        }

        private void PrepareGiftPanel()
        {
            var options = GetGiftOptions();
            foreach (var gift in options)
            {
                var cell = Instantiate(_giftTogglePrefab, transform);
                cell.UpgradeCallback = UpgradeGift;
                var toggle = cell.GetComponent<Toggle>();
                _giftToggles.Add(gift, toggle);
                toggle.isOn = false;
                toggle.gameObject.SetActive(true);

                cell.Open(gift);

                toggle.onValueChanged.AddListener((value) =>
                {
                    if (value)
                    {
                        if (CanAffordGift(gift))
                        {
                            ChooseGift(gift);
                        }

                        ResetAllToggleInteractable();
                    }
                    else
                    {
                        cell.UpdateView();
                        ResetAllToggleInteractable();
                    }

                    UpdatePointLeft();
                });
                toggle.GetComponentInChildren<Button>()?.onClick.AddListener(() =>
                {
                    if (CanAffordUpgrade(gift) && gift.CurrentLevel < gift.MaxLevel)
                    {
                        //UpgradeGift(gift);
                        UpdatePointLeft();
                        cell.UpdateView();
                    }
                });
            }

            //customSizeFitter.AdjustTheListLength();
        }

        private void ResetAllToggleInteractable()
        {
            foreach (var t in _giftToggles)
            {
                if (!t.Value.isOn)
                {
                    t.Value.interactable = CanAffordGift(t.Key);
                }
            }
        }


        private bool CanAffordUpgrade(Gift gift)
        {
            return gift.LevelUpCost <= _profile.CollectedSouls;
        }

        private bool CanAffordGift(Gift gift)
        {
            return gift.PointCost <= Gift.MaxPoint - GetSelectedGifts().Sum((g => g.PointCost));
        }


        private void UpgradeGift(Gift gift)
        {
            if (_profile.CollectedSouls < gift.LevelUpCost)
            {
                WindowManager.Instance.warningInfoPanel.Open("UI_Normal_NotEnoughSoul");
                return;
            }

            gift.Upgrade();
            foreach (var gift1 in Gift.GiftDictionary)
            {
                _profile.GiftsLevel[gift1.Key] = gift1.Value.CurrentLevel;
            }

            _profile.CollectedSouls -= gift.LevelUpCost;

            soulLocalizationManager.SetParameterValue("P1", _profile.CollectedSouls.ToString());

            _profile.Save();
        }

        private void ChooseGift(Gift gift)
        {
            UpdatePointLeft();
        }

        public Gift[] GetSelectedGifts()
        {
            return _giftToggles.Where((pair => pair.Value.isOn)).Select((pair => pair.Key)).ToArray();
        }

        private Gift[] GetGiftOptions()
        {
            var options = Gift.GiftDictionary.ChooseRandom(5, SData.CurGameRandom);
            foreach (var option in options)
            {
                option.Value.CurrentLevel = _profile.GiftsLevel[option.Key];
            }

            return options.Select((pair => pair.Value)).ToArray();
        }

        public void Save()
        {
            foreach (var gift in Gift.GiftDictionary)
            {
                _profile.GiftsLevel[gift.Key] = gift.Value.CurrentLevel;
            }

            _profile.Save();
        }
    }
}
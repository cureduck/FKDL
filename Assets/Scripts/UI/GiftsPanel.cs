using System.Collections.Generic;
using System.Linq;
using Game;
using I2.Loc;
using Managers;
using TMPro;
using Tools;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class GiftsPanel : MonoBehaviour
    {
        [SerializeField] private Toggle _giftTogglePrefab;

        [SerializeField] private TMP_Text pointLeftText;
        [SerializeField] private TMP_Text soulsLeftText;
        private Gifts _gifts;
        private Dictionary<Gift, Toggle> _giftToggles = new Dictionary<Gift, Toggle>();
        private Profile _profile;
        private SecondaryData SData => GameDataManager.Instance.SecondaryData;

        private void Start()
        {
            _gifts = Gifts.GetOrCreate();
            _profile = Profile.GetOrCreate();
            PrepareGiftPanel();
        }

        private void UpdatePointLeft()
        {
            pointLeftText.text = (Gifts.MaxPoint - _gifts.SpentPoint).ToString();
            soulsLeftText.text = _profile.CollectedSouls.ToString();
        }

        private void PrepareGiftPanel()
        {
            var options = GetGiftOptions();
            foreach (var gift in options)
            {
                var toggle = Instantiate(_giftTogglePrefab, transform);
                _giftToggles.Add(gift, toggle);
                toggle.isOn = false;
                toggle.gameObject.SetActive(true);
                toggle.GetComponentInChildren<Localize>().SetTerm(gift.GetType().Name);
                toggle.GetComponentInChildren<Localize>().SetLocalizeParam("CurLv", gift.CurrentLevel.ToString());
                toggle.GetComponentInChildren<Localize>().SetLocalizeParam("PointCost", gift.PointCost.ToString());
                toggle.GetComponentInChildren<Localize>().Calculate();

                toggle.onValueChanged.AddListener((value) =>
                {
                    if (value)
                    {
                        if (CanAffordGift(gift))
                        {
                            ChooseGift(gift);
                            ResetAllToggleInteractable();
                        }
                    }
                    else
                    {
                        _gifts.SpentPoint -= gift.PointCost;
                        ResetAllToggleInteractable();
                    }

                    UpdatePointLeft();
                });
                toggle.GetComponentInChildren<Button>().onClick.AddListener(() =>
                {
                    if (CanAffordUpgrade(gift))
                    {
                        UpgradeGift(gift);
                        UpdatePointLeft();
                    }
                });
            }
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
            return gift.PointCost <= Gifts.MaxPoint - _gifts.SpentPoint;
        }


        private void UpgradeGift(Gift gift)
        {
            gift.Upgrade();
            var profile = Profile.GetOrCreate();
            profile.CollectedSouls -= gift.LevelUpCost;
            profile.Save();
            _gifts.Save();
        }

        private void ChooseGift(Gift gift)
        {
            _gifts.SelectedGifts.Add(gift);
            _gifts.SpentPoint += gift.PointCost;
        }

        public Gift[] GetSelectedGifts()
        {
            return _gifts.SelectedGifts.ToArray();
        }


        private Gift[] GetGiftOptions()
        {
            var options = Gifts.GiftDictionary.ChooseRandom(5, SData.CurGameRandom);
            return options.Select((pair => pair.Value)).ToArray();
        }
    }
}
using System;
using Game;
using Managers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class OfferBtn : Button
    {
        private OfferUI _offerUi;
        
        protected override void Start()
        {
            base.Start();
            _offerUi = GetComponent<OfferUI>();

            onClick.AddListener(
                () =>
                {
                    if (GameManager.Instance.PlayerData.TryTake(_offerUi.Offer))
                    {
                        WindowManager.Instance.OffersWindow.SetActive(false);
                        return;
                    }
                    else
                    {
                        switch (_offerUi.Offer.Kind)
                        {
                            case Offer.OfferKind.Potion:
                                WindowManager.Instance.Warn("Potion Full");
                                break;
                            case Offer.OfferKind.Skill:
                                WindowManager.Instance.Warn("Skill Full");
                                break;
                            case Offer.OfferKind.Gold:
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                        
                    }



                });
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
        }
    }
}
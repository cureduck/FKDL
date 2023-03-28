using Game;
using System.Collections;
using Managers;
using UnityEngine;
using UnityEngine.UI;
//using System.Collections.Generic;

namespace UI
{
    public class OfferWindow : MonoBehaviour
    {
        [SerializeField]
        private OfferUI prefab;
        [SerializeField]
        private Button skip_btn;
        [SerializeField]
        private float delayCloseTime = 1.5f;
        [SerializeField]
        private Transform offerUIParent;
        [SerializeField]
        private OffersUIStartAnimationGroup animationGroup;


        public void Start()
        {
            skip_btn.onClick.AddListener(OnSkipButtonClick);
        }

        public void Load(Offer[] offers)
        {
            OffersUIStartAnimation[] offersUIStartAnimations = new OffersUIStartAnimation[offers.Length];
            for (int i = 0; i < offers.Length; i++)
            {
                OfferUI offerUI;
                if (i >= offerUIParent.transform.childCount)
                {
                    offerUI = Instantiate(prefab);
                    offerUI.transform.SetParent(offerUIParent);
                    offerUI.transform.localScale = Vector3.one;

                }
                else
                {
                    offerUI = offerUIParent.transform.GetChild(i).GetComponent<OfferUI>();
                    offerUI.gameObject.SetActive(true);
                }

                int targetIndex = i;
                Offer offer = offers[i];
                offerUI.SetData(offer,()=> { OnClick(offer, targetIndex); });
                offersUIStartAnimations[i] = offerUI.GetComponent<OffersUIStartAnimation>();
            }

            for (int i = offers.Length; i < offerUIParent.childCount; i++)
            {
                offerUIParent.GetChild(i).gameObject.SetActive(false);
            }

            gameObject.SetActive(true);
            animationGroup.Set(offersUIStartAnimations);
        }

        private void OnClick(Offer offer,int targetIndex) 
        {
            Debug.Log($"{offer}被点击");
            GameManager.Instance.PlayerData.TryTake(offer);
            animationGroup.SelectTarget(targetIndex);
            StartCoroutine(CloseWindowIE());
        }

        private void OnSkipButtonClick() 
        {
            animationGroup.SelectTarget(-1);
            StartCoroutine(CloseWindowIE());
        }

        private IEnumerator CloseWindowIE() 
        {

            yield return new WaitForSeconds(delayCloseTime);
            gameObject.SetActive(false);
        }


    }
}
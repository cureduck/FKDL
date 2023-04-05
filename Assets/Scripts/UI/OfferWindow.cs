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
                offerUI = Instantiate(prefab);
                offerUI.transform.SetParent(offerUIParent);
                offerUI.transform.localScale = Vector3.one;

                int targetIndex = i;
                Offer offer = offers[i];
                offerUI.SetData(offer,()=> { OnClick(offer, targetIndex); });
                offersUIStartAnimations[i] = offerUI.GetComponent<OffersUIStartAnimation>();
            }
            skip_btn.gameObject.SetActive(true);
            gameObject.SetActive(true);
            animationGroup.Set(offersUIStartAnimations);
        }

        private void OnDisable()
        {
            for (int i = 0; i < offerUIParent.childCount; i++)
            {
                Destroy(offerUIParent.GetChild(i).gameObject);
            }
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
            skip_btn.gameObject.SetActive(false);
            yield return new WaitForSeconds(delayCloseTime);
            gameObject.SetActive(false);
        }


    }
}
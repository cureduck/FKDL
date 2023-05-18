using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game;
using Managers;
using UnityEngine;
using UnityEngine.UI;

//using System.Collections.Generic;

namespace UI
{
    public class OfferWindow : BasePanel<Offer[]>
    {
        [SerializeField] private OfferUI prefab;
        [SerializeField] private Button skip_btn;
        [SerializeField] private float delayCloseTime = 1.5f;
        [SerializeField] private Transform offerUIParent;
        [SerializeField] private OffersUIStartAnimationGroup animationGroup;


        public void Start()
        {
            skip_btn.onClick.AddListener(OnSkipButtonClick);
        }

        private void OnDisable()
        {
            for (int i = 0; i < offerUIParent.childCount; i++)
            {
                Destroy(offerUIParent.GetChild(i).gameObject);
            }
        }

        public void Load(IEnumerable<Offer> ofs)
        {
            var offers = ofs.ToArray();
            //foreach (var c in ofs)
            //{
            //    Debug.Log(c.Kind);
            //}
            OffersUIStartAnimation[] offersUIStartAnimations = new OffersUIStartAnimation[offers.Length];
            Debug.Log(offers.Length);
            for (int i = 0; i < offers.Length; i++)
            {
                OfferUI offerUI;
                offerUI = Instantiate(prefab);
                offerUI.transform.SetParent(offerUIParent);
                offerUI.transform.localScale = Vector3.one;

                int targetIndex = i;
                Offer offer = offers[i];
                offerUI.SetData(offer, () => { OnClick(offer, targetIndex); });
                offersUIStartAnimations[i] = offerUI.GetComponent<OffersUIStartAnimation>();
            }

            skip_btn.gameObject.SetActive(true);
            gameObject.SetActive(true);
            animationGroup.Set(offersUIStartAnimations);
        }

        private void OnClick(Offer offer, int targetIndex)
        {
            //Debug.Log($"{offer}被点击");
            Info info;
            if (GameManager.Instance.PlayerData.TryTakeOffer(offer, out info))
            {
                animationGroup.SelectTarget(targetIndex);
                StartCoroutine(CloseWindowIE());
            }
            else
            {
                WindowManager.Instance.warningInfoPanel.Open(info.ToString());
            }
        }

        private void OnSkipButtonClick()
        {
            animationGroup.SelectTarget(-1);
            StartCoroutine(CloseWindowIE());
            Debug.LogWarning("获得金币!");
            AudioPlayer.Instance.Play(AudioPlayer.AuidoUIButtonClick);
        }

        private IEnumerator CloseWindowIE()
        {
            skip_btn.gameObject.SetActive(false);
            yield return new WaitForSeconds(delayCloseTime);
            gameObject.SetActive(false);
        }


        protected override void UpdateUI()
        {
            Load(Data);
        }
    }
}
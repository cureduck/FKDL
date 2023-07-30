using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game;
using I2.Loc;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//using System.Collections.Generic;

namespace UI
{
    public class OfferWindow : BasePanel<(Offer[] offers, string title, int skipCoinCount)>
    {
        [SerializeField] private Localize title;
        [SerializeField] private OfferUI prefab;
        [SerializeField] private TMP_Text gold_txt;
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

        public void Load(IEnumerable<Offer> ofs, string title)
        {
            var offers = ofs.ToArray();
            //foreach (var c in ofs)
            //{
            //    Debug.Log(c.Kind);
            //}
            this.title.SetTerm(title);
            gold_txt.text = Data.skipCoinCount.ToString();
            OffersUIStartAnimation[] offersUIStartAnimations = new OffersUIStartAnimation[offers.Length];
            for (int i = 0; i < offers.Length; i++)
            {
                OfferUI offerUI;
                offerUI = Instantiate(prefab);
                offerUI.transform.SetParent(offerUIParent);
                offerUI.transform.localScale = Vector3.one;

                int targetIndex = i;
                Offer offer = offers[i];
                offerUI.SetData(offer, false, () => { OnClick(offer, targetIndex); });
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
            if (GameManager.Instance.Player.TryTakeOffer(offer, out info))
            {
                AudioPlayer.Instance.Play(AudioPlayer.AudioGetItem);
                animationGroup.SelectTarget(targetIndex);
                delayCloseTime = 1.0f;
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
            GameManager.Instance.Player.Gain(Data.skipCoinCount);
            delayCloseTime = 0.5f;
            //GameManager.Instance.SkipReward(out _);
            StartCoroutine(CloseWindowIE());
            //Debug.LogWarning("获得金币!");
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
            Load(Data.offers, Data.title);
        }
    }
}
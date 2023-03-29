using System;
using Game;
using I2.Loc;
using Managers;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// 先设置offer的值，再enable gameobject
    /// </summary>
    public class OfferUI : MonoBehaviour
    {
        public Sprite PositiveImage;
        public Sprite PassiveImage;

        public Button targetButton;
        public Image Icon;
        public Localize Id;
        public Localize Prof;
        public Localize Positive;
        public Localize Description;

        public Image Bg;
        
        public Transform RankStar;
        
        public TMP_Text CostLabel;
        
        public int Cost;
        public bool IsGood;
        
        [ShowInInspector] 
        public Offer Offer;

        private System.Action onClick;

        //public Offer Offer 
        //{
        //    get 
        //    {
        //        return offer;
        //    }
        //}

        private void Start()
        {
            targetButton.onClick.AddListener(OnClick);
        }

        private void OnEnable()
        {
            UpdateData();
        }



        public void SetData(Offer offer, System.Action onClick)
        {
            this.Offer = offer;
            this.onClick = onClick;
            UpdateData();
        }


        private void OnClick() 
        {
            onClick?.Invoke();
        }

        public void UpdateData()
        {
            if ((Cost >= 0)&&(CostLabel != null))
            {
                CostLabel.text = Cost.ToString();
            }
            
            switch (Offer.Kind)
            {
                case Offer.OfferKind.Potion:
                    Id.SetTerm(Offer.Id);
                    break;
                case Offer.OfferKind.Skill:
                    Id.SetTerm(Offer.Id);
                    var skill = SkillManager.Instance.Lib[Offer.Id];
                    Positive?.SetTerm(skill.Positive? "positive" : "passive");
                    if (Bg != null)
                    {
                        Bg.sprite = skill.Positive ? PositiveImage : PassiveImage;
                    }

                    //Debug.Log(skill.Description);
                    Prof?.SetTerm(skill.Pool);
                    Description?.SetTerm(skill.Description);

                    if (RankStar != null)
                    {
                        foreach (Transform child in RankStar)
                        {
                        
                            if (child.GetSiblingIndex() > ((int)skill.Rank))
                            {
                                child.gameObject.SetActive(false);
                            }
                            else
                            {
                                child.gameObject.SetActive(true);
                            }
                        }
                    }
                    
                    if (CostLabel != null)
                    {
                        CostLabel.text = skill.CostInfo.Value == 0 ? skill.CostInfo.Value.ToString() : "";
                    }
                    break;
                case Offer.OfferKind.Gold:
                    Id.SetTerm(Offer.Gold.ToString());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
    }
}
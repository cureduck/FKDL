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
        public LocalizationParamsManager MaxLvParam;
        
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

        //private void OnEnable()
        //{
        //    UpdateData();
        //}



        public void SetData(Offer offer, System.Action onClick)
        {
            this.Offer = offer;
            //Debug.Log(offer.Kind);
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
            Debug.Log(Offer.Kind);
            switch (Offer.Kind)
            {
                case Offer.OfferKind.Potion:
                    Id.SetTerm(Offer.Id);
                    break;
                case Offer.OfferKind.Skill:
                    Id.SetTerm(Offer.Id);
                    var skill = SkillManager.Instance.GetById(Offer.Id);
                    Positive?.SetTerm(skill.Positive? "positive" : "passive");
                    if (Bg != null)
                    {
                        Bg.sprite = skill.Positive ? PositiveImage : PassiveImage;
                    }

                    //Debug.Log(skill.Description);
                    Prof?.SetTerm(skill.Pool);
                    Description?.SetTerm($"{skill.Id}_desc");
                    Description.GetComponent<LocalizationParamsManager>().SetParameterValue("P1", skill.Param1.ToString());
                    Description.GetComponent<LocalizationParamsManager>().SetParameterValue("P2", skill.Param2.ToString());
                    MaxLvParam?.SetParameterValue("P", skill.MaxLv.ToString());

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
                        CostLabel.text = skill.CostInfo.ActualValue == 0 ? skill.CostInfo.ActualValue.ToString() : "";
                    }
                    break;
                case Offer.OfferKind.Gold:
                    Id.SetTerm(Offer.Cost.ActualValue.ToString());
                    break;
                case Offer.OfferKind.Relic:
                    break;
                case Offer.OfferKind.Key:
                    break;
                case Offer.OfferKind.SkillSlot:
                    break;
                case Offer.OfferKind.NeedOnGetCheck:
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"未知类型：{Offer.Kind}");
            }
        }
    }
}
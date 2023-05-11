using System;
using Game;
using I2.Loc;
using Managers;
using Sirenix.OdinInspector;
using TMPro;
using Tools;
using UnityEngine;
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
        public Localize MaxLv;

        public Image Bg;

        public Transform RankStar;

        public TMP_Text CostLabel;

        public int Cost;
        public bool IsGood;

        [ShowInInspector] public Offer Offer;

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
            if (offer.Kind == Offer.OfferKind.Skill)
            {
                Icon.sprite = SkillManager.Instance.GetById(offer.Id).Icon;
            }

            this.onClick = onClick;
            UpdateData();
        }


        private void OnClick()
        {
            onClick?.Invoke();
        }

        private void SetRankStar(Rank rank)
        {
            if (RankStar != null)
            {
                foreach (Transform child in RankStar)
                {
                    if (child.GetSiblingIndex() > ((int)rank))
                    {
                        child.gameObject.SetActive(false);
                    }
                    else
                    {
                        child.gameObject.SetActive(true);
                    }
                }
            }
        }


        public void UpdateData()
        {
            if ((Cost >= 0) && (CostLabel != null))
            {
                CostLabel.text = Cost.ToString();
            }


            if (MaxLv != null || Offer.Kind == Offer.OfferKind.Skill)
            {
                MaxLv.gameObject.SetActive(false);
            }

            if (Prof != null)
            {
                Prof.gameObject.SetActive(Offer.Kind == Offer.OfferKind.Skill);
            }

            if (Positive != null)
            {
                Positive.gameObject.SetActive(Offer.Kind == Offer.OfferKind.Skill);
            }


            switch (Offer.Kind)
            {
                case Offer.OfferKind.Potion:
                    var potion = PotionManager.Instance.GetById(Offer.Id);
                    Id.SetTerm(Offer.Id);
                    Icon.sprite = potion.Icon;
                    SetRankStar(Offer.Rank);
                    Description?.SetTerm($"{potion.Id}_desc");
                    Description.SetLocalizeParam("P1", potion.Param1.ToString());
                    break;
                case Offer.OfferKind.Skill:
                    Id.SetTerm(Offer.Id);
                    var skill = SkillManager.Instance.GetById(Offer.Id);

                    if (MaxLv != null)
                    {
                        MaxLv.gameObject.SetActive(true);
                        MaxLv.SetTerm("maxLv");
                        MaxLv.SetLocalizeParam("P1", skill.MaxLv.ToString());
                    }

                    Positive?.SetTerm(skill.Positive ? "positive" : "passive");
                    if (Bg != null)
                    {
                        Bg.sprite = skill.Positive ? PositiveImage : PassiveImage;
                    }

                    //Debug.Log(skill.Description);
                    Prof?.SetTerm(skill.Prof);
                    Description?.SetTerm($"{skill.Id}_desc");
                    Description.SetLocalizeParam(
                        new string[] { "P1", "P2" },
                        new string[]
                        {
                            skill.Param1.ToString(),
                            skill.Param2.ToString()
                        });
                    Description.RemoveBetween();

                    SetRankStar(skill.Rank);

                    if (CostLabel != null)
                    {
                        CostLabel.text = skill.CostInfo.ActualValue == 0 ? skill.CostInfo.ActualValue.ToString() : "";
                    }

                    break;
                case Offer.OfferKind.Gold:
                    Id.SetTerm(Offer.Cost.ActualValue.ToString());
                    break;
                case Offer.OfferKind.Relic:
                    Id.SetTerm(Offer.Id);
                    Icon.sprite = RelicManager.Instance.GetById(Offer.Id).Icon;
                    Description?.SetTerm($"{Offer.Id}_desc");
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
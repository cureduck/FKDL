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
        
        [ShowInInspector] public Offer Offer;


        private void OnEnable()
        {
            UpdateData();
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
                        CostLabel.text = skill.Cost == 0 ? skill.Cost.ToString() : "";
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
using System;
using Game;
using I2.Loc;
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
        public Image Icon;
        public Localize Id;
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
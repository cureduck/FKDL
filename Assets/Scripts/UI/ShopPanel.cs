﻿using Game;
using Managers;
using UnityEngine;

namespace UI
{
    public class ShopPanel : MonoBehaviour
    {

        public OfferUI[] PotionList;
        public OfferUI[] SkillList;
        
        
        public void Refresh()
        {
            foreach (var good in PotionList)
            {
                good.transform.parent.gameObject.SetActive(true);
                good.Cost = Random.Range(7, 12);
                good.Offer.Kind = Offer.OfferKind.Potion;
                good.Offer.Id = PotionManager.Instance.Roll(RollForRank(), 1)[0];
                good.UpdateData();
            }

            foreach (var good in SkillList)
            {
                good.transform.parent.gameObject.SetActive(true);
                good.Cost = Random.Range(7, 12);
                good.Offer.Kind = Offer.OfferKind.Skill;
                good.Offer.Id = SkillManager.Instance.Roll(RollForRank(), 1)[0];
                good.UpdateData();
            }
        }

        public void Upgrade()
        {
            
        }



        private Rank RollForRank()
        {
            var f = Random.Range(0f, 1f);

            if (f < .2f) return Rank.Normal;
            if (f < .5f) return Rank.Uncommon;
            return Rank.Rare;
        }
        
    }
}
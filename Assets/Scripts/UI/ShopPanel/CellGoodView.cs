using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Game;
using Managers;
using I2.Loc;
using System;

public class CellGoodView : MonoBehaviour
{
    [SerializeField] private Color posstiveColor;
    [SerializeField] private Color unPosstiveColor;

    [SerializeField] private Image itemIcon;
    [SerializeField] private TMP_Text price_txt;
    [SerializeField] private Localize itemName_txt;
    [SerializeField] private GameObject soldOutSign;
    [SerializeField] private GameObject discountSign;
    [SerializeField] private PointEnterAndExit pointEnterAndExit;
    [SerializeField] private Button click_btn;

    private Offer offer;
    private int index;
    private System.Action<CellGoodView, Offer, int> onClick;

    private void Start()
    {
        pointEnterAndExit.onPointEnter.AddListener(OnPointEnter);
        pointEnterAndExit.onPointExit.AddListener(OnPointExit);
        click_btn.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        onClick?.Invoke(this, offer, index);
        OnPointExit();
    }

    public void SetData(int index, int curHaveGold, Offer offer, System.Action<CellGoodView, Offer, int> onClick)
    {
        this.offer = offer;
        this.index = index;
        this.onClick = onClick;


        discountSign.gameObject.SetActive(false);

        soldOutSign.gameObject.SetActive(offer.isSold);
        itemIcon.color = Color.white;
        if (offer.Kind == Offer.OfferKind.Skill)
        {
            Skill skill;
            SkillManager.Instance.TryGetById(offer.Id, out skill);
            itemIcon.sprite = skill.Icon;
            if (skill.Positive)
            {
                itemIcon.color = posstiveColor;
            }
            else
            {
                itemIcon.color = unPosstiveColor;
            }


            itemName_txt.SetTerm(skill.Id);
        }
        else if (offer.Kind == Offer.OfferKind.Potion)
        {
            Potion potion = PotionManager.Instance.GetById(offer.Id);
            itemIcon.sprite = potion.Icon;
            itemName_txt.SetTerm(potion.Id);
        }
        else if (offer.Kind == Offer.OfferKind.Key)
        {
            string temp;

            if (offer.Rank == Rank.Normal)
            {
                temp = "铜钥匙";
            }
            else if (offer.Rank == Rank.Uncommon)
            {
                temp = "银钥匙";
            }
            else
            {
                temp = "金钥匙";
            }

            itemName_txt.SetTerm(temp);
        }

        if (curHaveGold >= offer.Cost.ActualValue)
        {
            price_txt.text = offer.Cost.ActualValue.ToString();
        }
        else
        {
            price_txt.text = $"<color=red>{offer.Cost.ActualValue}</color>";
        }
    }

    private void OnPointEnter()
    {
        if (offer.Kind == Offer.OfferKind.Skill)
        {
            Skill skill;
            SkillManager.Instance.TryGetById(offer.Id, out skill);

            WindowManager.Instance.skillInfoPanel.Open(new SkillInfoPanel.Args02
                { screenPosition = itemIcon.transform.position, skill = skill });
        }
        else if (offer.Kind == Offer.OfferKind.Potion)
        {
            Potion potion = PotionManager.Instance.GetById(offer.Id);
            WindowManager.Instance.simpleInfoItemPanel.Open(new SimpleInfoItemPanel.Args
                { title = potion.Id, describe = potion.Des, screenPosition = itemIcon.transform.position });
        }
        else if (offer.Kind == Offer.OfferKind.Key)
        {
            string temp;

            if (offer.Rank == Rank.Normal)
            {
                temp = "铜钥匙";
            }
            else if (offer.Rank == Rank.Uncommon)
            {
                temp = "银钥匙";
            }
            else
            {
                temp = "金钥匙";
            }

            WindowManager.Instance.simpleInfoItemPanel.Open(new SimpleInfoItemPanel.Args
                { title = temp, describe = "一把钥匙", screenPosition = itemIcon.transform.position });
        }
    }

    private void OnPointExit()
    {
        if (offer.Kind == Offer.OfferKind.Skill)
        {
            WindowManager.Instance.skillInfoPanel.Close();
        }
        else if (offer.Kind == Offer.OfferKind.Potion || offer.Kind == Offer.OfferKind.Key)
        {
            WindowManager.Instance.simpleInfoItemPanel.Close();
        }
    }
}
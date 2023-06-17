using Game;
using I2.Loc;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CellGoodView : MonoBehaviour
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private TMP_Text price_txt;
    [SerializeField] private Localize itemName_txt;
    [SerializeField] private GameObject soldOutSign;
    [SerializeField] private GameObject discountSign;
    [SerializeField] private PointEnterAndExit pointEnterAndExit;
    [SerializeField] private Button click_btn;

    [Header("技能背景设置")] [SerializeField] private Image rankLevel_img;

    [SerializeField] private Sprite rank01Level;
    [SerializeField] private Sprite rank02Level;
    [SerializeField] private Sprite rank03Level;

    [Header("钥匙贴图")] [SerializeField] private Sprite keyIcon;

    [SerializeField] private Color level01KeyColor;
    [SerializeField] private Color level02KeyColor;
    [SerializeField] private Color level03KeyColor;
    private int index;

    private Offer offer;
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
            if (offer.Rank == Rank.Normal)
            {
                rankLevel_img.sprite = rank01Level;
            }
            else if (offer.Rank == Rank.Uncommon)
            {
                rankLevel_img.sprite = rank02Level;
            }
            else
            {
                rankLevel_img.sprite = rank03Level;
            }

            itemName_txt.SetTerm(skill.Id);
        }
        else if (offer.Kind == Offer.OfferKind.Potion)
        {
            Potion potion = PotionManager.Instance.GetById(offer.Id);
            if (offer.Rank == Rank.Normal)
            {
                rankLevel_img.sprite = rank01Level;
            }
            else if (offer.Rank == Rank.Uncommon)
            {
                rankLevel_img.sprite = rank02Level;
            }
            else
            {
                rankLevel_img.sprite = rank03Level;
            }

            itemIcon.sprite = potion.Icon;
            itemName_txt.SetTerm(potion.Id);
        }
        else if (offer.Kind == Offer.OfferKind.Key)
        {
            string temp;

            itemIcon.sprite = keyIcon;
            if (offer.Rank == Rank.Normal)
            {
                temp = "铜钥匙";
                //rankLevel_img.sprite = rank01Level;
                itemIcon.color = level01KeyColor;
            }
            else if (offer.Rank == Rank.Uncommon)
            {
                temp = "银钥匙";
                //rankLevel_img.sprite = rank02Level;
                itemIcon.color = level02KeyColor;
            }
            else
            {
                temp = "金钥匙";
                //rankLevel_img.sprite = rank03Level;
                itemIcon.color = level03KeyColor;
            }

            itemName_txt.SetTerm(temp);
        }
        else if (offer.Kind == Offer.OfferKind.Relic)
        {
            Relic relic = RelicManager.Instance.GetById(offer.Id);
            if (relic != null)
            {
                itemIcon.sprite = relic.Icon;
                itemName_txt.SetTerm(relic.Id);
            }
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
            Debug.Log("Enter!");
            Skill skill;
            SkillManager.Instance.TryGetById(offer.Id, out skill);

            WindowManager.Instance.skillInfoPanel.Open(new SkillInfoPanel.Args02
                { screenPosition = itemIcon.transform.position, skill = skill });
        }
        else if (offer.Kind == Offer.OfferKind.Potion)
        {
            //Debug.Log("开启！");
            Potion potion = PotionManager.Instance.GetById(offer.Id);
            WindowManager.Instance.simpleInfoItemPanel.Open(new SimpleInfoItemPanel.Args
            {
                title = potion.Id,
                describe = potion.Des,
                param1 = potion.Param1.ToString(),
                param2 = potion.Param2.ToString(),
                screenPosition = itemIcon.transform.position
            });
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
        else if (offer.Kind == Offer.OfferKind.Relic)
        {
            Relic relic = RelicManager.Instance.GetById(offer.Id);
            WindowManager.Instance.simpleInfoItemPanel.Open(new SimpleInfoItemPanel.Args
            {
                describe = $"{relic.Id}_desc",
                title = relic.Id,
                screenPosition = transform.position,
                param1 = relic.Param1.ToString(),
            });
        }
    }

    private void OnPointExit()
    {
        if (offer.Kind == Offer.OfferKind.Skill)
        {
            WindowManager.Instance.skillInfoPanel.Close();
        }
        else if (offer.Kind == Offer.OfferKind.Potion || offer.Kind == Offer.OfferKind.Key ||
                 offer.Kind == Offer.OfferKind.Relic)
        {
            //Debug.Log("关闭！");
            WindowManager.Instance.simpleInfoItemPanel.Close();
        }
    }
}
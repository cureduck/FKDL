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
        public Sprite rank01_img;
        public Sprite rank02_img;
        public Sprite rank03_img;

        public Image rankView;

        public Button targetButton;
        public PointEnterAndExit pointEvent;
        public Image Icon;
        public Localize Id;
        public Localize Prof;
        public TMP_Text rankLevel_txt;
        public Localize rankLevelInfo;
        public Localize Positive;
        public Localize Description;
        public Localize MaxLv;

        //public Image Bg;

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
            pointEvent.onPointEnter.AddListener(OnPointEnter);
            pointEvent.onPointExit.AddListener(OnPointExit);
        }

        private void OnPointEnter()
        {
            //寻找关键词
            if (this.Offer.Kind == Offer.OfferKind.Skill)
            {
                string totalDescribe = string.Empty;
                Skill skill = SkillManager.Instance.GetById(Offer.Id);
                foreach (var c in skill.Fs.Keys)
                {
                    string curInfo = GetTimingInfo(c);
                    if (!string.IsNullOrEmpty(curInfo))
                    {
                        totalDescribe += $"{curInfo}\n";
                    }
                }


                if (!string.IsNullOrEmpty(totalDescribe))
                {
                    WindowManager.Instance.simpleInfoItemPanel.Open(new SimpleInfoItemPanel.Args
                        { title = "关键词", screenPosition = transform.position, describe = totalDescribe });
                }
            }
        }

        private void OnPointExit()
        {
            if (this.Offer.Kind == Offer.OfferKind.Skill)
            {
                WindowManager.Instance.simpleInfoItemPanel.Close();
            }
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

        private void SetRankView(Rank rank)
        {
            switch (rank)
            {
                case Rank.Normal:
                    rankView.sprite = rank01_img;
                    rankLevelInfo.SetTerm("UI_Normal_RankInfo_01");
                    rankLevel_txt.color = new Color(1, 1, 1);
                    break;
                case Rank.Uncommon:
                    rankView.sprite = rank02_img;
                    rankLevelInfo.SetTerm("UI_Normal_RankInfo_02");
                    rankLevel_txt.color = new Color(36 / 255.0f, 176 / 255.0f, 143 / 255.0f);
                    break;
                default:
                    rankView.sprite = rank03_img;
                    rankLevelInfo.SetTerm("UI_Normal_RankInfo_03");
                    rankLevel_txt.color = new Color(255 / 255.0f, 163 / 255.0f, 0 / 255.0f);
                    break;
            }

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
                    SetRankView(Offer.Rank);
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
                    //if (Bg != null)
                    //{
                    //    Bg.sprite = skill.Positive ? PositiveImage : PassiveImage;
                    //}

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

                    SetRankView(skill.Rank);

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
                    var relic = RelicManager.Instance.GetById(Offer.Id);
                    if (relic != null)
                    {
                        Icon.sprite = relic.Icon;
                        Description?.SetTerm($"{Offer.Id}_desc");
                        Description?.SetLocalizeParam("P1", relic.Param1.ToString());
                    }

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

        private string GetTimingInfo(Timing timing)
        {
            switch (timing)
            {
                case Timing.OnHandleSkillInfo:
                    break;
                case Timing.OnGetSkillCost:
                    break;
                case Timing.BeforeAttack:
                    return "<color=yellow>交锋时</color>:第一次攻击时。";
                case Timing.OnAttack:
                    break;
                case Timing.OnStrike:
                    break;
                case Timing.OnReact:
                    break;
                case Timing.OnDefend:
                    break;
                case Timing.OnAttackSettle:
                    break;
                case Timing.OnDefendSettle:
                    break;
                case Timing.OnKill:
                    break;
                case Timing.OnRecover:
                    break;
                case Timing.OnLvUp:
                    break;
                case Timing.OnHeal:
                    break;
                case Timing.OnStrengthen:
                    break;
                case Timing.PotionEffect:
                    break;
                case Timing.SkillEffect:
                    break;
                case Timing.OnUsePotion:
                    break;
                case Timing.OnGetKey:
                    break;
                case Timing.OnUseKey:
                    break;
                case Timing.OnMarch:
                    break;
                case Timing.OnGain:
                    break;
                case Timing.OnApplied:
                    break;
                case Timing.OnApply:
                    break;
                case Timing.OnPurify:
                    break;
                case Timing.OnSetCoolDown:
                    break;
                case Timing.OnCounterCharge:
                    return "<color=yellow>反噬</color>:使用技能或道具损失生命值时。";
                case Timing.OnCost:
                    break;
                case Timing.OnGet:
                    break;
                case Timing.OnLose:
                    break;
                default:
                    break;
            }

            return string.Empty;
        }
    }
}
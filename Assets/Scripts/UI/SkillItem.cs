using System.Collections.Generic;
using Game;
using I2.Loc;
using Managers;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class SkillItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public Localize IdText;
        public TMP_Text LvText;
        public TMP_Text CdText;
        
        public GameObject DescriptionPanel;
        public Localize Description;
        public LocalizationParamsManager ParamsManager;
        
        public void Load(SkillData data)
        {
            Debug.LogWarning("该逻辑已被隐藏");
            return;
            if (data.IsEmpty)
            {
                IdText.SetTerm("empty");
                LvText.gameObject.SetActive(false);
                CdText.gameObject.SetActive(false);
                GetComponent<Button>().interactable = false;
            }
            else
            {
                LvText.gameObject.SetActive(true);
                IdText.SetTerm(data.Id);
                LvText.text = data.CurLv.ToString();


                if ((data.Cooldown > 0))
                {
                    CdText.gameObject.SetActive(true);
                    CdText.text = data.Cooldown.ToString();
                }
                else
                {
                    CdText.gameObject.SetActive(false);
                }
                
                
                if (data.CanCast)
                {
                    GetComponent<Button>().interactable = true;
                }
                else
                {
                    GetComponent<Button>().interactable = false;
                }
                
                /*if ((data.Bp.Positive) && (data.Cooldown > 0))
                {
                    CdText.gameObject.SetActive(true);
                    CdText.text = data.Cooldown.ToString();
                    GetComponent<Button>().interactable = false;
                }
                else
                {
                    if (!data.Bp.Positive)
                    {
                        GetComponent<Button>().interactable = false;
                    }
                    else
                    {
                        GetComponent<Button>().interactable = true;
                    }
                    CdText.gameObject.SetActive(false);
                }*/
            }
        }

        private Dictionary<string, Skill> Lib => SkillManager.Instance.Lib;
        
        [Button]
        public void Activate()
        {
            GetComponent<Animator>().SetTrigger("activate");
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            DescriptionPanel.gameObject.SetActive(true);
            if (Lib.ContainsKey(IdText.Term))
            {
                ParamsManager.SetParameterValue("P1", Lib[IdText.Term].Param1.ToString());
                ParamsManager.SetParameterValue("P2", Lib[IdText.Term].Param2.ToString());
                Description.SetTerm(Lib[IdText.Term].Description);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            DescriptionPanel.gameObject.SetActive(false);
        }
    }
}
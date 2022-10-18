﻿using System.Collections.Generic;
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
    public class SkillItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public Localize IdText;
        public TMP_Text LvText;

        public GameObject DescriptionPanel;
        public Localize Description;
        public LocalizationParamsManager ParamsManager;
        
        public void Load(SkillData data)
        {
            if (data.IsEmpty)
            {
                IdText.SetTerm("empty");
                LvText.gameObject.SetActive(false);
                GetComponent<Button>().interactable = false;
            }
            else
            {
                LvText.gameObject.SetActive(true);
                IdText.SetTerm(data.Id);
                LvText.text = data.CurLv.ToString();
                GetComponent<Button>().interactable = data.Bp.Positive;
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
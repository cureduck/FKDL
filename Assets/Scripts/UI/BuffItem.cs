using System;
using Game;
using Managers;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class BuffItem : MonoBehaviour
    {
        [ShowInInspector]
        public BuffData BuffData;
        public TMP_Text Lv;
        public Image Icon;

        private void Start()
        {
            Load();
        }

        public void Load()
        {
            Icon.sprite = BuffData.Bp?.Icon;
            Lv.text = BuffData.CurLv.ToString();
        }

        private void LoadSprite()
        {
            if (BuffData.Bp.Icon)
            {
                
            }
        }
    }
}
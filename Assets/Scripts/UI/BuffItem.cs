using System;
using Game;
using Managers;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace UI
{
    public class BuffItem : MonoBehaviour
    {
        [ShowInInspector]
        public BuffData BuffData;
        public TMP_Text Lv;


        private void Start()
        {
            Load();
        }

        public void Load()
        {
            Lv.text = BuffData.CurLv.ToString();
        }
    }
}
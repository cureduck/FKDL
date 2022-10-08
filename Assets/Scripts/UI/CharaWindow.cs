using System;
using Game;
using Managers;
using TMPro;
using UnityEngine;

namespace UI
{
    public class CharaWindow : Singleton<CharaWindow>
    {
        public TMP_Text Hp;
        public TMP_Text Sp;
        public TMP_Text Mp;
        public TMP_Text PAD;
        public TMP_Text MAD;


        private PlayerData _p;
        
        private void Start()
        {
            GameManager.Instance.GameLoaded += () =>
            {
                _p = GameManager.Instance.PlayerData;
                _p.OnUpdated += UpdateData;
            };
        }
        

        private void UpdateData()
        {
            Hp.text = _p.Status.CurHp + "/" + _p.Status.MaxHp;
            Sp.text = _p.PlayerStatus.CurSp + "/" + _p.PlayerStatus.MaxSp;
            Mp.text = _p.Status.CurMp + "/" + _p.Status.MaxMp;

            PAD.text = _p.Status.PAtk + "/" + _p.Status.PDef;
            MAD.text = _p.Status.MAtk + "/" + _p.Status.MDef;
        }
    }
}
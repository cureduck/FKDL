using System;
using I2.Loc;
using Managers;
using TMPro;
using UnityEngine;

namespace Game
{
    [ExecuteAlways]
    public class EnemySquare : Square<EnemySaveData>
    {
        public EnemyBp EnemyBp => EnemyManager.Instance.EnemyBps[Data.Id];

        protected override void Start()
        {
            base.Start();
            Sq.Set(Data);
            Sq.Id.SetTerm(EnemyBp.Id);
            Sq.Bonus.text = Data.CurHp + "/" + EnemyBp.Status.MaxHp;
            Sq.Clicked += Engage;
        }

        private void Engage()
        {
            Debug.Log("Engage!");
        }
    }
}
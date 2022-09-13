using System;
using Managers;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game
{
    [DisallowMultipleComponent]
    public class Square : MonoBehaviour
    {
        [ShowInInspector] public static Color EnemyColor;
        
        
        public SquareBase Sq;

        [ShowInInspector] public MapData Data;


        private void Start()
        {
            Sq = GetComponent<SquareBase>();
            Sq.SetSize(Data);
            UpdateFace();

            switch (Data)
            {
                case EnemySaveData d0:
                    Sq.OnFocus += () => WindowManager.Instance.Display(d0);
                    Sq.OnReact += () => BattleManager.Instance.Fight(this);
                    break;
                case CasinoSaveData d1:
                    break;
                case ChestSaveData d2:
                    break;
                case MountainSaveData d3:
                    break;
            }
            
        }


        public void UpdateFace()
        {
            switch (Data)
            {
                case EnemySaveData d0:
                    Sq.SetContent(d0.Id, d0.Status.CurHp +"/" + d0.Bp.Status.MaxHp, EnemyColor);
                    break;
                case CasinoSaveData d1:
                    Sq.SetContent("casino", d1.TimesLeft + "/" + CasinoSaveData.MaxTimes, EnemyColor);
                    break;
                case ChestSaveData d2:
                    Sq.SetContent("treasure", d2.Rank.ToString());
                    break;
                case MountainSaveData d3:
                    Sq.SetContent("mountain", d3.TimesLeft + "/" + MountainSaveData.MaxTimes);
                    break;
            }
        }
        
        

        private void OnDestroy()
        {
            try
            {
                GameManager.Instance.Map.Floors[GameManager.Instance.Map.CurrentFloor].Squares.Remove(Data);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
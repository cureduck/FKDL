using System;
using I2.Loc;
using Managers;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Game
{
    [DisallowMultipleComponent]
    public class Square : MonoBehaviour
    {
        [ShowInInspector] public static Color EnemyColor;
        
        [ShowInInspector] public MapData Data;


#if UNITY_EDITOR

        [JsonIgnore, ShowInInspector]
        public Placement Place
        {
            get => Data?.Placement ?? new Placement();
            set
            {
                Data.Placement = value;
                SetSize(value);
            }
        }
#endif
        
        
        private void Start()
        {
            SetSize(Data.Placement);

            switch (Data)
            {
                case EnemySaveData d0:
                    OnFocus += () => WindowManager.Instance.Display(d0);
                    OnReact += () => BattleManager.Instance.Fight(this);
                    if (GameManager.Instance.NewGame)
                    {
                        d0.Status = d0.Bp.Status;
                    }
                    break;
                case CasinoSaveData d1:
                    break;
                case ChestSaveData d2:
                    break;
                case MountainSaveData d3:
                    break;
            }
            UpdateFace();
        }


        public void UpdateFace()
        {
            switch (Data)
            {
                case EnemySaveData d0:
                    SetContent(d0.Id, d0.Status.CurHp +"/" + d0.Bp.Status.MaxHp, EnemyColor);
                    break;
                case CasinoSaveData d1:
                    SetContent("casino", d1.TimesLeft + "/" + CasinoSaveData.MaxTimes, EnemyColor);
                    break;
                case ChestSaveData d2:
                    SetContent("treasure", d2.Rank.ToString());
                    break;
                case MountainSaveData d3:
                    SetContent("mountain", d3.TimesLeft + "/" + MountainSaveData.MaxTimes);
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


        #region base
        public SpriteRenderer Sp;
        private const float Spacing = .06f;
        
        public SpriteRenderer Icon;
        public Localize Id;
        public TMP_Text Bonus;

        public Transform Global;

        public void SetSize(Placement d)
        {
            transform.position = new Vector3(d.x + Spacing/2, -d.y + Spacing/2, 0);
            transform.localScale = new Vector3(d.Width - Spacing, d.Height - Spacing, 0);
            Global.localScale = new Vector3(1/(d.Width -Spacing), 1/(d.Height - Spacing));
        }

        public void SetContent(string id, string text, Color color = default)
        {
            Id.SetTerm(id);
            Bonus.text = text;
        }
        

        public event Action OnFocus;
        public event Action OnReact;
        
        public void Focus()
        {
            OnFocus?.Invoke();
        }

        public void React()
        {
            OnReact?.Invoke();
        }
        #endregion
    }
}
﻿using System;
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

                    break;
                case CasinoSaveData d1:
                    break;
                case ChestSaveData d2:
                    break;
                case MountainSaveData d3:
                    break;
            }

            Data.OnDestroy += OnSquareDestroy;
            Data.OnUpdated += UpdateFace;
            
            if (GameManager.Instance.NewGame)
            {
                Data.Init();
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
                case RockSaveData d4:
                    SetContent("rock", d4.Cost.ToString());
                    break;
                case DoorSaveData d5:
                    SetContent("door", d5.Rank.ToString());
                    break;
                case KeySaveData d6:
                    SetContent("key", d6.Rank.ToString());
                    break;
                case CrystalSaveData d7:
                    SetContent("crystal", "");
                    break;
                case ObsidianSaveData d8:
                    SetContent("","");
                    break;
                case SupplySaveData d9:
                    switch (d9.Type)
                    {
                        case SupplyType.Spring:
                            SetContent("spring", d9.Rank.ToString());
                            break;
                        case SupplyType.Grassland:
                            SetContent("grassland", d9.Rank.ToString());
                            break;
                        case SupplyType.Camp:
                            SetContent("camp", d9.Rank.ToString());
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
                case ShopSaveData d10:
                    SetContent("shop", "");
                    break;
                case StairsSaveData d11:
                    SetContent("stairs", d11.Destination);
                    break;
                case StartSaveData d12:
                    SetContent("start", "");
                    break;
            }
        }
        

        private void OnSquareDestroy()
        {
            try
            {
                GameManager.Instance.Map.Floors[GameManager.Instance.Map.CurrentFloor].Squares.Remove(Data);
                Data = null;
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
            DestroyImmediate(gameObject);
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
            Data.OnFocus();
        }

        public void React()
        {
            OnReact?.Invoke();
        }
        #endregion
    }
}
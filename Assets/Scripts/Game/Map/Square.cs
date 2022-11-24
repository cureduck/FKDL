using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using I2.Loc;
using Managers;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

namespace Game
{
    [DisallowMultipleComponent]
    public class Square : SerializedMonoBehaviour
    {
        [ShowInInspector] public static Color EnemyColor;
        [ShowInInspector] public static Color DoneColor;
        
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
            
            Data.OnDestroy += OnSquareDestroy;
            Data.OnUpdated += UpdateFace;
            //Data.RevealAround += RevealAround;
            
            Data.Load();
            
            UpdateFace();
        }


        public void UpdateFace()
        {
            /*if (!Data.Revealed)
            {
                SetContent("null", "");
                return;
            }*/

            switch (Data.SquareState)
            {
                case SquareState.UnRevealed:
                    SetContent("empty", "", new Color(.5f, .5f, .5f, .8f), null);
                    return;
                case SquareState.Done:
                    SetContent("empty", "", new Color(.5f, .5f, .5f, .6f), null);
                    return;
            }


            var lib = SpriteManager.Instance.BuffIcons;
            
            switch (Data)
            {
                case EnemySaveData d0:
                    SetContent(d0.Id.ToLower(), d0.Status.CurHp +"/" + d0.Bp.Status.MaxHp, EnemyColor, lib["boss"]);
                    break;
                case CasinoSaveData d1:
                    SetContent("casino", d1.TimesLeft + "/" + CasinoSaveData.MaxTimes, EnemyColor, lib["casino"]);
                    break;
                case ChestSaveData d2:
                    SetContent("treasure", d2.Rank.ToString(), icon: lib["chest"]);
                    break;
                case MountainSaveData d3:
                    SetContent("mountain", d3.TimesLeft + "/" + MountainSaveData.MaxTimes, icon: lib["mountain"]);
                    break;
                case RockSaveData d4:
                    SetContent("rock", d4.Cost.ToString());
                    break;
                case DoorSaveData d5:
                    SetContent("door", d5.Rank.ToString(), icon: lib["door"]);
                    break;
                case KeySaveData d6:
                    SetContent("key", d6.Rank.ToString());
                    break;
                case CrystalSaveData d7:
                    SetContent("crystal", "", icon: lib["crystal"]);
                    break;
                case ObsidianSaveData d8:
                    SetContent("obsidian","");
                    break;
                case SupplySaveData d9:
                    switch (d9.Type)
                    {
                        case SupplyType.Spring:
                            SetContent("spring", d9.Rank.ToString(), icon: lib["spring"]);
                            break;
                        case SupplyType.Grassland:
                            SetContent("grassland", d9.Rank.ToString(), icon: lib["grassland"]);
                            break;
                        case SupplyType.Camp:
                            SetContent("camp", d9.Rank.ToString(), icon: lib["camp"]);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
                case ShopSaveData d10:
                    SetContent("shop", "", icon: lib["shop"]);
                    break;
                case StairsSaveData d11:
                    SetContent("stairs", d11.Destination, icon: lib["stairs"]);
                    break;
                case StartSaveData d12:
                    SetContent("play", "");
                    var tmp = transform.position;
                    tmp.z = -10;
                    Camera.main.transform.position = tmp;
                    break;
                case TravellerSaveData d13:
                    SetContent("traveler", "");
                    break;
                case GoldSaveData d14:
                    SetContent("gold", d14.Count.ToString(), icon: lib["gold"]);
                    break;
            }
        }
        

        private void OnSquareDestroy()
        {
            try
            {
                UpdateFace();
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
            //DestroyImmediate(gameObject);
        }


        #region base
        public SpriteRenderer Bg;
        public SpriteRenderer Box;
        private const float Spacing = .02f;
        
        public SpriteRenderer Icon;
        public Localize Id;
        public TMP_Text Bonus;

        //public RectTransform Global;

        public void SetSize(Placement d)
        {
            transform.position = new Vector3(d.x + Spacing/2, -d.y + Spacing/2, 0);
            //transform.localScale = new Vector3(d.Width - Spacing, d.Height - Spacing, 0);
            
            Bg.transform.localScale = new Vector3(d.Width - Spacing, d.Height -Spacing, 0);
            Box.size = new Vector2(d.Width - Spacing, d.Height - Spacing);
            
            
            Id.GetComponent<RectTransform>().localPosition = new Vector3((d.Width-Spacing)/2, -d.Height/2f +.6f, 0);
            Bonus.GetComponent<RectTransform>().localPosition = new Vector3((d.Width-Spacing)/2, -d.Height/2f - .6f, 0);
            Icon.GetComponent<RectTransform>().localPosition = new Vector3((d.Width-Spacing)/2, -d.Height/2f, 0);

            
            if (d.Height == 1)
            {
                Id.GetComponent<RectTransform>().localPosition -= new Vector3(0, .2f, 0);
                Bonus.GetComponent<RectTransform>().localPosition -= new Vector3(0, -.2f, 0);
            }
            
            //Global.sizeDelta = new Vector2(d.Width - Spacing, d.Height - Spacing);
        }

        public void SetContent(string id, string text, Color color = default, Sprite icon = null)
        {
            Id.SetTerm(id);
            Bonus.text = text;
            /*if (color == default)
            {
                color = Color.gray;
            }
            Sp.color = color;
            */
            Icon.sprite = icon;
        }


        [Button]
        public void Focus()
        {
            UniTask.WhenAll(OnFocus());
        }
        
        
        public void UnFocus()
        {
            UniTask.WhenAll(OnUnFocus());
        }

        private async UniTask OnFocus()
        {
            var f = .2f;
            var f2 = -.6f;
            var pos = transform.position;
            for (var i = 0f; i < 1f; i += .04f)
            {
                pos.z = math.lerp(0, f, i);
                transform.position = pos;
                await UniTask.NextFrame();
            }
            
            for (var i = 0f; i <= 1f; i += .02f)
            {
                pos.z = math.lerp(0, f2, i);
                transform.position = pos;
                await UniTask.NextFrame();
            }
        }
        
        private async UniTask OnUnFocus()
        {
            var f = transform.position.z;
            var pos = transform.position;
            for (var i = 0f; i <= 1f; i += .04f)
            {
                pos.z = math.lerp(f, 0, i);
                transform.position = pos;
                await UniTask.NextFrame();
            }
        }


        private void OnDestroy()
        {
            UnFocus();
            if (Data != null)
            {
                Data.OnDestroy -= OnSquareDestroy;
                Data.OnUpdated -= UpdateFace;
            }
        }
        #endregion
    }
}
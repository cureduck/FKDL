using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using I2.Loc;
using Managers;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.UI;

namespace Game
{
    [DisallowMultipleComponent]
    public partial class Square : SerializedMonoBehaviour
    {
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

            /*switch (Data.SquareState)
            {
                case SquareState.UnRevealed:
                    _animator.SetTrigger("UnReveal");
                    SetContent("empty", "", null);
                    var t = Mask.color;
                    t.a = 1f;
                    Mask.color = t;
                    return;
                case SquareState.Done:
                    _animator.SetTrigger("Done");
                    //SetContent("empty", "", new Color(.5f, .5f, .5f, .6f), null);
                    if (GameManager.Instance.Focus != this)
                    {
                        Mask.gameObject.SetActive(true);
                    }
                    var t2 = Mask.color;
                    t2.a = .95f;
                    Mask.color = t2;
                    break;
                case SquareState.Revealed:
                    Box.gameObject.SetActive(true);
                    Mask.gameObject.SetActive(false);
                    break;
                
            }*/

            switch (Data.SquareState)
            {
                case SquareState.UnRevealed:
                    //_animator.SetTrigger("Unreveal");
                    break;
                case SquareState.Focus:
                    OnFocus();
                    //_animator.SetTrigger("Focus");
                    break;
                case SquareState.UnFocus:
                    OnReveal();
                    //_animator.SetTrigger("UnFocus");
                    break;
                case SquareState.Done:
                    if (GameManager.Instance.Focus != this)
                    {
                        OnDone();
                        //_animator.SetTrigger("Done");
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            

            var lib = SpriteManager.Instance.BuffIcons;
            
            switch (Data)
            {
                case EnemySaveData d0:
                    var icon = "";
                    
                    if (SpriteManager.Instance.BuffIcons.TryGetValue(d0.Id, out _))
                    {
                        icon = d0.Id;
                    }
                    else
                    {
                        switch (d0.Bp.Rank)
                        {
                            case Rank.Normal:
                                icon = "soldier";
                                break;
                            case Rank.Uncommon:
                                icon = "elite";
                                break;
                            case Rank.Rare:
                                icon = "boss";
                                break;
                        }
                    }
                    
                    SetContent(d0.Id.ToLower(), d0.Status.CurHp +"/" + d0.Bp.Status.MaxHp, lib[icon]);
                    break;
                case CasinoSaveData d1:
                    SetContent("casino", d1.TimesLeft + "/" + CasinoSaveData.MaxTimes, lib["casino"]);
                    break;
                case ChestSaveData d2:
                    SetContent("treasure", d2.Rank.ToString(), icon: lib["chest"]);
                    break;
                case CemeterySaveData d3:
                    SetContent("cemetery", d3.TimesLeft + "/" + CemeterySaveData.MaxTimes, icon: lib["cemetery"]);
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
                    CameraMan.Instance.Target = transform.position;
                    break;
                case TravellerSaveData d13:
                    SetContent("traveler", "");
                    break;
                case GoldSaveData d14:
                    SetContent("gold", d14.Count.ToString(), icon: lib["gold"]);
                    break;
                case TotemSaveData d15:
                    SetContent("totem", "");
                    break;
            }
        }
        

        private void OnSquareDestroy()
        {
            try
            {
                UpdateFace();
                //UnFocus();
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
        public SpriteRenderer Mask;
        
        private const float Spacing = .02f;
        
        public SpriteRenderer Icon;
        public Localize Id;
        public TMP_Text Bonus;

        //private Animator _animator;

        private void Awake()
        {
            //_animator = GetComponent<Animator>();
        }

        //public RectTransform Global;

        public void SetSize(Placement d)
        {
            transform.position = new Vector3(d.x + Spacing/2, -d.y + Spacing/2, 0);
            //transform.localScale = new Vector3(d.Width - Spacing, d.Height - Spacing, 0);
            
            Bg.transform.localScale = new Vector3(d.Width - Spacing, d.Height -Spacing, 0);
            Bg1.localScale = new Vector3(d.Width - Spacing, d.Height -Spacing, 0);
            Bg2.localScale = new Vector3(d.Width - Spacing, d.Height -Spacing, 0);
            Mask.transform.localScale = new Vector3(d.Width - Spacing, d.Height -Spacing, 0);
            
            Box.size = new Vector2(d.Width - Spacing, d.Height - Spacing);
            
            
            Id.GetComponent<RectTransform>().localPosition = new Vector3((d.Width-Spacing)/2, -d.Height/2f +.6f, -0.01f);
            Bonus.GetComponent<RectTransform>().localPosition = new Vector3((d.Width-Spacing)/2, -d.Height/2f - .6f, -0.01f);
            
            Icon.GetComponent<RectTransform>().localPosition = new Vector3((d.Width-Spacing)/2, -d.Height/2f, 0);

            
            if (d.Height == 1)
            {
                Id.GetComponent<RectTransform>().localPosition -= new Vector3(0, .2f, 0);
                Bonus.GetComponent<RectTransform>().localPosition -= new Vector3(0, -.2f, 0);
            }
            
            //Global.sizeDelta = new Vector2(d.Width - Spacing, d.Height - Spacing);
        }


        public Light2D[] Light2D;
        
        public void SetContent(string id, string text, Sprite icon = null)
        {
            name = $"{id}, {Place}";
            Id.SetTerm(id);
            Bonus.text = text;
            Icon.sprite = icon;

            if (icon != null)
            {
                if (GameManager.Instance.SquareColors.TryGetValue(icon.name, out var L))
                {
                    for (int i = 0; i < Light2D.Length; i++)
                    {
                        Light2D[i].color = L;
                    }
                }
                else
                {
                    for (int i = 0; i < Light2D.Length; i++)
                    {
                        Light2D[i].color = GameManager.Instance.SquareColors["elite"];
                    }
                }
            }
        }


        public void OnReact()
        {
            if (Data is EnemySaveData d)
            {
                GameObject curEffectObject = ObjectPoolManager.Instance.SpawnAttackEffect();
                curEffectObject.transform.position = Icon.transform.position;
                
                AudioPlayer.Instance.Play(AudioPlayer.AudioNormalAttack);
                
                EnemyBp enemyBp = d.Bp;
                if (enemyBp != null && enemyBp.Rank >= Rank.Rare)
                {
                    AudioPlayer.Instance.SwitchBossOrNormalBGM(false);
                }
                else
                {
                    AudioPlayer.Instance.SwitchBossOrNormalBGM(true);
                }
            }
        }
        


        [Button]
        public void Focus()
        {
            Data.SquareState = SquareState.Focus;
            OnFocus();
            //_animator.SetTrigger("Focus");
        }
        
        
        public virtual void UnFocus()
        {
            if (Data.SquareState == SquareState.Done)
            {
                return;
            }
            Data.SquareState = SquareState.UnFocus;
            OnReveal();
            //_animator.SetTrigger("UnFocus");
        }

        private Sequence _sequence;

        private const float DownTime = .3f;
        private const float UpTime = .4f;

        private Tween _breath;
        
        private void OnFocus()
        {
            _sequence.Kill();
            _sequence = DOTween.Sequence();
            
            Bonus.gameObject.SetActive(true);
            Mask.gameObject.SetActive(false);


            if (!(Data is ObsidianSaveData))
            {
                _breath.Kill();
                _breath = DOTween.To(
                    () => { return Light2D[0].intensity; },
                    (value => Light2D[0].intensity = value),
                    2, 3f).SetLoops(-1, LoopType.Yoyo);
            }


            _sequence.Append(transform.DOMoveZ(.15f, DownTime))
                .Append(transform.DOMoveZ(-.3f, UpTime))
                .Insert(0, Bg1.transform.DOLocalMoveZ(0, UpTime))
                .Insert(0, Bg2.transform.DOLocalMoveZ(0, UpTime))
                .Insert(DownTime, Bg1.transform.DOLocalMoveZ(.15f, UpTime))
                .Insert(DownTime, Bg2.transform.DOLocalMoveZ(.3f, UpTime))
                .OnComplete((() => {
                    if (Data.SquareState == SquareState.Done)
                    {
                        OnDone();
                    }
                }));
        }

        private void OnReveal()
        {
            Bonus.gameObject.SetActive(true);
            
            _sequence.Kill();
            _sequence = DOTween.Sequence();
            _sequence.Append(transform.DOMoveZ(0f, .2f))
                .Insert(0f, Bg1.transform.DOLocalMoveZ(0f, UpTime))
                .Insert(0f, Bg2.transform.DOLocalMoveZ(0f, UpTime))
                .Insert(0f, Mask.GetComponent<SpriteRenderer>().DOFade(0, .3f))
                .OnComplete(() => Mask.gameObject.SetActive(false));
        }
        
        
        
        private void OnDone()
        {
            _sequence.Kill();
            _sequence = DOTween.Sequence();
            
            Mask.gameObject.SetActive(true);
            Bonus.gameObject.SetActive(false);
            Id.gameObject.SetActive(false);
            
            _sequence.Append(transform.DOMoveZ(0f, .4f))
                .Insert(0f, Bg1.transform.DOLocalMoveZ(0f, UpTime))
                .Insert(0f, Bg2.transform.DOLocalMoveZ(0f, UpTime))
                .Insert(0f, Mask.GetComponent<SpriteRenderer>().DOFade(.7f, UpTime))
                .OnComplete(() => { _breath.Kill(); Light2D[0].intensity = 0; });
        }
        






        public Transform Bg1;
        public Transform Bg2;


        public override string ToString()
        {
            return Icon.sprite.name;
        }


        private void OnDestroy()
        {
            if (Data != null)
            {
                Data.OnDestroy -= OnSquareDestroy;
                Data.OnUpdated -= UpdateFace;
            }
        }
        #endregion
    }
}
using System;
using DG.Tweening;
using I2.Loc;
using Managers;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using TMPro;
using Tools;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

namespace Game
{
    [DisallowMultipleComponent]
    public sealed partial class Square : SerializedMonoBehaviour
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


            //Data.RevealAround += RevealAround;

            Data.Load();

            UpdateFace();
        }

        private void Bind(MapData mapData)
        {
            Data.OnDestroy += OnSquareDestroy;
            Data.OnUpdated += UpdateFace;
            Data.ReactResultInfo += HandleReactResultArgs;
        }

        public void Reload(MapData mapData)
        {
            _sequence.Kill();
            _breath.Kill();
            Mask.gameObject.SetActive(true);
            gameObject.SetActive(true);
            Id.gameObject.SetActive(true);
            UnbindCurrent();
            Data = mapData;
            SetSize(Data.Placement);
            Bind(mapData);
            UpdateFace();
            _breathLight.intensity = 0;
        }


        public void UpdateFace()
        {
            switch (Data.SquareState)
            {
                case SquareState.UnRevealed:
                    //_animator.SetTrigger("Unreveal");
                    OnUnReveal();
                    break;
                case SquareState.Focus:
                    OnFocusAnimator();
                    //_animator.SetTrigger("Focus");
                    break;
                case SquareState.UnFocus:
                    OnReveal();
                    //_animator.SetTrigger("UnFocus");
                    break;
                case SquareState.Done:
                    OnDone();
                    /*if (GameManager.Instance.Focus != this)
                    {
                        OnDone();
                        //_animator.SetTrigger("Done");
                    }*/

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

                    SetContent(d0.Id.ToLower(), d0.Status.ToString("short"), lib[icon]);
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
                    switch (d5.Rank)
                    {
                        case Rank.Normal:
                            SetContent("copper door", " ", icon: lib["door"]);
                            break;
                        case Rank.Uncommon:
                            SetContent("silver door", " ", icon: lib["door"]);
                            break;
                        case Rank.Rare:
                            SetContent("gold door", " ", icon: lib["door"]);
                            break;
                        default:
                            break;
                    }

                    break;
                case KeySaveData d6:
                    SetContent("key", d6.KeyRank.ToString());
                    break;
                case CrystalSaveData d7:
                    SetContent("crystal", "", icon: lib["crystal"]);
                    break;
                case ObsidianSaveData d8:
                    SetContent(" ", "");
                    break;
                case SupplySaveData d9:
                    switch (d9.Type)
                    {
                        case SupplyType.Spring:
                            SetContent("spring", d9.Rank.ToString(RankDescType.Size), icon: lib["spring"]);
                            break;
                        case SupplyType.Grassland:
                            SetContent("grassland", d9.Rank.ToString(RankDescType.Size), icon: lib["grassland"]);
                            break;
                        case SupplyType.Camp:
                            SetContent("camp", d9.Rank.ToString(RankDescType.Size), icon: lib["camp"]);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    break;
                case ShopSaveData d10:
                    SetContent("shop", " ", icon: lib["shop"]);
                    break;
                case StairsSaveData d11:
                    SetContent("stairs", d11.Destination, icon: lib["stairs"]);
                    break;
                case StartSaveData d12:
                    SetContent("play", " ");
                    CameraMan.Instance.Target = transform.position;
                    break;
                case TravellerSaveData d13:
                    SetContent("traveler", "", icon: lib["traveller"]);
                    break;
                case GuineasSaveData d14:
                    SetContent("guineas", d14.Value.ToString(), icon: lib["gold"]);
                    break;
                case TotemSaveData d15:
                    SetContent("totem", "", icon: lib["totem"]);
                    break;
            }
        }


        #region base

        public SpriteRenderer Bg;
        public SpriteRenderer OutLine;
        public SpriteRenderer Mask;

        private const float Spacing = .02f;

        public SpriteRenderer Icon;
        public Localize Id;
        public TMP_Text Bonus;

        public SpriteRenderer OutLine1;
        public SpriteRenderer OutLine2;


        //private Animator _animator;

        private void Awake()
        {
            //_animator = GetComponent<Animator>();
        }

        //public RectTransform Global;

        private const float Normal = 2.5f;
        private const float Large = 3.5f;

        private void SetSize(Placement d)
        {
            transform.position = new Vector3(d.x + Spacing / 2, -d.y + Spacing / 2, 0);
            //transform.localScale = new Vector3(d.Width - Spacing, d.Height - Spacing, 0);

            Bg.transform.localScale = new Vector3(d.Width - Spacing, d.Height - Spacing, 0);
            Bg1.localScale = new Vector3(d.Width - Spacing, d.Height - Spacing, 0);
            Bg2.localScale = new Vector3(d.Width - Spacing, d.Height - Spacing, 0);
            OutLine1.transform.localScale = new Vector3(1 / (d.Width - Spacing), 1 / (d.Height - Spacing), 0);
            OutLine2.transform.localScale = new Vector3(1 / (d.Width - Spacing), 1 / (d.Height - Spacing), 0);
            OutLine1.size = new Vector2(d.Width - Spacing, d.Height - Spacing);
            OutLine2.size = new Vector2(d.Width - Spacing, d.Height - Spacing);


            Mask.transform.localScale = new Vector3(d.Width - Spacing, d.Height - Spacing, 0);

            OutLine.size = new Vector2(d.Width - Spacing, d.Height - Spacing);

            Id.GetComponent<TMP_Text>().fontSize = Normal;
            //Bonus.GetComponent<TMP_Text>().fontSize = Normal;

            if ((d.Height == 1) || (d.Width == 1))
            {
                Id.GetComponent<RectTransform>().localPosition =
                    new Vector3((d.Width - Spacing) / 2, -d.Height / 2f + .3f, -0.01f);
                Bonus.GetComponent<RectTransform>().localPosition =
                    new Vector3((d.Width - Spacing) / 2, -d.Height / 2f - .2f, -0.01f);
                //Icon.GetComponent<RectTransform>().localPosition = new Vector3((d.Width-Spacing)/2, -d.Height/2f, 0);
                return;
            }

            if ((d.Height >= 3) && (d.Width >= 3))
            {
                Id.GetComponent<TMP_Text>().fontSize = Large;
                //Bonus.GetComponent<TMP_Text>().fontSize = Large;
                Id.GetComponent<RectTransform>().localPosition =
                    new Vector3((d.Width - Spacing) / 2, -d.Height / 2f + 1f, -0.01f);
                Bonus.GetComponent<RectTransform>().localPosition =
                    new Vector3((d.Width - Spacing) / 2, -d.Height / 2f - 1f, -0.01f);
                Icon.GetComponent<RectTransform>().localPosition =
                    new Vector3((d.Width - Spacing) / 2, -d.Height / 2f, 0);
                Icon.transform.localScale = new Vector3(1.6f, 1.6f, 0);
                return;
            }

            Id.GetComponent<RectTransform>().localPosition =
                new Vector3((d.Width - Spacing) / 2, -d.Height / 2f + .6f, -0.01f);
            Bonus.GetComponent<RectTransform>().localPosition =
                new Vector3((d.Width - Spacing) / 2, -d.Height / 2f - .6f, -0.01f);
            Icon.GetComponent<RectTransform>().localPosition = new Vector3((d.Width - Spacing) / 2, -d.Height / 2f, 0);
            Icon.transform.localScale = new Vector3(1f, 1f, 0);


            //Global.sizeDelta = new Vector2(d.Width - Spacing, d.Height - Spacing);
        }


        public Light2D[] Light2D;

        public void SetContent(string id, string text, Sprite icon = null)
        {
            name = $"{id}, {Data.Placement}";
            Id.SetTerm(id);
            Bonus.text = text;
            if (!text.IsNullOrWhitespace())
            {
                Bonus.GetComponent<Localize>().SetTerm(text.ToLower());
            }
            else
            {
                Bonus.text = "";
            }

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
                    var color = Color.red;
                    if (Data is EnemySaveData e)
                    {
                        switch (e.Bp.Rank)
                        {
                            case Rank.Normal:
                                color = GameManager.Instance.SquareColors["soldier"];
                                break;
                            case Rank.Uncommon:
                                color = GameManager.Instance.SquareColors["elite"];
                                break;
                            case Rank.Rare:
                                color = GameManager.Instance.SquareColors["boss"];
                                break;
                            default:
                                color = GameManager.Instance.SquareColors["soldier"];
                                e.Bp.Rank = Rank.Normal;
                                break;
                        }

                        for (int i = 0; i < Light2D.Length; i++)
                        {
                            Light2D[i].color = color;
                        }
                    }
                }
            }
        }


        [Button]
        public void Focus()
        {
            Data.SquareState = SquareState.Focus;
            OnFocusAnimator();
            OnFocusLogic();
            //_animator.SetTrigger("Focus");
        }


        public void UnFocus()
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
        [SerializeField] private Light2D _breathLight;
        [SerializeField] private Light2D _pointerOverLight;

        private void OnFocusAnimator()
        {
            if (Data.SquareState == SquareState.Done) return;

            _sequence.Kill();
            _sequence = DOTween.Sequence();

            Bonus.gameObject.SetActive(true);
            Mask.gameObject.SetActive(false);


            if (!(Data is ObsidianSaveData))
            {
                _breath.Kill();
                _breathLight.intensity += .4f;
                _breath = DOTween.To(
                    () => _breathLight.intensity,
                    (value => _breathLight.intensity = value),
                    2, 3f).SetLoops(-1, LoopType.Yoyo);
            }


            _sequence.Append(transform.DOMoveZ(.15f, DownTime))
                .Append(transform.DOMoveZ(-.3f, UpTime))
                .Insert(0, Bg1.transform.DOLocalMoveZ(0, UpTime))
                .Insert(0, Bg2.transform.DOLocalMoveZ(0, UpTime))
                .Insert(DownTime, Bg1.transform.DOLocalMoveZ(.15f, UpTime))
                .Insert(DownTime, Bg2.transform.DOLocalMoveZ(.3f, UpTime))
                .OnComplete((() =>
                {
                    if (Data.SquareState == SquareState.Done)
                    {
                        OnDone();
                    }
                }));
        }


        private void OnUnReveal()
        {
            _breath.Kill();
            _sequence.Kill();
            Mask.gameObject.SetActive(true);
            Bonus.gameObject.SetActive(true);
            Id.gameObject.SetActive(true);
            Mask.GetComponent<SpriteRenderer>().DOFade(1, .1f);
        }

        private void OnReveal()
        {
            if (Data.SquareState == SquareState.Done) return;

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
            OnLogicDone();
            _sequence.Kill();
            _sequence = DOTween.Sequence();
            _breath.Kill();

            Mask.gameObject.SetActive(true);
            Bonus.gameObject.SetActive(false);
            Id.gameObject.SetActive(false);

            _sequence.Append(transform.DOMoveZ(0f, .4f))
                .Insert(0f, Bg1.transform.DOLocalMoveZ(0f, UpTime))
                .Insert(0f, Bg2.transform.DOLocalMoveZ(0f, UpTime))
                .Insert(0f, Mask.GetComponent<SpriteRenderer>().DOFade(.7f, UpTime))
                .Insert(0f,
                    DOTween.To(
                        () => _breathLight.intensity,
                        (value => _breathLight.intensity = value),
                        0, UpTime))
                .OnComplete(() =>
                {
                    _breathLight.intensity = 0;
                    _pointerOverLight.intensity = 0;
                });
        }


        public Transform Bg1;
        public Transform Bg2;


        public override string ToString()
        {
            return Icon.sprite.name;
        }

        public void UnbindCurrent()
        {
            if (Data != null)
            {
                Data.OnDestroy -= OnSquareDestroy;
                Data.OnUpdated -= UpdateFace;
                Data.ReactResultInfo -= HandleReactResultArgs;
            }
        }


        private void OnDestroy()
        {
            UnbindCurrent();
        }

        #endregion
    }
}
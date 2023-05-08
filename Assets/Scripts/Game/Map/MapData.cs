using System;
using Managers;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using Stateless;
using UI;
using Unity.Mathematics;
using UnityEngine;

namespace Game
{
    public class MapData : IUpdateable, ICloneable
    {
        public Placement Placement;
        public SquareState SquareState = SquareState.UnRevealed;

        [JsonIgnore] protected PlayerData Player => GameManager.Instance.PlayerData;
        [JsonIgnore] protected SecondaryData SData => GameDataManager.Instance.SecondaryData;

        public event Action<Args> ReactResultInfo;

        /*
        [ShowInInspector, JsonIgnore]
        private StateMachine<SquareState, SquareChangeTrigger> sm;


        public MapData()
        {
            sm = new StateMachine<SquareState, SquareChangeTrigger>(
                (() => SquareState),
                s => SquareState = s
                );

            sm.Configure(SquareState.UnRevealed)
                .Permit(SquareChangeTrigger.UnFocus, SquareState.UnFocus)
                .Permit(SquareChangeTrigger.Focus, SquareState.Focus);

            sm.Configure(SquareState.UnFocus)
                .Permit(SquareChangeTrigger.Focus, SquareState.Focus);
            
            sm.Configure(SquareState.Done)
                .Permit()
        }
        */


        /// <summary>
        /// 新游戏时调用
        /// </summary>
        public virtual void Init()
        {
        }

        /// <summary>
        /// 加载游戏时调用
        /// </summary>
        public virtual void Load()
        {
        }

        public virtual void OnFocus()
        {
        }

        public virtual void OnReact()
        {
        }

        public virtual void OnLeave()
        {
        }

        public event Action OnDestroy;
        public event Action OnUpdated;

        protected virtual void Destroyed()
        {
            SquareState = SquareState.Done;
            RevealAround();
            OnDestroy?.Invoke();
        }


        protected void DelayUpdate()
        {
            if (!(this is FighterData f && f.Cloned))
            {
                DelayBroadCastManager.Instance.Add(this);
            }
        }

        public void BroadCastUpdated()
        {
            OnUpdated?.Invoke();
        }


        /// <summary>
        /// 面积
        /// </summary>
        [JsonIgnore]
        public int Area => Placement.Height * Placement.Width;

        [JsonIgnore]
        protected Rank _rank
        {
            get
            {
                if (Area <= 4)
                {
                    return Rank.Normal;
                }

                return Area >= 16 ? Rank.Rare : Rank.Uncommon;
            }
        }

        public void Reveal()
        {
            SquareState = SquareState.UnFocus;
            DelayUpdate();
        }

        [Button]
        public bool NextTo(Placement p2)
        {
            var p1 = Placement;

            var p3 = new Placement
            {
                x = math.max(p1.x, p2.x),
                y = math.max(p1.y, p2.y),
                Width = math.min(p1.x + p1.Width, p2.x + p2.Width) - math.max(p1.x, p2.x),
                Height = math.min(p1.y + p1.Height, p2.y + p2.Height) - math.max(p1.y, p2.y),
            };

            return (!(p3.Height < 0 || p3.Width < 0) && (!((p3.Height == 0) && (p3.Width == 0))));
        }


        public void RevealAround()
        {
            foreach (var square in GameManager.Instance.Map.Floors[GameManager.Instance.Map.CurrentFloor].Squares)
            {
                if (square.SquareState != SquareState.UnRevealed)
                {
                    continue;
                }

                if (NextTo(square.Placement))
                {
                    square.Reveal();
                }
            }
        }

        [Button]
        public virtual SquareInfo GetSquareInfo()
        {
            var declaringType = GetType();
            var name = declaringType.Name.Replace("SaveData", "").ToLower();
            return new SquareInfo()
            {
                Name = name,
                Desc = $"{name}_desc"
            };
        }


        protected virtual void InformReactResult(Args obj)
        {
            ReactResultInfo?.Invoke(obj);
        }

        public object Clone()
        {
            var f = JsonConvert.SerializeObject(this, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            });

            return JsonConvert.DeserializeObject(f, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            });
        }
    }


    public struct Placement
    {
        public int x;
        public int y;
        public int Width;
        public int Height;

        public Placement(int x, int y, int width, int height)
        {
            this.x = x;
            this.y = y;
            this.Width = width;
            this.Height = height;
        }

        public override string ToString()
        {
            return $"({x},{y}), ({x + Width},{y + Height})";
        }
    }


    [Flags]
    public enum SquareState
    {
        UnRevealed = 0b0001,
        Focus = 0b0010,
        UnFocus = 0b0100,
        Done = 0b1000,
        Revealed = Focus | UnFocus
    }

    public enum SquareChangeTrigger
    {
        UnReveal,
        Focus,
        UnFocus,
        Done
    }


    public class SquareInfo
    {
        public string Name;
        public string Desc;
        public string P1;
        public string P2;
    }
}
using System;
using Managers;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UI;
using Unity.Mathematics;
using UnityEngine;

namespace Game
{
    public class MapData : IUpdateable
    {
        public Placement Placement;
        public SquareState SquareState = SquareState.UnRevealed;

        public Func<FighterData> SkillBody;
        
        /// <summary>
        /// 新游戏时调用
        /// </summary>
        public virtual void Init(){}
        
        /// <summary>
        /// 加载游戏时调用
        /// </summary>
        public virtual void Load(){}
        
        public virtual void OnFocus(){}
        
        public virtual void OnReact(){}
        
        public virtual void OnLeave(){}
        
        public event Action OnDestroy;
        public event Action OnUpdated;

        protected virtual void Destroyed()
        {
            SquareState = SquareState.Done;
            RevealAround();
            OnDestroy?.Invoke();
        }

        protected void Updated()
        {
            OnUpdated?.Invoke();
        }
        
        
        /// <summary>
        /// 面积
        /// </summary>
        [JsonIgnore] public int Area => Placement.Height * Placement.Width;

        [JsonIgnore]
        protected Rank _rank
        {
            get
            {
                if (Area <= 4)
                {
                    return Rank.Normal;
                }

                return Area>=16 ? Rank.Rare : Rank.Uncommon;
            }
        }

        public void Reveal()
        {
            SquareState = SquareState.UnFocus;
            Updated();
        }
        
        [Button]
        public bool NextTo(Placement p2)
        {
            var p1 = Placement;
            
            var p3 = new Placement
            {
                x = math.max(p1.x, p2.x),
                y = math.max(p1.y, p2.y),
                Width = math.min(p1.x+p1.Width, p2.x + p2.Width)- math.max(p1.x, p2.x),
                Height = math.min(p1.y+p1.Height, p2.y + p2.Height)- math.max(p1.y, p2.y),
            };

            return (!(p3.Height < 0 || p3.Width < 0)&&(!((p3.Height == 0)&&(p3.Width == 0))));
        }
        
        
        public void RevealAround()
        {
            foreach (var square in GameManager.Instance.Map.Floors[GameManager.Instance.Map.CurrentFloor].Squares)
            {
                if (square.SquareState!= SquareState.UnRevealed)
                {
                    continue;
                }
                
                if (NextTo(square.Placement))
                {
                    square.Reveal();
                }
            }
        }


        protected void PlaySoundEffect(string id)
        {
            AudioPlayer.Instance.Play(id);
        }


    }


    public struct Placement
    {
        public int x;
        public int y;
        public int Width;
        public int Height;

        public override string ToString()
        {
            return $"({x},{y}), ({x+Width},{y+Height})";
        }
    }
    
    
    public enum SquareState
    {
        UnRevealed = 0b0001,
        Focus = 0b0010,
        UnFocus = 0b0100,
        Done = 0b1000,
        Revealed = Focus | UnFocus
    }
}
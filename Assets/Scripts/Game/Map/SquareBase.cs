using System;
using I2.Loc;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Game
{
    [ExecuteAlways]
    public class SquareBase: MonoBehaviour
    {

        private const float Spacing = .06f;
        
        public SpriteRenderer Icon;
        public Localize Id;
        public TMP_Text Bonus;

        public Transform Global;
        
        public void Set(MapData data)
        {
            transform.position = new Vector3(data.x + Spacing/2, data.y + Spacing/2, 0);
            transform.localScale = new Vector3(data.Width - Spacing, data.Height - Spacing, 0);
            Global.localScale = new Vector3(1/(data.Width -Spacing), 1/(data.Height - Spacing));
        }


        public event Action Clicked;
        
        public void OnClick()
        {
            Clicked?.Invoke();
        }
    }
}
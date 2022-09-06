using System;
using I2.Loc;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Game
{
    public class SquareBase: MonoBehaviour
    {

        private const float Spacing = .06f;
        
        public SpriteRenderer Icon;
        public Localize Id;
        public TMP_Text Bonus;

        public Transform Global;
        
        public void Set(MapData Data)
        {
            transform.position = new Vector3(Data.x + Spacing/2, Data.y + Spacing/2, 0);
            transform.localScale = new Vector3(Data.Width - Spacing, Data.Height - Spacing, 0);
            Global.localScale = new Vector3(1/(Data.Width -Spacing), 1/(Data.Height - Spacing));
        }


        public event Action Clicked;
        
        public void OnClick()
        {
            Clicked?.Invoke();
        }
    }
}
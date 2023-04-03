using System;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;

namespace Managers
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "Game", order = 0)]
    public class GameSettings : SerializedScriptableObject
    {
        [Range(0, 1)] public float BgmVolume;
        [Range(0, 1)] public float SEVolume;
        public bool BgmMute;
        public bool SEMute;

        private GameObject _bg;
        public GameObject BG 
        {
            get 
            {
                if (_bg == null) 
                {
                    _bg =  Camera.main.transform.Find("BG").gameObject;
                }
                return _bg;
            }
        }
        
        [JsonIgnore, ShowInInspector] public float Degree
        {
            get => _degree;
            set
            {
                var r = Camera.main.transform.rotation.eulerAngles;
                r.x = value;
                Debug.Log(r);
                Camera.main.transform.rotation = Quaternion.Euler(r);
                WindowManager.Instance.EnemyPanel.transform.rotation = Camera.main.transform.rotation;
                _degree = value;
            }
        }

        private float _degree;
        
        [JsonIgnore, ShowInInspector] public float FOV
        {
            get => _fov;
            set
            {
                _fov = value;
                Camera.main.fieldOfView = value;
                BG.transform.localScale = new Vector3(2, 2, 0) * (float)Math.Tan(_fov / 360 * Math.PI);
            }
        }
        private float _fov;

    }
}
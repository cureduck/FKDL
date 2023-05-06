﻿using System;
using I2.Loc;
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
        public bool AutoGoToFocus;


        private GameObject _bg;
        public GameObject BG
        {
            get
            {
                if (_bg == null)
                {
                    _bg = Camera.main.transform.Find("BG").gameObject;
                }
                return _bg;
            }
        }

        //角度
        [JsonIgnore, ShowInInspector] public float Degree
        {
            get => _degree;
            set
            {
                var r = Camera.main.transform.rotation.eulerAngles;
                r.x = value;
                Debug.Log(r);
                Camera.main.transform.rotation = Quaternion.Euler(r);
                if (GameManager.Instance.Focus != null)
                {
                    CameraMan.Instance.Target = GameManager.Instance.Focus.transform.position;
                }
                WindowManager.Instance.EnemyPanel.transform.rotation = Camera.main.transform.rotation;
                WindowManager.Instance.SquareInfoPanel.transform.rotation = Camera.main.transform.rotation;
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

        #region 语言
        private string _languageType;
        [JsonIgnore, ShowInInspector]
        public string LanguageType
        {
            get => _languageType;
            set
            {
                if (value == _languageType)
                {
                    return;
                }
                else
                {
                    _languageType = value;
                    LocalizationManager.CurrentLanguage = _languageType;
                }
            }
        }

        #endregion

        #region 分辨率
        private int _settingScreenX;
        private int _settingScreenY;
        [JsonIgnore, ShowInInspector]
        public Vector2Int ScreenSize
        {
            get => new Vector2Int(_settingScreenX, _settingScreenY);
            set
            {
                if (_settingScreenX == value.x && _settingScreenY == value.y)
                {
                    return;
                }
                else
                {
                    _settingScreenX = value.x;
                    _settingScreenY = value.y;
                    Screen.SetResolution(_settingScreenX, _settingScreenY, _isFullScreen);
                }


            }
        }

        #endregion

        #region 全屏
        private bool _isFullScreen;
        [JsonIgnore, ShowInInspector]
        public bool IsFullScreen
        {
            get => _isFullScreen;
            set
            {
                if (_isFullScreen == value)
                {
                    return;
                }
                else 
                {
                    _isFullScreen = value;
                    Screen.SetResolution(_settingScreenX, _settingScreenY, _isFullScreen);
                }

            }
        }
        #endregion
    }
}
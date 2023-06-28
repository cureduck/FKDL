using System;
using EasyTransition;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Managers
{
    public enum CameraSetting
    {
        AlwaysFollow,
        EnemyFollow,
        NeverFollow
    }

    [CreateAssetMenu(fileName = "GameSettings", menuName = "Game", order = 0)]
    public class GameSettings : SerializedScriptableObject
    {
        public const float MaxDegree = -35;
        [Range(0, 1)] public float BgmVolume;
        [Range(0, 1)] public float SEVolume;
        public bool BgmMute;
        public bool SEMute;

        [FormerlySerializedAs("AutoGoToFocus")]
        public CameraSetting CameraFollow = CameraSetting.NeverFollow;

        public TransitionManagerSettings TransitionManagerSettings;

        private GameObject _bg;

        private float _degree;


        private float _fov;

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
        [JsonIgnore, ShowInInspector]
        public float Degree
        {
            get => _degree;
            set
            {
                if (SceneManager.GetActiveScene().name == "StartScene") return;
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


        [JsonIgnore, ShowInInspector]
        public float FOV
        {
            get => _fov;
            set
            {
                if (SceneManager.GetActiveScene().name == "StartScene") return;
                _fov = value;
                Camera.main.fieldOfView = value;
                BG.transform.localScale = new Vector3(2, 2, 0) * (float)Math.Tan(_fov / 360 * Math.PI);
            }
        }

        /*#region 语言

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

        #endregion*/

        #region 分辨率

        [SerializeField] private int _settingScreenX;

        [SerializeField] private int _settingScreenY;

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
                    TransitionManagerSettings.transitions[5].refrenceResolution =
                        new Vector2(_settingScreenX, _settingScreenY);
                }
            }
        }

        #endregion

        #region 全屏

        [ShowInInspector] private bool _isFullScreen = true;

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
                    Screen.SetResolution(Screen.width, Screen.height, _isFullScreen);
                }
            }
        }

        #endregion
    }
}
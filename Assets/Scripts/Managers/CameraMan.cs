using System;
using System.Collections;
using Game;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;

namespace Managers
{
    public class CameraMan : Singleton<CameraMan>
    {
        [SerializeField] private Volume volume;

        public Vector3 offset;
        private Camera _camera;

        private Vector2 _target;

        public Vector2 Target
        {
            get => _target;
            set
            {
                _target = value;
                StartCoroutine(GoToPoint());
            }
        }

        [ShowInInspector]
        private Vector3 _offset
        {
            get
            {
                var degree = transform.rotation.eulerAngles.x;
                var dx = (float)Math.Tan(degree / 180 * Math.PI) * 10;
                return new Vector3(1.5f, dx - 2, 0);
            }
        }


        protected override void Awake()
        {
            base.Awake();
            _camera = GetComponent<Camera>();
        }

        private void Start()
        {
            SettingManager.Instance.GameSettings.Degree = SettingManager.Instance.GameSettings.Degree;
        }


        private void Update()
        {
            if (GameManager.Instance)
            {
                PlayerData playerData = GameManager.Instance.Player;
                if (playerData != null)
                {
                    volume.weight = 1 - playerData.Status.CurHp / (float)playerData.Status.MaxHp;
                }
            }
        }

        IEnumerator GoToPoint()
        {
            var delta = Vector2.left;
            while (delta.magnitude >= .1f)
            {
                var t = transform.position - _offset;
                delta = (Vector2)t - Target;
                var f = 10f;

                transform.position = new Vector3((t.x - delta.x / f), (t.y - delta.y / f), -10) + _offset;
                yield return null;
            }
        }

        public void SetVignetteInstensity(float targetWeight)
        {
            volume.weight = targetWeight;
        }
    }
}
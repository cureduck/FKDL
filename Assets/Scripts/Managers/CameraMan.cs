using System;
using System.Collections;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;

namespace Managers
{
    public class CameraMan : Singleton<CameraMan>
    {
        private Camera _camera;

        public Vector2 Target
        {
            get => _target;
            set
            {
                _target = value;
                StartCoroutine(GoToPoint());
            }
        }

        private Vector2 _target;

        public Vector3 offset;

        [ShowInInspector] private Vector3 _offset
        {
            get
            {
                var degree = transform.rotation.eulerAngles.x;
                var dx = (float)Math.Tan(degree /180 * Math.PI) * 10;
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


        }

        IEnumerator GoToPoint()
        {
            var delta = Vector2.left;
            while(delta.magnitude >= .1f)
            {
                var t = transform.position - _offset;
                delta = (Vector2)t - Target;
                var f = 10f;
            
                transform.position = new Vector3((t.x - delta.x/f), (t.y - delta.y/f), -10) + _offset;
                yield return null;
            }
            
        }
    }
}
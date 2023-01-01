using System;
using System.Collections;
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
        
        protected override void Awake()
        {
            base.Awake();
            _camera = GetComponent<Camera>();
        }

        private void Update()
        {


        }

        IEnumerator GoToPoint()
        {
            var delta = Vector2.left;
            while(delta.magnitude >= .1f)
            {
                var t = transform.position - offset;
                delta = (Vector2)t - Target;
                var f = 10f;
            
                transform.position = new Vector3((t.x - delta.x/f), (t.y - delta.y/f), -10) + offset;
                yield return null;
            }
            
        }
    }
}
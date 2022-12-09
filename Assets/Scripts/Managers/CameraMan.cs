using System;
using UnityEngine;

namespace Managers
{
    public class CameraMan : Singleton<CameraMan>
    {
        private Camera _camera;

        public Vector2 Target;

        public Vector3 offset;
        
        protected override void Awake()
        {
            base.Awake();
            _camera = GetComponent<Camera>();
        }

        private void Update()
        {

            var t = transform.position - offset;
            var delta = (Vector2)t - Target;
            var f = 10f;
            
            
            
            transform.position = new Vector3((t.x - delta.x/f), (t.y - delta.y/f), -10) + offset;
            
        }
    }
}
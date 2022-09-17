using Game;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Sirenix.OdinInspector;


namespace Managers
{
    public class InputSystem : MonoBehaviour
    {
        
        public enum Mode
        {
            ChooseMode,
            RemoveSkillMode,
            DragMode
        }
        
        public Vector2 pos;

        [ShowInInspector] private Vector2 delta;

        public float f;
        
        private void Update()
        {
            Debug.Log(Input.mousePosition);
            
            delta =  ((Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - pos)/2;
            pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            
            if (EventSystem.current.IsPointerOverGameObject()) return;

            if (LeftClicked())
            {
                pos = GetPosition();
                Raycast();
                return;
            }

            if (BeginDrag())
            {
                //Debug.Log(delta);
                Camera.main.transform.position -= (Vector3)delta;
            }
            
        }

        
        

        private bool LeftClicked()
        {
            /*if (EventSystem.current.IsPointerOverGameObject())
            {
                return false;
            }*/
#if UNITY_ANDROID
            return (Input.touchCount == 1) ;
#endif
            return (Input.GetMouseButtonUp(0)) ;
        }


        private bool BeginDrag()
        {
            return Input.GetMouseButton(1);
        }
        
        
        

        private static Vector2 GetPosition()
        {
#if UNITY_ANDROID

                return Input.GetTouch(0).position;
#endif
            
            return Input.mousePosition;
        }

        private void Raycast()
        {
            var worldPos = Camera.main.ScreenToWorldPoint(pos);
            var hit = Physics2D.Raycast(worldPos, Vector2.zero);
            if (hit.transform != null)
            {
                var t = hit.transform.GetComponent<Square>().Data;
                if ( t != null)
                {
                    t.OnFocus();
                    t.OnReact();
                }
            }
        }
    }
}
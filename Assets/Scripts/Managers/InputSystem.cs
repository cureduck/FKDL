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
        
        private void Update()
        {
            
            Scroll();
            

            
            if (EventSystem.current.IsPointerOverGameObject()) return;

            if (LeftClicked())
            {
                pos = GetPosition();
                Raycast();
                return;
            }

            if (BeginDrag())
            {
                delta =  ((Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - pos + delta);
                if (Input.GetMouseButtonDown(1))
                {
                    delta = Vector2.zero;
                }
                pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
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

        private void Scroll()
        {
            var orthographicSize = Camera.main.orthographicSize;
            orthographicSize -=  Input.mouseScrollDelta.y;
            orthographicSize = orthographicSize > 10 ? 10 : orthographicSize;
            orthographicSize = orthographicSize <3 ? 3 : orthographicSize;
            Camera.main.orthographicSize = orthographicSize;
        }
    }
}
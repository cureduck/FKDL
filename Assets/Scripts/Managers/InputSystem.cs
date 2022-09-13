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



        private void Update()
        {
            if (Clicked()&&(!EventSystem.current.IsPointerOverGameObject()))
            {
                pos = GetPosition();
                Raycast();
            }
        }

        
        

        private bool Clicked()
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
                var t = hit.transform.GetComponent<SquareBase>();
                if ( t != null)
                {
                    t.Focus();
                    t.React();
                }
            }
        }
    }
}
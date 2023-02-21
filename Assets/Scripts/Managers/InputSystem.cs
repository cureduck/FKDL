using System;
using Cysharp.Threading.Tasks;
using Game;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using UnityEngine.Experimental.Rendering.Universal;


namespace Managers
{
    public class InputSystem : Singleton<InputSystem>
    {
        private PlayerData P => GameManager.Instance.PlayerData;

        public GameObject BG;
        
        public enum Mode
        {
            SelectPotionMode,
            SelectEnemyMode,
            NormalMode
        }

        public Mode InputMode;
        
        public Vector2 pos;

        [ShowInInspector] private Vector2 delta;
        private Vector2 prePos;
        
        private void Update()
        {
            
            Scroll();

            delta = (Vector2)Input.mousePosition - prePos;
            prePos = Input.mousePosition;
            
            if (EventSystem.current.IsPointerOverGameObject()) return;

            if (LeftClicked())
            {
                Raycast();
                return;
            }

            if (BeginDrag())
            {
                /*delta =  ((Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - pos + delta);
                if (Input.GetMouseButtonDown(1))
                {
                    //delta = Vector2.zero;
                }
                pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Camera.main.transform.position -= (Vector3)delta;*/
                
                Camera.main.transform.position -= (Vector3)delta * DragRate;
                BG.transform.position -= (Vector3)delta * DragRate * 0.05f;
            }
            
        }

        public float DragRate;



        private bool LeftClicked()
        {
            /*if (EventSystem.current.IsPointerOverGameObject())
            {
                return false;
            }*/
#if UNITY_ANDROID
            return (Input.touchCount == 1) ;
#endif
            return (Input.GetMouseButtonUp(0));
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
            var Pos = Input.mousePosition;

            var ray = Camera.main.ScreenPointToRay(Pos);
            if (IntersectionOfRayAndFace(ray, Vector3.forward, Vector3.zero, out var ret))
            {
                Debug.Log(ray);
                Debug.Log(ret);
                var hit = Physics2D.Raycast(ret, Vector2.zero);
                if (hit.transform != null)
                {
                    var sq = hit.transform.GetComponentInParent<Square>();
                    
                    var t = sq.Data;

                    if ((t != null) && (t.SquareState != SquareState.UnRevealed))
                    {
                        CameraMan.Instance.Target = sq.transform.position;
                    }

                    if ((t != null)&&((t.SquareState == SquareState.Focus)||(t.SquareState == SquareState.UnFocus)))
                    {
                        if (GameManager.Instance.Focus != sq)
                        {
                            Square previous = null;
                            if (GameManager.Instance.Focus != null)
                            {
                                GameManager.Instance.Focus?.UnFocus();

                                if ((GameManager.Instance.Focus.Data is EnemySaveData es)&&(es.IsAlive))
                                {
                                    es.Chase();
                                }
                                
                                GameManager.Instance.PlayerData.Engaging = true;
                                
                                previous = GameManager.Instance.Focus;
                            }
                            GameManager.Instance.Focus = sq;
                            if (previous != null) previous.UpdateFace();
                            sq.Focus();
                            //t.RevealAround();
                        }
                        //t.OnFocus();
                        t.OnReact();
                    }
                }
            }
        }


        /*
        private MapData RaycastGetMapData()
        {
            var worldPos = Camera.main.ScreenToWorldPoint(pos);
            var hit = Physics2D.Raycast(worldPos, Vector2.zero);
            if (hit.transform != null)
            {
                var t = hit.transform.GetComponent<Square>().Data;
                return t;
            }
            else
            {
                return null;
            }
        }
        */


        public Light2D light2D;
        
        private void Scroll()
        {
            var orthographicSize = Camera.main.orthographicSize;
            orthographicSize -=  Input.mouseScrollDelta.y;
            orthographicSize = orthographicSize > 10 ? 10 : orthographicSize;
            orthographicSize = orthographicSize <3 ? 3 : orthographicSize;

            light2D.pointLightOuterRadius = orthographicSize;
            Camera.main.orthographicSize = orthographicSize;
        }
        
        
        /// <summary>
        /// 计算射线和面的交点 
        /// 会有一定误差 ， 浮点数计算没有办法
        /// </summary>
        /// <param name="ray">射线</param>
        /// <param name="normal">面的法线</param>
        /// <param name="Point">面上的一点</param>
        /// <param name="ret">交点</param>
        /// <returns>线和面是否相交</returns>
        private bool IntersectionOfRayAndFace(Ray ray, Vector3 normal,Vector3 Point, out Vector3 ret)
        {
            if (Vector3.Dot(ray.direction, normal) == 0)
            {
                //如果平面法线和射线垂直 则不会相交
                ret = Vector3.zero;
                return false;
            }
            Vector3 Forward = normal;
            Vector3 Offset = Point - ray.origin; //获取线的方向
            float DistanceZ = Vector3.Angle(Forward, Offset); //计算夹角
            DistanceZ = Mathf.Cos(DistanceZ / 180f * Mathf.PI) * Offset.magnitude; //算点到面的距离
            DistanceZ /= Mathf.Cos(Vector3.Angle(ray.direction, Forward) / 180f * Mathf.PI); //算点沿射线到面的距离
            ret = ray.origin + ray.direction * DistanceZ; //算得射线和面的交点
            return true;
        }

        
    }
}
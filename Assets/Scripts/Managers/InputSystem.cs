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

        public SkillData AwaitTargetSkill;
        
        private PlayerData P => GameManager.Instance.PlayerData;

        
        public enum Mode
        {
            SelectPotionMode,
            SelectEnemyMode,
            NormalMode
        }

        public Mode InputMode;
        
        public Vector2 pos;

        [ShowInInspector] private Vector2 delta;
        
        private void Update()
        {
            
            Scroll();
            
            
            if (EventSystem.current.IsPointerOverGameObject()) return;

            if (LeftClicked())
            {
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



        private void ResetInputMode()
        {
            InputMode = Mode.NormalMode;
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
            return (Input.GetMouseButtonUp(0));
        }


        private bool BeginDrag()
        {
            return Input.GetMouseButton(1);
        }
        
        
        
        public void ArrangeFight(EnemySaveData enemy)
        {
            Attack pa;
            if (this.InputMode == Mode.NormalMode)
            {
                pa = P.ForgeAttack(enemy);
            }
            else
            {
                pa = P.ForgeAttack(enemy, AwaitTargetSkill);
                ResetInputMode();
            }
            

            //怪物防御阶段
            var result = enemy.Defend(pa, P);

            //攻击后结算阶段
            var r = P.Settle(result, enemy);

            
            //死亡判断
            if (enemy.Status.CurHp <= 0 )
            {
                //DestroyImmediate(sq.gameObject);
                P.Kill(r, enemy);
                P.Gain(enemy.Gold);
            }
            else
            {
                var pa2 = enemy.ForgeAttack(P);
                
                var result2 = P.Defend(pa2, enemy);
                var r2 = enemy.Settle(result2, P);
            }
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
                    var sq = hit.transform.parent.GetComponent<Square>();
                    
                    var t = sq.Data;
                    if ((t != null)&&(t.SquareState == SquareState.Revealed))
                    {
                        if (GameManager.Instance.Focus != sq)
                        {
                            GameManager.Instance.Focus?.UnFocus();
                            GameManager.Instance.Focus = sq;
                            sq.Focus();
                        }
                        
                        t.RevealAround();
                        t.OnFocus();
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


        public Light2D light;
        
        private void Scroll()
        {
            var orthographicSize = Camera.main.orthographicSize;
            orthographicSize -=  Input.mouseScrollDelta.y;
            orthographicSize = orthographicSize > 10 ? 10 : orthographicSize;
            orthographicSize = orthographicSize <3 ? 3 : orthographicSize;

            light.pointLightOuterRadius = orthographicSize;
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
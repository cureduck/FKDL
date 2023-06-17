using System;
using Game;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.Rendering.Universal;
using static UnityEngine.Mathf;

namespace Managers
{
    public class InputSystem : Singleton<InputSystem>
    {
        public enum Mode
        {
            SelectPotionMode,
            SelectEnemyMode,
            NormalMode
        }


        private const float BaseIndent = 1f;

        public GameObject BG;

        public Mode InputMode;

        public Vector2 pos;

        public float DragRate;


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


        private Vector3 center;

        [ShowInInspector] private Vector2 delta;
        private float HoldTime;
        private float indent;
        private Vector2 prePos;
        private PlayerData P => GameManager.Instance.Player;

        private void Update()
        {
            Scroll();

            delta = (Vector2)Input.mousePosition - prePos;
            prePos = Input.mousePosition;

            if (EventSystem.current.IsPointerOverGameObject()) return;

            if (LeftClicked())
            {
                ClickReact();
                return;
            }

            if (Input.GetMouseButton(0))
            {
                HoldTime += Time.deltaTime;
                HoldToContinuousAttack();
            }

            if (Input.GetMouseButtonUp(0))
            {
                ResetContinuousAttack();
            }

            MiddleClickToCloseBattlePredict();


            if (BeginDrag())
            {
                return;
                delta = delta * 960 / SettingManager.Instance.GameSettings.ScreenSize.y;
                Camera.main.transform.position -= (Vector3)delta * DragRate * Camera.main.fieldOfView;
                BG.transform.position -= (Vector3)delta * DragRate * 0.05f;
                if (Vector3.Distance(Camera.main.transform.position, center) > 25f)
                {
                    Camera.main.transform.position += (Vector3)delta * DragRate * Camera.main.fieldOfView;
                }
            }
        }

        private void ResetContinuousAttack()
        {
            HoldTime = 0;
            indent = BaseIndent;
        }


        private void MiddleClickToCloseBattlePredict()
        {
            if (Input.GetMouseButton(2))
            {
                WindowManager.Instance.EnemyPanel.Close();
            }
        }

        private void HoldToContinuousAttack()
        {
            var sq = RaycastGetSquare();
            if (sq != null && GameManager.Instance.Focus != null && sq == GameManager.Instance.Focus &&
                sq.Data is EnemySaveData enemy)
            {
                indent -= Time.deltaTime;
                if (indent < 0)
                {
                    enemy.OnReact();
                    sq.OnReactLogic();
                    indent = Max(.1f, BaseIndent - HoldTime / 7f);
                }
            }
            else
            {
                ResetContinuousAttack();
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
            return (Input.GetMouseButtonUp(0));
        }

        private bool BeginDrag()
        {
            if (!Input.GetMouseButton(1)) return false;
            center = GameManager.Instance.FindStartSquare().transform.position;
            return true;
        }


        private static Vector2 GetPosition()
        {
#if UNITY_ANDROID
                return Input.GetTouch(0).position;
#endif

            return Input.mousePosition;
        }


        private Square RaycastGetSquare()
        {
            var Pos = Input.mousePosition;

            var ray = Camera.main.ScreenPointToRay(Pos);
            if (IntersectionOfRayAndFace(ray, Vector3.forward, Vector3.zero, out var ret))
            {
                // Debug.Log(ray);
                // Debug.Log(ret);
                var hit = Physics2D.Raycast(ret, Vector2.zero);
                if (hit.transform != null)
                {
                    var sq = hit.transform.GetComponentInParent<Square>();

                    return sq;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        private void ClickReact()
        {
            var sq = RaycastGetSquare();
            var t = sq != null ? sq.Data : null;

            if ((t != null) && (t.SquareState != SquareState.UnRevealed) && t.SquareState != SquareState.Done)
            {
                switch (SettingManager.Instance.GameSettings.CameraFollow)
                {
                    case CameraSetting.NeverFollow:
                        break;
                    case CameraSetting.EnemyFollow:
                        if (t is EnemySaveData)
                        {
                            CameraMan.Instance.Target = sq.transform.position;
                        }

                        break;
                    case CameraSetting.AlwaysFollow:
                        CameraMan.Instance.Target = sq.transform.position;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            if ((t != null) && ((t.SquareState == SquareState.Focus) || (t.SquareState == SquareState.UnFocus)))
            {
                if (GameManager.Instance.Focus != sq)
                {
                    GameManager.Instance.BroadcastSquareChanged(sq);

                    Square previous = null;
                    if (GameManager.Instance.Focus != null)
                    {
                        GameManager.Instance.Focus?.UnFocus();

                        /*if ((GameManager.Instance.Focus.Data is EnemySaveData es)&&(es.IsAlive))
                        {
                            es.Chase();
                        }*/
                        GameManager.Instance.Focus.Data.OnLeave();

                        GameManager.Instance.Player.Engaging = true;

                        previous = GameManager.Instance.Focus;
                    }

                    GameManager.Instance.Focus = sq;

                    if (previous != null) previous.UpdateFace();
                    sq.Focus();

                    if (t is EnemySaveData)
                    {
                    }
                    else
                    {
                        t.OnReact();
                        sq.OnReactLogic();
                    }
                }
                else
                {
                    t.OnReact();
                    sq.OnReactLogic();
                }
            }
        }

        private void Scroll()
        {
            var FOV = Camera.main.fieldOfView;
            FOV -= Input.mouseScrollDelta.y * 3;
            FOV = FOV > 80 ? 80 : FOV;
            FOV = FOV < 30 ? 30 : FOV;

            light2D.pointLightOuterRadius = FOV;
            SettingManager.Instance.GameSettings.FOV = FOV;
            //Camera.main.fieldOfView = FOV;
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
        private bool IntersectionOfRayAndFace(Ray ray, Vector3 normal, Vector3 Point, out Vector3 ret)
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
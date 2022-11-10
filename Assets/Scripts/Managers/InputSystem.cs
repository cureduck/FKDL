using System;
using Cysharp.Threading.Tasks;
using Game;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Sirenix.OdinInspector;


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
            var worldPos = Camera.main.ScreenToWorldPoint(pos);
            var hit = Physics2D.Raycast(worldPos, Vector2.zero);
            if (hit.transform != null)
            {
                var t = hit.transform.GetComponent<Square>().Data;
                if ((t != null)&&(t.SquareState == SquareState.Revealed))
                {
                    t.RevealAround();
                    t.OnFocus();
                    t.OnReact();
                }
            }
        }


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
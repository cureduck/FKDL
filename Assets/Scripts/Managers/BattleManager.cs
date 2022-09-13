using Game;
using UnityEngine;

namespace Managers
{
    public class BattleManager : Singleton<BattleManager>
    {
        private PlayerData P => GameManager.Instance.PlayerData;
        public void Fight(Square sq)
        {
            var enemy = (EnemySaveData) sq.Data;
            var pa = P.ForgeAtk();
            
            //玩家攻击阶段
            foreach (var func in P.AttackModifiers)
            {
                pa = func.Invoke(pa, P, enemy);
            }
            
            //怪物防御阶段
            foreach (var func in enemy.DefendModifiers)
            {
                pa = func.Invoke(pa, enemy, P);
            }
            
            var result = enemy.Suffer(pa);

            //攻击后结算阶段
            foreach (var func in P.SettleModifiers)
            {
                result = func.Invoke(result, P, enemy);
            }

            sq.UpdateFace();
            //死亡判断
            if (enemy.Status.CurHp <= 0)
            {
                DestroyImmediate(sq.gameObject);
            }
            else
            {
                var pa2 = enemy.ForgeAtk();
                //怪物攻击阶段
                foreach (var func in enemy.AttackModifiers)
                {
                    pa2 = func.Invoke(pa2, enemy, P);
                }
                
                foreach (var func in P.DefendModifiers)
                {
                    pa2 = func.Invoke(pa2, P, enemy);
                }
                
                var result2 = P.Suffer(pa2);
            
                //攻击后结算阶段
                foreach (var func in enemy.SettleModifiers)
                {
                    result2 = func.Invoke(result2, enemy, P);
                }
            }
            
            
            
        }


        
    }
}
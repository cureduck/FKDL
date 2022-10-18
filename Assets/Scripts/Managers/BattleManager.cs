using Game;
using Sirenix.Utilities;
using UnityEngine;

namespace Managers
{
    public class BattleManager : Singleton<BattleManager>
    {
        private PlayerData P => GameManager.Instance.PlayerData;
        public void Fight(EnemySaveData enemy)
        {
            var pa = P.ForgeAttack(enemy);
            

            //怪物防御阶段
            var result = enemy.Suffer(pa, P);

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
                
                var result2 = P.Suffer(pa2, enemy);
                var r2 = enemy.Settle(result2, P);

            }
            
            
            
        }


        
    }
}
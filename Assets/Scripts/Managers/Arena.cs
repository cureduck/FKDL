using Game;

namespace Managers
{
    public class Arena
    {
        public readonly PlayerData PlayerClone;
        public readonly EnemySaveData EnemyClone;
        public Attack? PlayerAttack;
        public Attack? EnemyAttack;

        public Arena(PlayerData player, EnemySaveData enemy)
        {
            PlayerClone = (PlayerData)player.Clone();
            EnemyClone = (EnemySaveData)enemy.Clone();
            PlayerClone.enemy = EnemyClone;
            EnemyClone.enemy = PlayerClone;
            PlayerClone.Cloned = true;
            EnemyClone.Cloned = true;
        }

        public void Simulate()
        {
            PlayerAttack = PlayerClone.ManageAttackRound();
            if (EnemyClone.IsAlive) EnemyAttack = EnemyClone.PlanAttackRound();
        }
    }
}
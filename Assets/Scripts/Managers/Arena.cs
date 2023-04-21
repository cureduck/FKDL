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

        public void Simulate(SkillData skill)
        {
            var sk = (SkillData)skill.Clone();
            PlayerAttack = PlayerClone.ManageAttackRound(sk);
            if (EnemyClone.IsAlive) EnemyAttack = EnemyClone.PlanAttackRound();
        }

        public override string ToString()
        { 
            if (EnemyAttack != null) 
                return $"PD:{PlayerAttack.Value}*{PlayerAttack.Value.Multi}*{PlayerAttack.Value.Combo} \n" +
                       $" ED: {EnemyAttack.Value}*{EnemyAttack.Value.Multi}*{EnemyAttack.Value.Combo}";
            else
                return $"Player Dmg :{PlayerAttack.Value} Enemy Dmg: -";
        }
    }
}
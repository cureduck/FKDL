using System.Linq;
using Game;

namespace Managers
{
    public static class Arena
    {
        private static PlayerData PlayerClone;
        private static EnemySaveData EnemyClone;


        public static FightPredictResult ArrangeFight(PlayerData player, EnemySaveData enemy,
            SkillData playerUsingSkill)
        {
            BuildArena(player, enemy);
            return Simulate(playerUsingSkill);
        }

        private static void BuildArena(PlayerData player, EnemySaveData enemy)
        {
            PlayerClone = (PlayerData)player.Clone();
            EnemyClone = (EnemySaveData)enemy.Clone();
            PlayerClone.enemy = EnemyClone;
            EnemyClone.enemy = PlayerClone;
            PlayerClone.Cloned = true;
            EnemyClone.Cloned = true;
        }

        private static FightPredictResult Simulate(SkillData skill)
        {
            SkillData sk = null;
            if (skill != null)
            {
                sk = PlayerClone.Skills.FirstOrDefault(data =>
                    {
                        if (data == null)
                        {
                            return false;
                        }
                        else
                        {
                            return data.Id == skill.Id;
                        }
                    }
                );
            }


            EnemyClone.OnReact(sk, out var PlayerAttack, out var EnemyAttack);


            return new FightPredictResult()
            {
                Player = PlayerClone,
                Enemy = EnemyClone,
                PlayerAttack = PlayerAttack,
                EnemyAttack = EnemyAttack
            };
        }

        public struct FightPredictResult
        {
            public PlayerData Player;
            public EnemySaveData Enemy;

            public Attack? PlayerAttack;
            public Attack? EnemyAttack;
        }


        /*public override string ToString()
        { 
            if (EnemyAttack != null) 
                return $"PD:{PlayerAttack.Value}*{PlayerAttack.Value.Multi}*{PlayerAttack.Value.Combo} \n" +
                       $" ED: {EnemyAttack.Value}*{EnemyAttack.Value.Multi}*{EnemyAttack.Value.Combo}";
            else
                return $"Player Dmg :{PlayerAttack.Value} Enemy Dmg: -";
        }*/
    }
}
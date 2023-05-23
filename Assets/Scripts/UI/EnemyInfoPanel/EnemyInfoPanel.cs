using Game;
using Managers;
using UI;
using UnityEngine;

public class EnemyInfoPanel : BasePanel<EnemyInfoPanel.Args>
{
    [SerializeField] private TargetBattleView enemyView;
    [SerializeField] private TargetBattleView playerView;

    private SkillData playerUseSkill;

    public override void Init()
    {
        enemyView.Init();
        playerView.Init();
    }

    protected override void SetData(Args d)
    {
        if (Data != null)
        {
            Data.playerData.OnUpdated -= UpdateUI;
            Data.targetEnemy.OnUpdated -= UpdateUI;
        }

        base.SetData(d);
        if (Data != null)
        {
            Data.playerData.OnUpdated += UpdateUI;
            Data.targetEnemy.OnUpdated += UpdateUI;
        }
    }

    protected override void OnOpen()
    {
        UpdateUI();
    }


    protected override void UpdateUI()
    {
        playerView.SetData(Data.playerData);
        enemyView.SetData(Data.targetEnemy);
        SetResult(Arena.ArrangeFight(Data.playerData, Data.targetEnemy, playerUseSkill));
    }

    public void SetPlayerUseSkill(SkillData skillData)
    {
        playerUseSkill = skillData;
        Debug.Log(playerUseSkill);
        UpdateUI();
    }

    private void SetResult(Arena.FightPredictResult fightPredictResult)
    {
        //return;
        if (fightPredictResult.EnemyAttack != null)
        {
            int totalDamage = fightPredictResult.EnemyAttack.Value.PDmg + fightPredictResult.EnemyAttack.Value.MDmg +
                              fightPredictResult.EnemyAttack.Value.CDmg;
            int poisonDamage = Data.playerData.Status.CurHp - fightPredictResult.Player.Status.CurHp - totalDamage;

            playerView.SetResult(fightPredictResult.EnemyAttack.Value.PDmg, 1,
                fightPredictResult.EnemyAttack.Value.MDmg, 1, fightPredictResult.EnemyAttack.Value.CDmg, 1,
                poisonDamage);
        }
        else
        {
            int posionDamage = Data.playerData.Status.CurHp - fightPredictResult.Player.Status.CurHp;
            playerView.SetResult(0, 1, 0, 1, 0, 1, posionDamage);
        }

        if (fightPredictResult.PlayerAttack != null)
        {
            int enemyTotal = fightPredictResult.PlayerAttack.Value.PDmg + fightPredictResult.PlayerAttack.Value.MDmg +
                             fightPredictResult.PlayerAttack.Value.CDmg;
            Debug.Log(Data.targetEnemy.Status.CurHp);
            Debug.Log(fightPredictResult.Enemy.Status.CurHp);
            Debug.Log(enemyTotal);
            int posion = Data.targetEnemy.Status.CurHp - fightPredictResult.Enemy.Status.CurHp - enemyTotal;

            enemyView.SetResult(fightPredictResult.PlayerAttack.Value.PDmg, 1,
                fightPredictResult.PlayerAttack.Value.MDmg, 1, fightPredictResult.PlayerAttack.Value.CDmg, 1, posion);
        }
        else
        {
            int poisonDamage = Data.targetEnemy.Status.CurHp - fightPredictResult.Enemy.Status.CurHp;
            enemyView.SetResult(0, 1, 0, 1, 0, 1, poisonDamage);
        }
    }

    public class Args
    {
        public PlayerData playerData;
        public EnemySaveData targetEnemy;
    }
}
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

    private void SetResult(Arena.FightPredictResult outcome)
    {
        //return;
        if (outcome.EnemyAttack != null)
        {
            int totalDamage = outcome.EnemyAttack.Value.SumDmg;
            int dif = Data.playerData.Status.CurHp - outcome.Player.Status.CurHp - totalDamage;

            playerView.SetResult(outcome.EnemyAttack.Value.PDmg, 1,
                outcome.EnemyAttack.Value.MDmg, 1, outcome.EnemyAttack.Value.CDmg, 1,
                dif);
        }
        else
        {
            int posionDamage = Data.playerData.Status.CurHp - outcome.Player.Status.CurHp;
            playerView.SetResult(0, 1, 0, 1, 0, 1, posionDamage);
        }

        if (outcome.PlayerAttack != null)
        {
            int enemyTotal = outcome.PlayerAttack.Value.SumDmg;
            int dif = Data.targetEnemy.Status.CurHp - outcome.Enemy.Status.CurHp - enemyTotal;

            enemyView.SetResult(outcome.PlayerAttack.Value.PDmg, 1,
                outcome.PlayerAttack.Value.MDmg, 1, outcome.PlayerAttack.Value.CDmg, 1, dif);
        }
        else
        {
            int poisonDamage = Data.targetEnemy.Status.CurHp - outcome.Enemy.Status.CurHp;
            enemyView.SetResult(0, 1, 0, 1, 0, 1, poisonDamage);
        }
    }

    public class Args
    {
        public PlayerData playerData;
        public EnemySaveData targetEnemy;
    }
}
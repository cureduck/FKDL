using Game;
using Managers;
using UI;
using UnityEngine;

public class EnemyInfoPanel : BasePanel<(PlayerData playerData, EnemySaveData targetEnemy, Vector3 position)>
{
    [SerializeField] private TargetBattleView enemyView;
    [SerializeField] private TargetBattleView playerView;

    private SkillData playerUseSkill;

    public override void Init()
    {
        enemyView.Init();
        playerView.Init();
    }

    protected override void SetData((PlayerData, EnemySaveData, Vector3) d)
    {
        if (!Data.Equals(default))
        {
            Data.playerData.OnUpdated -= UpdateUI;
            Data.targetEnemy.OnUpdated -= UpdateUI;
        }

        base.SetData(d);
        if (!Data.Equals(default))
        {
            Data.playerData.OnUpdated += UpdateUI;
            Data.targetEnemy.OnUpdated += UpdateUI;
        }
    }


    protected override void OnClose()
    {
        base.OnClose();
        playerUseSkill = null;
    }

    protected override void OnOpen()
    {
        Vector3 curPosition = Data.position;
        transform.position = curPosition;
        base.OnOpen();
        UpdateUI();
    }


    protected override void UpdateUI()
    {
        Canvas canvas = GetComponent<Canvas>();
        if (!canvas.worldCamera)
        {
            canvas.worldCamera = Camera.main;
        }

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
            int poisonDmg = Data.playerData.Status.CurHp - fightPredictResult.Player.Status.CurHp;
            playerView.SetResult(0, 1, 0, 1, 0, 1, poisonDmg);
        }

        if (fightPredictResult.PlayerAttack != null)
        {
            int enemyTotal = fightPredictResult.PlayerAttack.Value.PDmg + fightPredictResult.PlayerAttack.Value.MDmg +
                             fightPredictResult.PlayerAttack.Value.CDmg;
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
}
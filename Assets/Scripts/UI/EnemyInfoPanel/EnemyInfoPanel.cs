using Game;
using Managers;
using UI;
using UnityEngine;

public class
    EnemyInfoPanel : BasePanel<(PlayerData playerData, EnemySaveData targetEnemy, Square square, Vector3 position)>
{
    [SerializeField] private TargetBattleView enemyView;
    [SerializeField] private TargetBattleView playerView;
    private Square curCheckSquare;

    private SkillData playerUseSkill;

    private void Start()
    {
        Canvas canvas = GetComponent<Canvas>();
        if (!canvas.worldCamera)
        {
            canvas.worldCamera = Camera.main;
        }
    }

    private void OnDestroy()
    {
        GlobalEvents.MouseEnteringSquare -= OnPointEnter;
        GlobalEvents.MouseExitingSquare -= OnPointExit;
    }

    public override void Init()
    {
        enemyView.Init();
        playerView.Init();
    }

    protected override void SetData((PlayerData, EnemySaveData, Square, Vector3) d)
    {
        if (!Data.Equals(default))
        {
            Data.playerData.OnUpdated -= UpdateUI;
            Data.targetEnemy.OnUpdated -= UpdateUI;
            GlobalEvents.MouseEnteringSquare -= OnPointEnter;
            GlobalEvents.MouseExitingSquare -= OnPointExit;
        }

        base.SetData(d);
        if (!Data.Equals(default))
        {
            Data.playerData.OnUpdated += UpdateUI;
            Data.targetEnemy.OnUpdated += UpdateUI;
        }

        curCheckSquare = Data.square;
        GlobalEvents.MouseEnteringSquare += OnPointEnter;
        GlobalEvents.MouseExitingSquare += OnPointExit;
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
        playerView.SetData(Data.playerData);
        enemyView.SetData(Data.targetEnemy);

        if (playerUseSkill != null)
        {
            SetResult(Arena.ArrangeFight(Data.playerData, Data.targetEnemy, playerUseSkill));
        }
        else
        {
            if (curCheckSquare != Data.square)
            {
                SetResult(Arena.ArrangeFlee(Data.playerData, Data.targetEnemy));
            }
            else
            {
                SetResult(Arena.ArrangeFight(Data.playerData, Data.targetEnemy, playerUseSkill));
            }
        }
    }

    public void SetPlayerUseSkill(SkillData skillData)
    {
        playerUseSkill = skillData;
        Debug.Log(playerUseSkill);
        UpdateUI();
    }


    private void OnPointEnter(Square square)
    {
        //Debug.Log("Enter!");
        this.curCheckSquare = square;
        UpdateUI();
    }

    private void OnPointExit(Square square)
    {
        //Debug.Log("Exit!");
        this.curCheckSquare = null;
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
                poisonDamage, false);
        }
        else
        {
            int poisonDmg = Data.playerData.Status.CurHp - fightPredictResult.Player.Status.CurHp;
            playerView.SetResult(0, 1, 0, 1, 0, 1, poisonDmg, false);
        }

        if (fightPredictResult.PlayerAttack != null)
        {
            int enemyTotal = fightPredictResult.PlayerAttack.Value.PDmg + fightPredictResult.PlayerAttack.Value.MDmg +
                             fightPredictResult.PlayerAttack.Value.CDmg;
            int posion = Data.targetEnemy.Status.CurHp - fightPredictResult.Enemy.Status.CurHp - enemyTotal;

            enemyView.SetResult(fightPredictResult.PlayerAttack.Value.PDmg, 1,
                fightPredictResult.PlayerAttack.Value.MDmg, 1, fightPredictResult.PlayerAttack.Value.CDmg, 1, posion,
                fightPredictResult.isPlayerExcape);
        }
        else
        {
            int poisonDamage = Data.targetEnemy.Status.CurHp - fightPredictResult.Enemy.Status.CurHp;
            enemyView.SetResult(0, 1, 0, 1, 0, 1, poisonDamage, fightPredictResult.isPlayerExcape);
        }
    }
}
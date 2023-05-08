using Managers;
using TMPro;
using UnityEngine.UI;

namespace UI
{
    public class FightPredictPanel : BasePanel<Arena.FightPredictResult>
    {
        public TMP_Text PlayerDmgPred;
        public TMP_Text EnemyDmgPred;

        public Image PlayerDeathIcon;
        public Image EnemyDeathIcon;


        protected override void UpdateUI()
        {
            EnemyDmgPred.text = Data.Enemy.IsAlive ? Data.EnemyAttack.ToString() : "-";
            PlayerDmgPred.text = Data.PlayerAttack.ToString();

            PlayerDeathIcon.gameObject.SetActive(!Data.Player.IsAlive);
            EnemyDeathIcon.gameObject.SetActive(!Data.Enemy.IsAlive);
        }
    }
}
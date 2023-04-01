using Game;
using UnityEngine;

namespace UI
{
    public class EnemyPanel : FighterPanel<EnemyPanel>
    {

        public EnemySkillInfoView skillView;

        private void Start()
        {
            GetComponent<UnityEngine.Canvas>().worldCamera = Camera.main;
            //gameObject.SetActive(false);
        }

        public void Load(FighterData master)
        {
            Master = master;
        }

        protected override void SetMaster(FighterData master)
        {
            base.SetMaster(master);
            //获得主技能
            if (master.Skills.Count <= 0 || master.Skills[0] == null)
            {
                skillView.gameObject.SetActive(false);
            }
            else 
            {
                skillView.SetData(master, master.Skills[0]);
                skillView.gameObject.SetActive(true);
            }

        }

    }
}
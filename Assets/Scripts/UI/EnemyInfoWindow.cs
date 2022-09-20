using Game;
using Managers;
using TMPro;

namespace UI
{
    public class EnemyInfoWindow : Singleton<EnemyInfoWindow>
    {
        public TMP_Text Hp;
        public TMP_Text Mp;
        public TMP_Text PAD;
        public TMP_Text MAD;


        private EnemySaveData _previous;


        public void Load(EnemySaveData enemy)
        {
            enemy.OnUpdated += ()=> Display(enemy);
        }
        
        
        private void Display(EnemySaveData _p)
        {
            Hp.text = _p.Status.CurHp + "/" + _p.Status.MaxHp;
            Mp.text = _p.Status.CurMp + "/" + _p.Status.MaxMp;

            PAD.text = _p.Status.PAtk + "/" + _p.Status.PDef;
            MAD.text = _p.Status.MAtk + "/" + _p.Status.MDef;
        }
    }
}
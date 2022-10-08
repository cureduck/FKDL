using Game;

namespace UI
{
    public class EnemyPanel : FighterPanel<EnemyPanel>
    {
        public void Load(FighterData master)
        {
            Master = master;
        }
    }
}
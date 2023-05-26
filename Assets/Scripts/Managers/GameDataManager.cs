using Game;

namespace Managers
{
    public class GameDataManager : Singleton<GameDataManager>
    {
        public Map Map;
        public PlayerData PlayerData;
        public SecondaryData SecondaryData;

        protected override void Awake()
        {
            DontDestroyOnLoad(gameObject);
            base.Awake();
        }
    }
}
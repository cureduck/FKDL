namespace Managers
{
    public class SceneSwitchManager : Singleton<SceneSwitchManager>
    {
        /// <summary>
        /// TRUE就是新游戏，FALSE就是加载游戏
        /// </summary>
        public bool NewGame;

        private void Start()
        {
            DontDestroyOnLoad(this);
        }
    }
}
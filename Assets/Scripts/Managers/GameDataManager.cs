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

        public void SetProf()
        {
            SecondaryData = new SecondaryData { Prof = new string[ChoosingPanel.Instance.SelectedList.Count] };
            for (int i = 0; i < ChoosingPanel.Instance.SelectedList.Count; i++)
            {
                SecondaryData.Prof[i] = ChoosingPanel.Instance.SelectedList[i].gameObject.name;
            }
            //SecondaryData.Prof = profs;
        }
    }
}
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
            var profs = new string[ChoosingPanel.Instance.SelectedList.Count + 1];
            for (int i = 0; i < ChoosingPanel.Instance.SelectedList.Count; i++)
            {
                profs[i] = ChoosingPanel.Instance.SelectedList[i].gameObject.name;
            }

            profs[ChoosingPanel.Instance.SelectedList.Count] = "COM";

            SecondaryData = SecondaryData.GetOrCreate();
            SecondaryData.Profs = profs;
        }
    }
}
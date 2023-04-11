using Managers;

namespace Game
{
    public class KeySaveData : MapData
    {
        public Rank KeyRank;

        public KeySaveData(Rank rank)
        {
            KeyRank = rank;
        }

        public override void OnReact()
        {
            base.OnReact();
            GameManager.Instance.PlayerData.TryTakeKey(KeyRank, out _);
            Destroyed();
        }
    }
}
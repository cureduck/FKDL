using Managers;

namespace Game
{
    public class KeySaveData : MapData
    {
        public Rank Rank;

        public KeySaveData(Rank rank)
        {
            Rank = rank;
        }

        public override void OnReact()
        {
            base.OnReact();
            GameManager.Instance.PlayerData.Keys[Rank] += 1;
            OnDestroy?.Invoke();
        }
    }
}
using Game;
using Sirenix.OdinInspector;

namespace Managers
{
    public class DebugManager : Singleton<DebugManager>
    {
        [Button]
        public void Apply(string id, int lv)
        {
            GameManager.Instance.PlayerData.Apply(
                new BuffData
                {
                    Id = id,
                    CurLv = lv
                });
        }
    }
}
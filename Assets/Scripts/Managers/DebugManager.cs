using Game;
using Sirenix.OdinInspector;

namespace Managers
{
    public class DebugManager : Singleton<DebugManager>
    {
        [Button]
        public void Apply(string id, int lv)
        {
            GameManager.Instance.PlayerData.ApplyBuff(
                new BuffData
                {
                    Id = id,
                    CurLv = lv
                });
        }
        
        public void Add1Atk()
        {
            GameManager.Instance.PlayerData.Strengthen(new BattleStatus{PAtk = 1});
        }
        
        public void Add1Def()
        {
            GameManager.Instance.PlayerData.Strengthen(new BattleStatus{PDef = 1});
        }
    }
}
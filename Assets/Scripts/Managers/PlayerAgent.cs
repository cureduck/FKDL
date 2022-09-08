using Game;

namespace Managers
{
    public class PlayerAgent : Singleton<PlayerAgent>
    {
        public PlayerData P => GameManager.Instance.PlayerData;
        
        
    }
}
using I2.Loc;

namespace Game
{
    public abstract class Args
    {
        
    }

    public class BaseArgs : Args
    {
        public bool CanReact;
        public bool Win;
    }

    public class CasinoArgs : BaseArgs
    {
        
    }

    public class DoorArgs : BaseArgs
    {
        
    }

    public class EnemyArgs : Args
    {
        public Attack? PlayerAttack;
        public Attack? EnemyAttack;
        public string Keyword;
    }

    public class RockArgs : BaseArgs
    {
        
    }
}
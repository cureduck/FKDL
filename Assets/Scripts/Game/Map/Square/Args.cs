using I2.Loc;

namespace Game
{
    public abstract class Args
    {
        public Info Info;
    }

    public class BaseArgs : Args
    {
        public bool CanReact;
        public bool Win;

        public BaseArgs(bool canReact = true, bool win = false)
        {
            CanReact = canReact;
            Win = win;
        }
    }

    public class CasinoArgs : BaseArgs
    {
        public Offer[] Offers;
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
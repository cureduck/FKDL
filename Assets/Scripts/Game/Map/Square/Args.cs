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
        public int SkipCompensate = 15;
    }

    public class DoorArgs : BaseArgs
    {
    }

    public class EnemyArgs : Args
    {
        public bool IsDeath;
        public string Keyword;
        public string MonsterId;
        public Attack? PlayerAttack;
    }

    public class RockArgs : BaseArgs
    {
    }
}
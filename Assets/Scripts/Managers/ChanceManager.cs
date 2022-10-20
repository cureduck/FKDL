namespace Managers
{
    public class ChanceManager : Singleton<ChanceManager>
    {
        public float Never = 0f;
        public float Rare = .1f;
        public float Uncommon = .3f;
        public float Common = .7f;
        public float Always = 1f;
    }
}
namespace Game
{
    public class RelicData : IEffectContainer
    {
        public bool MayAffect(Timing timing, out int priority)
        {
            throw new System.NotImplementedException();
        }

        public T Affect<T>(Timing timing, object[] param)
        {
            throw new System.NotImplementedException();
        }

        public void Affect(Timing timing, object[] param)
        {
            throw new System.NotImplementedException();
        }
    }
}
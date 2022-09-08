namespace Game
{
    public struct Offer
    {
        public enum OfferKind
        {
            Potion,
            Skill,
            Gold
        }

        public OfferKind Kind;
        public string Id;
        public int Gold;
    }
}
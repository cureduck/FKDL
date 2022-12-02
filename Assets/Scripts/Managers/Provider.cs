using Game;

namespace Managers
{
    public class Provider : Singleton<Provider>
    {
        public PotionData CreateRandomPotion(Rank rank = Rank.Normal, int count = 1)
        {
            var potion = new PotionData
            {
                Id = (Tools.ChooseRandom(PotionManager.Instance.Ordered[rank])).Id,
                Count = count
            };

            return potion;
        }
        
        
        
    }
}
using Game;
using Tools;

namespace Managers
{
    public class Provider : Singleton<Provider>
    {
        public PotionData CreateRandomPotion(Rank rank = Rank.Normal, int count = 1)
        {
            var potion = new PotionData
            {
                Id = (Tools.Tools.ChooseRandom(PotionManager.Instance.Ordered[rank], GameDataManager.Instance.SecondaryData.CurGameRandom)).Id,
                Count = count
            };

            return potion;
        }
        
        
        
    }
}
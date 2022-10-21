using Game;
using Sirenix.OdinInspector;
using TMPro;

namespace Managers
{
    public class DebugManager : Singleton<DebugManager>
    {
        public TMP_InputField PotionDebugInput;
        
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


        [Button]
        public void AddPotion(string id)
        {
            GameManager.Instance.PlayerData.TryTake(new Offer()
            {
                Id = id,
                Kind = Offer.OfferKind.Potion
            });
        }


        public void AddPotion()
        {
            GameManager.Instance.PlayerData.TryTake(new Offer()
            {
                Id = PotionDebugInput.text,
                Kind = Offer.OfferKind.Potion
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
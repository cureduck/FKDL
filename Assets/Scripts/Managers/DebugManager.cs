using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Game;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class DebugManager : Singleton<DebugManager>
    {
        public TMP_InputField PotionDebugInput;
        public TMP_InputField BuffDebugInput;
        public TMP_InputField RelicDebugInput;
        public TMP_InputField SKillDebugInput;
        
        [Button]
        public void Apply(string id, int lv)
        {
            GameManager.Instance.PlayerData.AppliedBuff(
                new BuffData(id, lv));
        }


        [Button]
        public void AddPotion(string id)
        {
            GameManager.Instance.PlayerData.TryTakeOffer(new Offer()
            {
                Id = id,
                Kind = Offer.OfferKind.Potion
            }, out _);
        }

        [Button]
        public void AddBuff(string id)
        {
            GameManager.Instance.PlayerData.ApplySelfBuff(new BuffData(id, 1));
        }

        [Button]
        public void AddRelic(string id)
        {
            if (RelicManager.Instance.TryGetById(id, out var relic))
            {
                GameManager.Instance.PlayerData.TryTakeOffer(new Offer(relic), out _);
            }
        }

        public void AddRelic()
        {
            AddRelic(RelicDebugInput.text);
        }
        
        
        public void AddSkill(string id)
        {
            if (SkillManager.Instance.TryGetById(id, out var skill))
            {
                GameManager.Instance.PlayerData.TryTakeOffer(new Offer(skill), out _);
            }
        }

        public void AddSkill()
        {
            AddSkill(SKillDebugInput.text);
        }
        
        
        [Button]
        public void AddBuff()
        {
            AddBuff(BuffDebugInput.text);
        }

        

        public void AddPotion()
        {
            GameManager.Instance.PlayerData.TryTakeOffer(new Offer()
            {
                Id = PotionDebugInput.text,
                Kind = Offer.OfferKind.Potion
            }, out _);
        }

        public void Add100Gold()
        {
            GameManager.Instance.PlayerData.Gain(100);
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
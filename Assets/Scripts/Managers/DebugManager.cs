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
        
        [Button]
        public void Apply(string id, int lv)
        {
            GameManager.Instance.PlayerData.AppliedBuff(
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
        
        [Button]
        public void Test()
        {
            var t1 = UniTask.WhenAll(AsyncTest());
            
        }

        public async UniTask<string> AsyncTest()
        {
            await UniTask.DelayFrame(100);
            Debug.Log("123");
            return "sdff";
        }

    }
}
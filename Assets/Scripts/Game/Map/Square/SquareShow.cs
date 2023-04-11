using System;
using Managers;
using UnityEngine;

namespace Game
{
    public partial class Square
    {

        public void OnReact()
        {
            switch (Data)
            {
                case EnemySaveData d:
                    OnReactEnemy(d);
                    break;
                case SupplySaveData supplySaveData:
                    OnReactSupply(supplySaveData);
                    break;
                case RockSaveData rockSaveData:
                    break;
                case ShopSaveData shopSaveData:
                    WindowManager.Instance.ShopPanel.Open(shopSaveData);
                    break;
                case ChestSaveData chestSaveData:
                    AudioPlayer.Instance.Play(AudioPlayer.AudioOpenChest);
                    WindowManager.Instance.OffersWindow.Open(chestSaveData.Offers);
                    break;
            }
        }

        private void OnReactEnemy(EnemySaveData d)
        {
            GameObject curEffectObject = ObjectPoolManager.Instance.SpawnAttackEffect();
            curEffectObject.transform.position = Icon.transform.position;
                
            AudioPlayer.Instance.Play(AudioPlayer.AudioNormalAttack);
                
            EnemyBp enemyBp = d.Bp;
            if (enemyBp != null && enemyBp.Rank >= Rank.Rare)
            {
                AudioPlayer.Instance.SwitchBossOrNormalBGM(false);
            }
            else
            {
                AudioPlayer.Instance.SwitchBossOrNormalBGM(true);
            }
        }

        private void OnReactSupply(SupplySaveData supplySaveData)
        {
            switch (supplySaveData.Type)
            {
                case SupplyType.Spring:
                    PlaySoundEffect("spring");
                    break;
                case SupplyType.Grassland:
                    PlaySoundEffect("grassland");
                    break;
                case SupplyType.Camp:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        
        public void HandleReactResultArgs(Args args)
        {
            switch (args)
            {
                case EnemyArgs args0:
                    break;
                case CasinoArgs args1:
                    PlaySoundEffect(args1.Win? "casino_win" : "casino_lose");
                    if (args1.Win)
                    {
                        WindowManager.Instance.OffersWindow.Open(args1.Offers);
                    }
                    break;
                case DoorArgs doorArgs:
                    PlaySoundEffect(doorArgs.CanReact? "door": "blocked");
                    break;
                case RockArgs rockArgs:
                    PlaySoundEffect(rockArgs.CanReact? AudioPlayer.AudioClearRock : null);
                    break;
                
            }
        }


        private static void PlaySoundEffect(string id)
        {
            AudioPlayer.Instance.Play(id);
        }
    }
}
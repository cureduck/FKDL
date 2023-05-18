using System;
using Managers;
using UnityEngine;
using UI;

namespace Game
{
    public partial class Square
    {
        private void OnSquareDestroy()
        {
            if (Data is EnemySaveData)
            {
                WindowManager.Instance.EnemyPanel.Close();
            }

            if ((Data is EnemySaveData ee) && (ee.Bp.Rank == Rank.Rare))
            {
                WindowManager.Instance.CrystalPanel.Open(
                    (
                        GameManager.Instance.PlayerData,
                        CrystalManager.Instance.Lib["boss"]
                    ));
            }

            try
            {
                UpdateFace();
                //UnFocus();
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
            //DestroyImmediate(gameObject);
        }


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
                    AudioPlayer.Instance.Play(AudioPlayer.AudioOpenShop);
                    WindowManager.Instance.ShopPanel.Open(shopSaveData);
                    PlayerMainPanel.Instance.SetUsePotionState(true);
                    break;
                case ChestSaveData chestSaveData:
                    AudioPlayer.Instance.Play(AudioPlayer.AudioOpenChest);
                    Debug.Log(chestSaveData.Offers.Length);
                    WindowManager.Instance.OffersWindow.Open(chestSaveData.Offers);
                    break;
                case CrystalSaveData crystalPanel:
                    AudioPlayer.Instance.Play(AudioPlayer.AudioCrystal);

                    var panel = WindowManager.Instance.CrystalPanel;
                    WindowManager.Instance.CrystalPanel.Open(
                        (GameManager.Instance.PlayerData, CrystalManager.Instance.Lib[crystalPanel.Id])
                    );
                    panel.gameObject.SetActive(true);
                    break;
                case DoorSaveData doorSaveData:

                    break;
            }
        }

        private void OnReactEnemy(EnemySaveData d)
        {
            WindowManager.Instance.EnemyPanel.Open(new EnemyInfoPanel.Args
                { targetEnemy = d, playerData = GameManager.Instance.PlayerData });

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


        private void HandleEnemyArgs(EnemyArgs args0)
        {
            GameObject curEffectObject = ObjectPoolManager.Instance.SpawnAttackEffect();
            curEffectObject.transform.position = Icon.transform.position;
            AudioPlayer.Instance.Play(AudioPlayer.AudioNormalAttack);
            Attack result = args0.PlayerAttack.GetValueOrDefault();
            //GameObject temp = ObjectPoolManager.Instance.SpawnDamageSignEffect(10, 0);
            float range = 0.5f;
            //temp.transform.position = Icon.transform.position + new Vector3(UnityEngine.Random.Range(-range, range), UnityEngine.Random.Range(-range, range));
            //Debug.Log(result);
            if (result.PDmg > 0)
            {
                GameObject cur = ObjectPoolManager.Instance.SpawnDamageSignEffect(result.PDmg, 0);
                cur.transform.position = Icon.transform.position + new Vector3(UnityEngine.Random.Range(-range, range),
                    UnityEngine.Random.Range(-range, range));
            }

            if (result.MDmg > 0)
            {
                GameObject cur = ObjectPoolManager.Instance.SpawnDamageSignEffect(result.MDmg, 1);
                cur.transform.position = Icon.transform.position + new Vector3(UnityEngine.Random.Range(-range, range),
                    UnityEngine.Random.Range(-range, range));
            }

            if (result.CDmg > 0)
            {
                GameObject cur = ObjectPoolManager.Instance.SpawnDamageSignEffect(result.CDmg, 2);
                cur.transform.position = Icon.transform.position + new Vector3(UnityEngine.Random.Range(-range, range),
                    UnityEngine.Random.Range(-range, range));
            }
        }


        public void HandleReactResultArgs(Args args)
        {
            switch (args)
            {
                case EnemyArgs args0:
                    HandleEnemyArgs(args0);
                    break;
                case CasinoArgs args1:
                    PlaySoundEffect(args1.Win ? "casino_win" : "casino_lose");
                    if (args1.Win)
                    {
                        WindowManager.Instance.OffersWindow.Open(args1.Offers);
                    }

                    break;
                case DoorArgs doorArgs:
                    PlaySoundEffect(doorArgs.CanReact ? "door" : "blocked");
                    break;
                case RockArgs rockArgs:
                    PlaySoundEffect(rockArgs.CanReact ? AudioPlayer.AudioClearRock : null);
                    break;
            }
        }


        private static void PlaySoundEffect(string id)
        {
            AudioPlayer.Instance.Play(id);
        }
    }
}
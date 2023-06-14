using System;
using Managers;
using Newtonsoft.Json;
using UnityEngine;

namespace Game
{
    public sealed partial class Square
    {
        [JsonIgnore] PlayerData _player => GameManager.Instance.Player;

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
                        GameManager.Instance.Player,
                        CrystalManager.Instance.Lib["boss"],
                        "UI_MagicCrystal_BossReword_Title"
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

        private void OnFocusLogic()
        {
            switch (Data)
            {
                case EnemySaveData enemy:
                    DisplayEnemyInfoView(enemy);
                    break;
                default:
                    break;
            }
        }


        public void OnReactLogic()
        {
            AudioPlayer.Instance.SwitchBossOrNormalBGM(true);
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
                    break;
                case ChestSaveData chestSaveData:
                    AudioPlayer.Instance.Play(AudioPlayer.AudioOpenChest);
                    //Debug.Log(chestSaveData.Offers.Length);
                    switch (chestSaveData.RewardType)
                    {
                        case Offer.OfferKind.Skill:
                            WindowManager.Instance.OffersWindow.Open(
                                (chestSaveData.Offers, "UI_OfferPanel_Title_Skill", chestSaveData.SkipGold));
                            break;
                        case Offer.OfferKind.Potion:
                            WindowManager.Instance.OffersWindow.Open((chestSaveData.Offers,
                                "UI_OfferPanel_Title_Potion", chestSaveData.SkipGold));
                            break;
                        case Offer.OfferKind.Relic:
                            WindowManager.Instance.OffersWindow.Open(
                                (chestSaveData.Offers, "UI_OfferPanel_Title_Relic", chestSaveData.SkipGold));
                            break;
                        default:
                            WindowManager.Instance.OffersWindow.Open(
                                (chestSaveData.Offers, "UI_OfferPanel_Title_Other", chestSaveData.SkipGold));
                            break;
                    }

                    break;
                case CrystalSaveData crystalPanel:
                    AudioPlayer.Instance.Play(AudioPlayer.AudioCrystal);

                    var panel = WindowManager.Instance.CrystalPanel;
                    WindowManager.Instance.CrystalPanel.Open(
                        (GameManager.Instance.Player, CrystalManager.Instance.Lib[crystalPanel.Id],
                            "UI_MagicCrystal_Title")
                    );
                    panel.gameObject.SetActive(true);
                    break;
                case StairsSaveData stairsSaveData:
                    WindowManager.Instance.effectPanel.enterNewAreaEffect.Play();
                    break;
                case DoorSaveData doorSaveData:

                    break;
            }
        }


        private void OnReactEnemy(EnemySaveData d)
        {
            DisplayEnemyInfoView(d);
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

        private void DisplayEnemyInfoView(EnemySaveData d)
        {
            if (!WindowManager.Instance.EnemyPanel.IsOpen && d.IsAlive)
            {
                var tmp = transform.position;
                tmp.x = Icon.transform.position.x;
                WindowManager.Instance.EnemyPanel.Open((_player, d, this, tmp));
            }
        }


        private void OnLogicDone()
        {
            switch (Data)
            {
                case EnemySaveData enemy:
                    WindowManager.Instance.EnemyPanel.Close();
                    break;
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
                        WindowManager.Instance.OffersWindow.Open
                        (
                            (
                                args1.Offers, "UI_OfferPanel_Title_Other",
                                args1.SkipCompensate
                            )
                        );
                    }

                    break;
                case DoorArgs doorArgs:
                    PlaySoundEffect(doorArgs.CanReact ? "door" : "door_knocked");
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
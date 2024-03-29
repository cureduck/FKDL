﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Game;
using I2.Loc;
using Sirenix.OdinInspector;
using Tools;
using UI;
using UnityEngine;
using UnityEngine.Serialization;
using static GameKeywords;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Managers
{
    [RequireComponent(typeof(LocalizationParamsManager))]
    public class GameManager : Singleton<GameManager>
    {
        public Square Prefab;

        public Transform MapGo;

        public Square Focus;

        private ObjectPool<Square> _pool;

        [FormerlySerializedAs("GlobalLocalizationParamsManager")]
        private LocalizationParamsManager GLPM;


        public Dictionary<string, Color> SquareColors;

        private List<Square> squares = new List<Square>();
        public string CurFloor => $"{Map.CurrentFloor}";

        public PlayerData Player
        {
            get => GameDataManager.Instance.PlayerData;
            set => GameDataManager.Instance.PlayerData = value;
        }

        public Map Map
        {
            get => GameDataManager.Instance.Map;
            private set => GameDataManager.Instance.Map = value;
        }

        public SecondaryData SecondaryData
        {
            get => GameDataManager.Instance.SecondaryData;
            private set => GameDataManager.Instance.SecondaryData = value;
        }


        public bool InBattle => (Focus?.Data is EnemySaveData e) && (e.IsAlive);

        private void Start()
        {
            GLPM = GetComponent<LocalizationParamsManager>();
#if UNITY_EDITOR
            GetLocalization();
#endif
            _pool = new ObjectPool<Square>(CreateSquare);

            if (Application.isPlaying)
            {
                if (SceneSwitchManager.Instance.NewGame)
                {
                    LoadFromInit();
                }
                else
                {
                    LoadFromSave();
                }
            }

            SetGlobalLocalizationParams();
        }


        private void OnApplicationQuit()
        {
            Save();
        }

        private void GetProfRelic()
        {
            var prof = PlayerData.ProfInfo[0].ToUpper();
            if (RelicData.ProfRelic.TryGetValue(prof, out var relic))
            {
                Player.TryTakeRelic(relic, out var _);
            }
        }


        private void SetGlobalLocalizationParams()
        {
            void Set(string key, string value)
            {
                GLPM.SetParameterValue(key, value);
            }

            void SetParam()
            {
                Set("player_max_hp", Player.Status.MaxHp.ToString());
                Set("player_cur_hp", Player.Status.CurHp.ToString());
                Set("player_max_mp", Player.Status.MaxMp.ToString());
                Set("player_cur_mp", Player.Status.CurMp.ToString());
                Set("player_patk", Player.Status.PAtk.ToString());
                Set("player_matk", Player.Status.MAtk.ToString());
                Set("player_pdef", Player.Status.PDef.ToString());
                Set("player_mdef", Player.Status.MDef.ToString());
                Set("player_lucky_chance", Player.LuckyChance.ToString("player_lucky_chance"));
                Set("main_prof", PlayerData.ProfInfo[0].ToUpper());
            }

            SetParam();

            Player.OnUpdated += SetParam;
        }


        private void FindAndSetFocus()
        {
            Focus = squares.Find(square => square.Data.SquareState == SquareState.Focus);
        }

        public Square FindStartSquare()
        {
            return squares.Find(square => square.Data is StartSaveData);
        }

        public void SetCameraMoveRange(out Vector2 leftDownPoint, out Vector2 rightUpPoint)
        {
            leftDownPoint = new Vector2(9999999, 9999999);
            rightUpPoint = new Vector2(-9999999, -9999999);

            for (int i = 0; i < squares.Count; i++)
            {
                if (!squares[i].gameObject.activeInHierarchy) continue;

                if (squares[i].Icon.transform.position.x > rightUpPoint.x)
                {
                    rightUpPoint.x = squares[i].Icon.transform.position.x;
                }

                if (squares[i].Icon.transform.position.x < leftDownPoint.x)
                {
                    leftDownPoint.x = squares[i].Icon.transform.position.x;
                }

                if (squares[i].Icon.transform.position.y > rightUpPoint.y)
                {
                    rightUpPoint.y = squares[i].Icon.transform.position.y;
                }

                if (squares[i].Icon.transform.position.y < leftDownPoint.y)
                {
                    leftDownPoint.y = squares[i].Icon.transform.position.y;
                }
            }

            if (CameraMove.Instance)
            {
                CameraMove.Instance.leftDownPoint = leftDownPoint - new Vector2(2, 2);
                CameraMove.Instance.rightUpRange = rightUpPoint + new Vector2(2, 2);
            }
        }

#if UNITY_EDITOR

        [MenuItem("Tools/GetLocalization")]
#endif
        private static void GetLocalization()
        {
            var localizationFilePath = "Assets/Localization";
            try
            {
                var localizationFiles = Directory.GetFiles(localizationFilePath);
                Debug.Log(localizationFiles.Length);
                foreach (var fileName in localizationFiles)
                {
                    if (fileName.EndsWith(".csv"))
                    {
                        var f = File.ReadAllText(fileName);
                        Debug.Log(fileName);
                        f = f.Replace("\r\n", "\n");
                        Debug.Log($"加载当前文件数据----》{fileName}");
                        Debug.Log(LocalizationManager.Sources.Count);
                        Resources.Load<LanguageSourceAsset>("I2Languages").SourceData
                            .Import_CSV("", f, eSpreadsheetUpdateMode.Merge);
                        //LocalizationManager.Sources[0].Import_CSV();
                        Debug.Log("当前文件加载完毕");
                    }
                }
#if UNITY_EDITOR
                LanguageSourceAsset languageSourceAsset = Resources.Load<LanguageSourceAsset>("I2Languages");
                EditorUtility.SetDirty(languageSourceAsset);
                AssetDatabase.Refresh();
                AssetDatabase.SaveAssets();
#endif
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                Console.WriteLine(e);
            }

            Debug.Log("GetLocalization success");
        }

        public event Action GameLoaded;
        public event Action<string> Marched;
        public event Action<Square> FocusChanged;

        public void BroadcastSquareChanged(Square square)
        {
            FocusChanged?.Invoke(square);
        }

        private Square CreateSquare()
        {
            return Instantiate(Prefab, MapGo);
        }

        public void RollForSkill(int rank)
        {
            RollForSkill((Rank)rank);
        }

        public void RollForPotion(int rank)
        {
            var potions = PotionManager.Instance.GenerateT((Rank)rank, Player.LuckyChance, 3);

            var offers = potions.Select((s => new Offer(s)));

            WindowManager.Instance.OffersWindow.Load(offers, "Debug Reward");
        }


        public void Attack()
        {
            if (Player.Enemy != null)
            {
                Player.OnReact();
            }
        }

        public void RollForSkill(Rank rank)
        {
            var skills = SkillManager.Instance.GenerateT(rank, Player.LuckyChance, 3);

            var offers = skills.Select((s => new Offer(s)));

            WindowManager.Instance.OffersWindow.Load(offers, "UI_OfferPanel_Title_Skill");
        }

        public void RollForRelic(int rank)
        {
            RollForRelic((Rank)rank);
        }


        public void RollForRelic(Rank rank)
        {
            var skills = RelicManager.Instance.RollT(rank, 3);

            var offers = skills.Select((s => new Offer(s)));

            WindowManager.Instance.OffersWindow.Load(offers, "UI_OfferPanel_Title_Relic");
        }


        [Button]
        public void LoadFromSave()
        {
            try
            {
                Player = PlayerData.LoadFromSave();
                SecondaryData = SecondaryData.LoadFromSave();
                SecondaryData.SetRandom();
                Map = Map.LoadFromSave();
                Player.BroadCastUpdated();
                LoadMap();
                FindAndSetFocus();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            GameLoaded?.Invoke();
            //GC.Collect();
        }


        private void GetGifts()
        {
            foreach (var gift in SecondaryData.Gifts)
            {
                foreach (var command in gift.Commands)
                {
                    command.Execute(Player);
                }
            }
        }

        private void ApplyNightmare()
        {
            foreach (var nightmare in SecondaryData.Nightmares)
            {
                Nightmares.ApplyNightMare(nightmare, Map);
                Player.TryTakeRelic(nightmare, out _);
            }
        }

        [Button]
        public void LoadFromInit()
        {
            SecondaryData.Init();
            SecondaryData.SetRandom();
            Player = PlayerData.LoadFromInit();
            Player.BroadCastUpdated();
            Map = Map.LoadFromInit();
            Map.Init();
            LoadMap();
            GetProfRelic();
            GetGifts();
            ApplyNightmare();
            GameLoaded?.Invoke();
            //GameManager.Instance.PlayerData.Gain(10000);
            //GC.Collect();
        }

        [Button]
        public void Save()
        {
            Player.Save();
            Map.Save();
            //SecondaryData.CurCardSeed = CardRand.
            SecondaryData.Save();
        }


        private void LoadMap()
        {
            GameManager.Instance.Focus = null;
            LoadFloor(Map.Floors[Map.CurrentFloor]);
            ScanEnemyLegal();
        }

        private void ScanEnemyLegal()
        {
            foreach (var floor in Map.Floors.Values)
            {
                foreach (var sq in floor.Squares)
                {
                    if (sq is EnemySaveData enemy)
                    {
                        try
                        {
                            if (!EnemyManager.Instance.EnemyBps.ContainsKey(enemy.Id))
                            {
                                Debug.LogError($"{enemy.Id} not found, in {floor.FloorName}");
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            throw;
                        }
                    }
                }
            }
        }

        public void LoadFloor(Map.Floor floor)
        {
            Marched?.Invoke(floor.FloorName);
            foreach (var square in squares)
            {
                square.UnbindCurrent();
                square.gameObject.SetActive(false);
                Destroy(square);
                //_pool.Return(square);
            }


            squares.Clear();
            /*foreach (Transform trans in MapGo)
            {
                _pool.Return(trans.gameObject.GetComponent<>());
            }*/


            foreach (var square in floor.Squares)
            {
                if (square == null) continue;

                //Square curObject = CreateSquare(square);
                var sq = Instantiate(Prefab, MapGo);
                sq.Reload(square);
                if (sq != null)
                {
                    squares.Add(sq);
                }
            }

            SetCameraMoveRange(out _, out _);
        }


        public void GameOver()
        {
            WindowManager.Instance.GameOverWindow.Open(GetEndingData());
            //WindowManager.Instance.GameOverWindow.Load();
            if (File.Exists(Paths._savePath))
            {
                File.Delete(Paths._savePath);
            }

            SecondaryData.DeleteSave();
            var profile = Profile.GetOrCreate();
            profile.CollectedSouls += SecondaryData.RecentCollectedSouls;
            profile.Save();
        }

        [Button]
        private Profile GetProfile()
        {
            return Profile.GetOrCreate();
        }

        public GameOverData GetEndingData()
        {
            var collectedSouls = (ENDING_PARAM_SOULS, SecondaryData.RecentCollectedSouls.ToString());
            var killed = (ENDING_PARAM_KILLED, SecondaryData.Killed.Values.Sum().ToString());
            var deepestLevel = (ENDING_PARAM_FLOOR, GameManager.Instance.CurFloor.ToString());

            var endingParams = new (string id, string param)[]
            {
                collectedSouls,
                killed,
                deepestLevel
            };

            return new GameOverData()
            {
                EndingId = GetEndingId(),
                EndingParams = endingParams
            };
        }

        private string GetEndingId()
        {
            if (!Player.IsAlive)
            {
                return ENDIND_DEAD;
            }
            else
            {
                if (SecondaryData.Nightmares.Length == 5)
                {
                    return ENDING_TRUE;
                }
                else
                {
                    return ENDING_FALSE;
                }
            }
        }


        [Button]
        private Square CreateSquare(MapData data)
        {
            if (data == null) return null;

            var sq = Instantiate(Prefab, MapGo);
            //var sq = _pool.Get();
            sq.Reload(data);

            try
            {
                sq.Data = data;
                return sq;
            }
            catch (Exception e)
            {
                DestroyImmediate(sq.gameObject);
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Game;
using I2.Loc;
using Sirenix.OdinInspector;
using Tools;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

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

        public Profile Profile;

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
            Profile = Profile.GetOrCreate();
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
            }

            SetParam();

            Player.OnUpdated += SetParam;
        }


        public void FindAndSetFocus()
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
                CameraMove.Instance.leftDownPoint = leftDownPoint;
                CameraMove.Instance.rightUpRange = rightUpPoint;
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
                foreach (var fileName in localizationFiles)
                {
                    if (fileName.EndsWith(".csv"))
                    {
                        var f = File.ReadAllText(fileName);
                        f = f.Replace("\r\n", "\n");
                        LocalizationManager.Sources[0].Import_CSV("", f, eSpreadsheetUpdateMode.Merge);
                    }
                }
            }
            catch (Exception e)
            {
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
                _pool.Return(square);
            }

            squares.Clear();
            /*foreach (Transform trans in MapGo)
            {
                _pool.Return(trans.gameObject.GetComponent<>());
            }*/


            foreach (var square in floor.Squares)
            {
                Square curObject = CreateSquare(square);
                if (curObject != null)
                {
                    squares.Add(curObject);
                }
            }

            SetCameraMoveRange(out _, out _);
        }


        [Button]
        private Square CreateSquare(MapData data)
        {
            if (data == null) return null;

            //var sq = Instantiate(Prefab, MapGo);
            var sq = _pool.Get();
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
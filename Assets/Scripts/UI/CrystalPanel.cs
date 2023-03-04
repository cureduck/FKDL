using I2.Loc;
using Managers;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class CrystalPanel : Singleton<CrystalPanel>
    {
        public Localize Title;
        public Button OptionPrefab;

        public Transform OptionList;
        
        [Button]
        public void Load(string id)
        {
            var crystal = CrystalManager.Instance.Lib[id];
            Title.SetTerm(crystal.Title);

            foreach (Transform trans in OptionList)
            {
                Destroy(trans.gameObject);
            }
            
            foreach (var option in crystal.Options)
            {
                var tmp = Instantiate(OptionPrefab, OptionList);
                tmp.gameObject.SetActive(true);
                tmp.GetComponentInChildren<Localize>().SetTerm(option.Line);
                tmp.onClick.AddListener((() =>
                {
                    gameObject.SetActive(false);
                    GameManager.Instance.PlayerData.Execute(option.Effect);
                }));
            }
        }
    }
}
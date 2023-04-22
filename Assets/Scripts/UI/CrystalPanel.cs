using System;
using I2.Loc;
using Managers;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using Game;
using CH.ObjectPool;

namespace UI
{
    public class CrystalPanel : BasePanel<CrystalPanel.Args>
    {
        public class Args
        {
            public PlayerData playerData;
            public Crystal crystal;
        }
        
        [SerializeField]
        private Localize describe;
        [SerializeField]
        private CellCrystalOptionView crystalOptionPrefab;
        [SerializeField]
        private Transform OptionList;

        private UIViewObjectPool<CellCrystalOptionView, Crystal.Option> objectPools;

        public override void Init()
        {
            objectPools = new UIViewObjectPool<CellCrystalOptionView, Crystal.Option>(crystalOptionPrefab, null);
            gameObject.SetActive(false);
        }

        protected override void OnOpen()
        {
            describe.SetTerm(Data.crystal.Title);
            objectPools.SetDatas(Data.crystal.GetOptions(CrystalManager.Instance.Lib.Random), OnSet, OptionList);
            transform.SetAsLastSibling();
        }

        private void OnSet(CellCrystalOptionView cellCrystalOptionView, Crystal.Option data) 
        {
            cellCrystalOptionView.SetData(Data.playerData, Data.crystal.Id, data, CellOptionClick);
        }

        private void CellOptionClick(PlayerData playerData, Crystal.Option option) 
        {
            playerData.Execute(option.Effect);
            Close();
        }

        protected override void UpdateUI()
        {
            
        }
    }
}
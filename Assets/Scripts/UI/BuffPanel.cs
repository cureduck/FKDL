using System.Collections.Generic;
using Game;
using Sirenix.OdinInspector;
using UnityEngine;

namespace UI
{
    public class BuffPanel : FighterUIPanel
    {
        public BuffItem BuffItemPrefab;


        public override void SetMaster(FighterData master)
        {
            if (master == _master) return;

            UnbindPrevious();

            base.SetMaster(master);
            Bind();
            LoadBuffs();
        }


        protected override void UpdateData()
        {
            base.UpdateData();
            foreach (var buffItem in GetComponentsInChildren<BuffItem>())
            {
                buffItem.Load();
            }
        }

        private void UnbindPrevious()
        {
            if (_master == null) return;

            _master.Buffs.BuffAdded -= AddNewBuff;
            _master.Buffs.BuffRemoved -= RemoveBuff;
        }


        private void Bind()
        {
            _master.Buffs.BuffAdded += AddNewBuff;
            _master.Buffs.BuffRemoved += RemoveBuff;
        }

        [Button]
        private void LoadBuffs()
        {
            ClearCurrentBuffItems();

            for (int i = 0; i < _master.Buffs.Count; i++)
            {
                CreatePrefab(_master.Buffs[i]);
            }
        }

        private void AddNewBuff(BuffData data)
        {
            CreatePrefab(data);
        }

        private void RemoveBuff(BuffData data)
        {
            foreach (var buffItem in GetComponentsInChildren<BuffItem>())
            {
                if (buffItem.BuffData == data)
                {
                    Destroy(buffItem.gameObject);
                }
            }
        }


        private void CreatePrefab(BuffData data)
        {
            var go = Instantiate(BuffItemPrefab, transform);
            go.BuffData = data;
        }


        private void ClearCurrentBuffItems()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
        }
    }
}
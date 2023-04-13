using System;
using System.Collections.Generic;
using System.Linq;
using I2.Loc;
using Sirenix.OdinInspector;
using Tools;
using UnityEngine;

namespace UI
{
    public class ListPanel<T> : SerializedMonoBehaviour where T : class
    {
        public List<CellView<T>> Cells;
        public Transform ListTransform;

        public CellView<T> CellPrefab;

        private ObjectPool<CellView<T>> _pool;

        private void Start()
        {
            _pool = new ObjectPool<CellView<T>>(Generator);
        }

        private CellView<T> Generator()
        {
            var t = Instantiate(CellPrefab, ListTransform);
            t.gameObject.SetActive(true);
            return t;
        }

        public void UpdateUI(IEnumerable<T> datas)
        {
            var delayList = Cells.FindAll((view => !datas.Contains(view.Data)));
            Cells.RemoveAll((view => delayList.Contains(view)));
            foreach (var cell in delayList)
            {
                cell.gameObject.SetActive(false);
                _pool.Return(cell);
            }
            
            foreach (var d in datas)
            {
                var tmp = Cells.Find((view => view.Data == d));
                if (tmp == null)
                {
                    tmp = _pool.Get();
                    tmp.gameObject.SetActive(true);
                    tmp.Data = d;
                    tmp.UpdateUI();
                    Cells.Add(tmp);
                }
                else
                {
                    tmp.UpdateUI();
                }
            }
        }
        
        
    }
}
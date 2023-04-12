using System.Collections.Generic;
using System.Linq;
using I2.Loc;
using Sirenix.OdinInspector;
using UnityEngine;

namespace UI
{
    public class ListPanel<T> : SerializedMonoBehaviour where T : class
    {
        public List<CellView<T>> Cells;
        public Transform ListTransform;

        public CellView<T> CellPrefab;
        
        public void UpdateUI(IEnumerable<T> datas)
        {
            var delayList = Cells.FindAll((view => !datas.Contains(view.Data)));
            Cells.RemoveAll((view => delayList.Contains(view)));
            foreach (var cell in delayList)
            {
                cell.Removed();
            }
            
            foreach (var d in datas)
            {
                var tmp = Cells.Find((view => view.Data == d));
                if (tmp == null)
                {
                    tmp = Instantiate(CellPrefab, ListTransform);
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
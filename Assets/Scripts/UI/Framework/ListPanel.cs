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
        
        public void UpdateUI(IEnumerable<T> data)
        {
            var delayList = Cells.FindAll((view => !data.Contains(view.Data)));
            Cells.RemoveAll((view => delayList.Contains(view)));
            foreach (var view in delayList)
            {
                Destroy(view.gameObject);
            }
            
            foreach (var d in data)
            {
                var tmp = Cells.Find((view => view.Data == d));
                if (tmp == null)
                {
                    tmp = Instantiate(CellPrefab, ListTransform);
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
using System;
using System.Collections.Generic;
using System.Linq;
using Managers;
using Sirenix.OdinInspector;

namespace Game
{
    public class BuffAgent : List<BuffData>
    {
        public event Action<BuffData> BuffAdded;
        public event Action<BuffData> BuffRemoved;


        public BuffAgent() : base()
        {
            foreach (var  buff in this)
            {
                buff.Removed += () => { Remove(buff); };
            }
        }
        
        
        public new void Add(BuffData data)
        {
            var d = (BuffData)data.Clone();
            d.Id = d.Id.ToLower();
            if (BuffManager.Instance.Lib.TryGetValue(d.Id, out _))
            {
                var buff = Find((buffData => buffData.Id == d.Id));
                if (buff == null)
                {
                    base.Add(d);
                    BuffAdded?.Invoke(d);
                }
                else
                {
                    buff.CurLv += d.CurLv;
                }
            }
        }

        public new void Remove(BuffData data)
        {
            BuffRemoved?.Invoke(data);
            base.Remove(data);
        }


        public void RemoveZeroStackBuff()
        {
            var tmp = new LinkedList<BuffData>();
            foreach (BuffData buff in this.Where(buff => buff.CurLv == 0))
            {
                tmp.AddLast(buff);
            }

            foreach (var buff in tmp)
            {
                Remove(buff);
                BuffRemoved?.Invoke(buff);
            }
            
        }
        
    }
}
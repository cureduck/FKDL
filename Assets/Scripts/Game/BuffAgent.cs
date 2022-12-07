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
            data.Id = data.Id.ToLower();
            if (BuffManager.Instance.Lib.TryGetValue(data.Id, out _))
            {
                var buff = Find((buffData => buffData.Id == data.Id));
                if (buff == null)
                {
                    base.Add(data);
                    BuffAdded?.Invoke(data);
                }
                else
                {
                    buff.CurLv += data.CurLv;
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
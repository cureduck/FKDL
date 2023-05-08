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
            foreach (var buff in this)
            {
                buff.Removed += () => { Remove(buff); };
            }
        }

        public void Add_Test(BuffData data)
        {
            base.Add(data);
        }


        public new void Add(BuffData data)
        {
            var d = (BuffData)data.Clone();
            //d.Id = d.Id.ToLower();
            if (BuffManager.Instance.TryGetById(d.Id, out var dBp))
            {
                var oppositeId = dBp.OppositeId.ToLower();
                var buff = Find((buffData => buffData.Id == d.Id || buffData.Id == oppositeId));
                if (buff == null)
                {
                    base.Add(d);
                    BuffAdded?.Invoke(d);
                }
                else
                {
                    if (buff.Id == d.Id)
                    {
                        buff.StackChange(d.CurLv);
                    }
                    else
                    {
                        buff.StackChange(-d.CurLv);
                    }
                }
            }

            RemoveZeroStackBuff();
        }

        public new void Remove(BuffData data)
        {
            BuffRemoved?.Invoke(data);
            base.Remove(data);
        }


        public void RemoveZeroStackBuff()
        {
            RemoveAll(data =>
            {
                BuffRemoved?.Invoke(data);
                return data.CurLv <= 0;
            });
        }

        public override string ToString()
        {
            return string.Join("-", this);
        }
    }
}
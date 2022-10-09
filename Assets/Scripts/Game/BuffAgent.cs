using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace Game
{
    public class BuffAgent : List<BuffData>
    {
        public event Action<BuffData> BuffAdded;
        public event Action<BuffData> BuffRemoved;

        public new void Add(BuffData data)
        {
            base.Add(data);
            BuffAdded?.Invoke(data);
        }

        public new void Remove(BuffData data)
        {
            base.Remove(data);
            BuffRemoved?.Invoke(data);
        }
        
        
    }
}
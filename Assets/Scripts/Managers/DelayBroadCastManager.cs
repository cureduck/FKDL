using System;
using System.Collections.Generic;
using UI;

namespace Managers
{
    public class DelayBroadCastManager : Singleton<DelayBroadCastManager>
    {
        protected override void Awake()
        {
            base.Awake();
            List = new HashSet<IUpdateable>();
        }

        private void LateUpdate()
        {
            foreach (var updateable in List)
            {
                updateable.BroadCastUpdated();
            }
            List.Clear();
        }

        public HashSet<IUpdateable> List;

        public void Add(IUpdateable updateable)
        {
            if (!List.Contains(updateable))
            {
                List.Add(updateable);
            }
        }
    }
}
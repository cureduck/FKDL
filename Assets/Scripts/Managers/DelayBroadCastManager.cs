using System;
using System.Collections.Generic;
using Game;
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
            try
            {
                foreach (var updateable in List)
                {
                    updateable.BroadCastUpdated();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                List.Clear();
            }
        }

        public HashSet<IUpdateable> List;

        public void Add(IUpdateable updateable)
        {
            List.Add(updateable);
        }
    }
}
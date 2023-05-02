using System;
using System.Collections.Generic;
using System.Linq;
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
                while (List.Any())
                {
                    List.First().BroadCastUpdated();
                    List.Remove(List.First());
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
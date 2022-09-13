using System;
using Game;
using UnityEngine;

namespace Managers
{
    public class WindowManager : Singleton<WindowManager>
    {
        public GameObject OffersWindow;


        public void Display(EnemySaveData data)
        {
            
        }
        
        
        public void Warn(string log)
        {
            throw new NotImplementedException();
        }
    }
}
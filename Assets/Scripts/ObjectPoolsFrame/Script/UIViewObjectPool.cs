using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CH.ObjectPool
{
    public class UIViewObjectPool<MONO,ARGS> where MONO :MonoBehaviour
    {
        private List<MONO> curActive;
        private Stack<MONO> curUnactive;
        private MONO prefab;
        private System.Action<MONO, ARGS> onInit;

        public UIViewObjectPool(MONO prefab,System.Action<MONO, ARGS> onInit)
        {
            this.prefab = prefab;
            this.onInit = onInit;
            curActive = new List<MONO>();
            curUnactive = new Stack<MONO>();
        }

        public void SetDatas(IEnumerable<ARGS> args,System.Action<MONO,ARGS> onSet, Transform spawnTargetParent) 
        {
            int loopCount = 0;

            foreach (var c in args)
            {
                MONO curMono;
                if (loopCount >= curActive.Count)
                {
                    if (curUnactive.Count <= 0)
                    {
                        curMono = GameObject.Instantiate(prefab);
                        curMono.transform.SetParent(spawnTargetParent);
                        curMono.transform.localScale = Vector3.one;
                        onInit?.Invoke(curMono, c);

                    }
                    else
                    {
                        curMono = curUnactive.Pop();
                        curMono.gameObject.SetActive(true);
                    }
                    curActive.Add(curMono);
                }
                else 
                {
                    curMono = curActive[loopCount];
                }
                onSet?.Invoke(curMono,c);
                loopCount++;
            }
            while (loopCount < curActive.Count) 
            {
                MONO curUnActive = curActive[loopCount];
                curActive.RemoveAt(loopCount);
                curUnactive.Push(curUnActive);
                curUnActive.gameObject.SetActive(false);
            }


        }

        public int ActiveCount() 
        {
            return curActive.Count;
        }

        public MONO GetCurActiveMemberByIndex(int index) 
        {
            return curActive[index];
        }

    }
}
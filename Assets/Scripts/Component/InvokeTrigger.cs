using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvokeTrigger : MonoBehaviour
{
    public float lateTime;
    public System.Action onTimeEvent;

    public void Set(float lateTime, System.Action onTimeEvent)
    {
        this.lateTime = lateTime;
        this.onTimeEvent = onTimeEvent;
        StartCoroutine(TimerIE());
    }

    private IEnumerator TimerIE()
    {
        yield return new WaitForSeconds(lateTime);
        onTimeEvent?.Invoke();
    }
}
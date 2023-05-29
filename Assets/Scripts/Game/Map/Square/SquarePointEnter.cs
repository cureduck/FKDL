using DG.Tweening;
using Managers;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

namespace Game
{
    public class SquarePointEnter : MonoBehaviour
    {
        [SerializeField] private const float targetIntensity = 2.5f;

        [SerializeField] private const float changeSpeed = .7f;

        //private static SquarePointEnter curClick;
        [SerializeField] private Light2D targetLight;
        [SerializeField] private GameObject targetMask;

        [SerializeField] private Square square;
        //private float curTargetIntensity = 0;

        private Tween anim;

        private void Start()
        {
            targetLight.intensity = 0;
        }

        private void Update()
        {
            //targetLight.intensity = Mathf.Lerp(targetLight.intensity, curTargetIntensity, changeSpeed * Time.deltaTime);
        }

        private void OnMouseEnter()
        {
            var data = square.Data;
            if (((data.SquareState & SquareState.Revealed) != 0) && !(data is EnemySaveData))
            {
                WindowManager.Instance.SquareInfoPanel.Open(data.GetSquareInfo());
                var v = transform.position;
                v.x += (float)data.Placement.Width / 2;
                WindowManager.Instance.SquareInfoPanel.transform.position = v;
            }
            else
            {
                WindowManager.Instance.SquareInfoPanel.Close();
            }


            if (!targetMask.activeInHierarchy)
            {
                //curTargetIntensity = targetIntensity;
                anim.Kill();
                anim = DOTween.To(Getter, Setter, targetIntensity, changeSpeed);

                if (square.Data.SquareState == SquareState.UnFocus || square.Data.SquareState == SquareState.Focus)
                {
                    SquareInfo squareInfo = square.Data.GetSquareInfo();
                    //WindowManager.Instance.simpleInfoItemPanel.Open(new SimpleInfoItemPanel.Args { title = squareInfo.Name, describe = squareInfo.Desc, worldTrans = transform });
                }
            }
        }

        private void OnMouseExit()
        {
            if (targetMask.activeInHierarchy) return;

            //if (square.Data.SquareState == SquareState.UnFocus || square.Data.SquareState == SquareState.Focus)
            //{
            //    WindowManager.Instance.simpleInfoItemPanel.Close();
            //}

            anim.Kill();
            anim = DOTween.To(Getter, Setter, 0, changeSpeed / 1.5f)
                .OnComplete(() => Setter(0f));
            //curTargetIntensity = 0;
        }

        private void OnMouseUp()
        {
            if (targetMask.activeInHierarchy) return;
            anim.Kill();

            anim = DOTween.To(Getter, Setter, 0, 0.1f)
                .OnComplete(() => Setter(0f));

            /*if (curClick != null) 
        {
            curClick.curTargetIntensity = 0;
        }
        curTargetIntensity = targetIntensity;
        curClick = this;*/
        }


        private float Getter()
        {
            return targetLight.intensity;
        }

        private void Setter(float f)
        {
            targetLight.intensity = f;
        }
    }
}
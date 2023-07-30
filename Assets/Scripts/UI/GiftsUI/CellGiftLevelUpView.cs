using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class CellGiftLevelUpView : MonoBehaviour
    {
        [SerializeField] private Image progressView;
        [SerializeField] private float needTime;
        [SerializeField] private GameObject completeSign;
        public bool isProgress;
        private float curProgress;

        private System.Action onComplete;


        // Update is called once per frame
        void Update()
        {
            if (curProgress <= 0 && !isProgress)
            {
                curProgress = 0;
                progressView.fillAmount = curProgress;
                return;
            }
            else if (curProgress >= 1 && isProgress)
            {
                curProgress = 1;
                progressView.fillAmount = curProgress;
                return;
            }

            if (isProgress)
            {
                curProgress += 1 / needTime * Time.deltaTime;
                if (curProgress >= 1)
                {
                    isProgress = false;
                    curProgress = 0;

                    completeSign.SetActive(false);
                    completeSign.SetActive(true);

                    onComplete?.Invoke();

                    //Debug.Log("Complete!");
                }
            }
            else
            {
                curProgress -= 1 / needTime * Time.deltaTime;
            }

            progressView.fillAmount = curProgress;
        }

        public void Init(System.Action onComplete)
        {
            this.onComplete = onComplete;
            completeSign.SetActive(false);
        }
    }
}
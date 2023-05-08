using DG.Tweening;
using Game;
using I2.Loc;
using TMPro;

namespace Managers
{
    public class HelpInfoManager : Singleton<HelpInfoManager>
    {
        public Localize Helper;

        private Tween dt;

        public void SetTerm(string term)
        {
            Helper.gameObject.SetActive(true);
            Helper.SetTerm(term);
            Helper.GetComponent<TMP_Text>().alpha = 1f;
            dt.Kill();
            dt = Helper.GetComponent<TMP_Text>().DOFade(0.2f, 3f)
                .OnComplete(() => Helper.gameObject.SetActive(false));
        }
    }
}
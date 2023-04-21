using Managers;
using TMPro;

namespace UI
{
    public class FightPredictPanel : BasePanel<Arena>
    {
        public TMP_Text test;
        protected override void UpdateUI()
        {
            test.text = Data.ToString();
        }
    }
}
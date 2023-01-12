using Managers;
using UI;

namespace Game
{
    public class ShopSaveData : MapData
    {
        public override void OnReact()
        {
            base.OnReact();
            var shop =WindowManager.Instance.ShopPanel;
            shop.gameObject.SetActive(true);
            shop.Refresh();
            Destroyed();
        }
    }
}
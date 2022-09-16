using Game;
using UnityEngine;

namespace UI
{
    public class OfferWindow : MonoBehaviour
    {
        public OfferUI[] OfferUIs;
        public void Load(Offer[] offers)
        {
            for (int i = 0; i < offers.Length; i++)
            {
                OfferUIs[i].Offer = offers[i];
            }
            gameObject.SetActive(true);
        }
    }
}
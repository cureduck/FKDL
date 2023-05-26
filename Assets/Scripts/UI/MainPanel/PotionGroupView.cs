using Game;
using UnityEngine;

namespace UI
{
    public class PotionGroupView : FighterUIPanel
    {
        [SerializeField] private CellPotionView prefab;

        [SerializeField] private Transform listParent;

        //public int MinLen => math.min(((PlayerData) _master).Potions.Length, SkillItems.Length);

        public override void SetMaster(FighterData master)
        {
            base.SetMaster(master);
            UpdateData();
        }

        //private void UseMasterPotion(int index)
        //{
        //    ((PlayerData) _master).UsePotion(index);
        //}

        protected override void UpdateData()
        {
            //Debug.LogWarning("gengx!");
            PlayerData playerData = _master as PlayerData;
            if (playerData != null)
            {
                for (int i = 0; i < playerData.Potions.Length; i++)
                {
                    CellPotionView cellPotionView;
                    if (i >= listParent.childCount)
                    {
                        cellPotionView = Instantiate(prefab);
                        cellPotionView.transform.SetParent(listParent);
                        cellPotionView.transform.localScale = Vector3.one;
                        //cellPotionView.Init();
                    }
                    else
                    {
                        cellPotionView = listParent.GetChild(i).GetComponent<CellPotionView>();
                        cellPotionView.gameObject.SetActive(true);
                    }

                    cellPotionView.SetData(i, playerData.Potions[i]);
                }

                for (int i = playerData.Potions.Length; i < listParent.childCount; i++)
                {
                    listParent.GetChild(i).gameObject.SetActive(false);
                }
            }
        }
    }
}
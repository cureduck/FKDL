using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;

public class SkillListView : MonoBehaviour
{
    [SerializeField]
    private CellSkillView prefab;
    [SerializeField]
    private Transform listParent;

//    public void Start()
//    {
//        StartCoroutine(Temp());
//    }
//    private IEnumerator Temp() 
//    {
//        yield return new WaitForSeconds(1.0f);
//        SkillData[] skillDatas = new SkillData[]
//{
//            null,
//            null,
//            new SkillData { Cooldown = 0, Id = "YWLZ_ALC".ToLower(), CurLv = 1 },
//            new SkillData { Cooldown = 2, Id = "JSLC_ALC".ToLower(), CurLv = 2 },
//            null,
//            null,
//};
//        SetData(skillDatas);
//    }


    public void SetData(PlayerData playerData,SkillData[] curSkills)
    {
        for (int i = 0; i < curSkills.Length; i++)
        {
            CellSkillView cellSkillView;
            if (i >= listParent.childCount)
            {
                cellSkillView = Instantiate(prefab);
                cellSkillView.transform.SetParent(listParent);
                cellSkillView.transform.localScale = Vector3.one;
            }
            else 
            {
                cellSkillView = listParent.GetChild(i).GetComponent<CellSkillView>();
                cellSkillView.gameObject.SetActive(true);
            }
            cellSkillView.SetData(playerData,curSkills[i]);
        }

        for (int i = curSkills.Length; i < listParent.childCount; i++)
        {
            listParent.GetChild(i).gameObject.SetActive(false);
        }

    }

}

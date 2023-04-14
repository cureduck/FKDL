using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Game;
using Managers;

public class CareerInformationView : MonoBehaviour
{
    [SerializeField]
    private CellProfView main;
    [SerializeField]
    private CellProfView second01;
    [SerializeField]
    private CellProfView second02;

    public void SetData(string[] profInfos) 
    {
        main.SetData(profInfos[0]);
        second01.SetData(profInfos[1]);
        second02.SetData(profInfos[2]);
    }



}

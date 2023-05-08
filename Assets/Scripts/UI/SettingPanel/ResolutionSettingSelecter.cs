﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Managers;

[RequireComponent(typeof(TMP_Dropdown))]
public class ResolutionSettingSelecter : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown tMP_Dropdown;
    private Resolution[] resolutions;

    public void Start()
    {
        resolutions = Screen.resolutions;
        tMP_Dropdown.ClearOptions();
        List<string> options = new List<string>();
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = $"{resolutions[i].width}X{resolutions[i].height}";
            options.Add(option);
        }

        tMP_Dropdown.AddOptions(options);
        tMP_Dropdown.RefreshShownValue();
    }

    public void OnValueChange(int index)
    {
        Resolution resolution = resolutions[index];
        SettingManager.Instance.SetResolutionSize(new Vector2Int(resolution.width, resolution.height));
    }
}
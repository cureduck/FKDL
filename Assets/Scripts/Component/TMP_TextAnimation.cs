using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class TMP_TextAnimation : MonoBehaviour
{
    [SerializeField] private TMP_Text tmp_Text;
    [SerializeField] private Color targetColor;
    [SerializeField] private float targetTansSize;
    [SerializeField] private float transSpeed = 1;


    // Update is called once per frame
    void Update()
    {
        if (tmp_Text.text != null)
        {
            tmp_Text.color = Color.Lerp(tmp_Text.color, targetColor, transSpeed * Time.deltaTime);
            tmp_Text.transform.localScale = Vector3.Lerp(tmp_Text.transform.localScale, targetTansSize * Vector3.one,
                transSpeed * Time.deltaTime);
        }
    }

    public void SetTempColor(Color color)
    {
        tmp_Text.color = color;
    }

    public void SetTempFontSize(float localScale)
    {
        tmp_Text.transform.localScale = localScale * Vector3.one;
    }
}
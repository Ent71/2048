using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class NumberDisplay : MonoBehaviour
{
    private TMP_Text _text;

    private void Awake()
    {
        _text = gameObject.GetComponent<TMP_Text>();
    }

    public void SetTextValue(int value)
    {
        float fontSize = 2f;
        string stringValue = "";

        if(value >= 10000)
        {
            stringValue = (value / 1000).ToString() + "K";
        }
        else 
        {
            stringValue = value.ToString();
        }

        switch (stringValue.Length)
        {
            case 1:
                fontSize = 6f;
                break;
            case 2:
                fontSize = 5f;
                break;
            case 3:
                fontSize = 3.5f;
                break;
            case 4:
                fontSize = 2.5f;
                break;
        }

        _text.fontSize = fontSize;
        _text.text = stringValue;
    }
}

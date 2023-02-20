using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoolWindow : MonoBehaviour
{
    private bool isOn = false;
    [SerializeField] Text text_OnOff;
    [SerializeField] Slider slider;
    private Color color_On = new Color(0.717f, 0f, 0f);
    private Color color_Off = new Color(0f, 0.149f, 0.718f);

    void Update()
    {
        if (slider.value == 1)//ON
        {
            text_OnOff.text = "ON";
            text_OnOff.color = color_On;
            isOn = true;
        }
        else//OFF
        {
            text_OnOff.text = "OFF";
            text_OnOff.color = color_Off;
            isOn = false;
        }
    }

    public bool GetBool()
    {
        return isOn;
    }
}
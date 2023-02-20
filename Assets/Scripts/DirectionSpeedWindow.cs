using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DirectionSpeedWindow : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] InputField inputField;
    private float pValue;//変更前のスライダーの値
    private GameManager gm;

    private void Awake()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();

        //最初は１倍速
        slider.value = 0f;
        pValue = 0f;
        SliderValueToInputField();
    }

    /// <summary>
    /// スライダーの値を入力枠に反映させる
    /// </summary>
    public void SliderValueToInputField()
    {
        float fixedValue = slider.value < 0 ? 10f / (10f - slider.value) : (10f + slider.value) / 10f;
        pValue = slider.value;
        inputField.text = slider.value < 0 ? "1 / " + ((10f - slider.value) / 10f).ToString() : ((10f + slider.value) / 10f).ToString();
        gm.SetActionSpeed(fixedValue);
    }

    /// <summary>
    /// 入力枠の値をスライダーに反映させる
    /// </summary>
    public void InputFieldValueToSlider()
    {
        //数値変換できれば（1倍速以上であれば）
        if (float.TryParse(inputField.text, out float value))
        {
            value = value * 10f - 10f;
            value = Mathf.FloorToInt(value);//必ず小数点第一位までにする
            if (value < 0)
            {
                value = 0;
            }
            if (value > slider.maxValue)
            {
                value = slider.maxValue;
            }
            pValue = value;
            inputField.text = ((10f + value) / 10f).ToString();
            slider.value = value;
            gm.SetActionSpeed((10f + value) / 10f);
        }
        else//数値変換できなければ
        {
            //割り算の形であるかどうか確認する
            int divisionType = 0;
            int divisionPos = -1;
            string inputText = inputField.text;
            for (int i = 0; i < inputText.Length; i++)
            {
                //1文字ずつ見ていく
                string character = inputText.Substring(i, 1);
                //「1 /」の形があるかどうか確認し、あればその位置を記録する
                if (divisionType == 0)
                {
                    if (character == "1" || character == "1")
                    {
                        divisionType = 1;
                    }
                    else if (!(character == " ") && !(character == "　")) { break; }
                }
                else if (divisionType == 1)
                {
                    if (character == "/" || character == "／")
                    {
                        divisionPos = i + 1;
                        break;
                    }
                    else if (!(character == " ") && !(character == "　")) { break; }
                }
            }
            //割り算の形であれば
            if (divisionPos != -1)
            {
                //分母にあたる部分を取り出す
                string numText = inputText.Substring(divisionPos, inputText.Length - divisionPos);
                //取り出した部分が数字に変換できるなら
                if (float.TryParse(numText, out float divisionValue))
                {
                    divisionValue = divisionValue * 10f - 10f;
                    divisionValue = Mathf.FloorToInt(divisionValue);
                    if (divisionValue < 1)//最小値は1.1にする
                    {
                        divisionValue = 1f;
                    }
                    else if (divisionValue > slider.maxValue)
                    {
                        divisionValue = slider.maxValue;
                    }
                    pValue = -divisionValue;
                    inputField.text = "1 / " + ((divisionValue + 10f) / 10f).ToString();
                    slider.value = -divisionValue;
                    gm.SetActionSpeed(10f / (divisionValue + 10f));
                }
                else//分母が数字でなければ1つ前の値に戻す
                {
                    slider.value = pValue;
                    SliderValueToInputField();
                }
            }
            else//割り算の形でなければ1つ前の値に戻す
            {
                slider.value = pValue;
                SliderValueToInputField();
            }
        }
    }
}

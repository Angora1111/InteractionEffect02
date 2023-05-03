using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DecideDistWindow : MonoBehaviour
{
    private const float SLIDER_MAX = 750.0f;
    private const float DISAPPEAR_START = 10.00f;

    // 各インプットフィールドの、直前の値
    private float pValue_perfect = -1f;
    private float pValue_good = -1f;
    private float pValue_miss = -1f;
    private float pValue_disappear = -1f;
    // 直前の全体の値
    private float pValue_c_perfect = 0f;
    private float pValue_c_good = 0f;
    private float pValue_c_miss = 0f;
    private float pValue_c_neglect = 0f;

    [Header("ノブ")]
    [SerializeField] DistBarNob nob_perfect;
    [SerializeField] DistBarNob nob_good, nob_miss;
    [Header("消滅位置のテキスト")]
    [SerializeField] Text label_disappear;
    [Header("インプットフィールド")]
    [SerializeField] InputField inputField_perfect;
    [SerializeField] InputField inputField_good, inputField_miss, inputField_disappear;
    [Header("ガイドライン")]
    [SerializeField] DrawJudgeGuideline guideline;

    void Start()
    {
        InputFieldIntoSlider(3);

        SliderIntoInputField(0);
        SliderIntoInputField(1);
        SliderIntoInputField(2);
    }

    void Update()
    {
        // インプットフィールドが全て入力されていれば
        if (float.TryParse(inputField_perfect.text, out var value))
        {
            if (float.TryParse(inputField_good.text, out var _value))
            {
                if (float.TryParse(inputField_miss.text, out var __value))
                {
                    if (float.TryParse(label_disappear.text, out var ___value))
                    {
                        // 値が変更されていれば反映する
                        if (pValue_c_perfect != value
                           || pValue_c_good != _value
                           || pValue_c_miss != __value
                           || pValue_c_neglect != ___value)
                        {
                            // 全体の値に設定する
                            CommonData.perfectDist = value;
                            CommonData.goodDist = _value;
                            CommonData.missDist = __value;
                            CommonData.neglectDist = ___value;

                            // ガイドラインに反映する
                            guideline.Draw();

                            // 値を保存する
                            pValue_c_perfect = CommonData.perfectDist;
                            pValue_c_good = CommonData.goodDist;
                            pValue_c_miss = CommonData.missDist;
                            pValue_c_neglect = CommonData.neglectDist;
                        }
                    }
                }
            }
        }
    }

    // スライダーの値をインプットフィールドに反映する
    public void SliderIntoInputField(int trigger)
    {
        float max = 0.0f;
        float value = 0.0f;
        switch (trigger)
        {
            // PERFECT
            case 0:
                max = float.Parse(label_disappear.text);
                value = max * (nob_perfect.rectTransform.anchoredPosition.x / SLIDER_MAX);
                value = Truncate(value, 2);
                inputField_perfect.text = value.ToString();

                // 値を記録
                pValue_perfect = value;
                break;

            // GOOD
            case 1:
                max = float.Parse(label_disappear.text);
                value = max * (nob_good.rectTransform.anchoredPosition.x / SLIDER_MAX);
                value = Truncate(value, 2);
                inputField_good.text = value.ToString();

                // 値を記録
                pValue_good = value;
                break;

            // MISS
            case 2:
                max = float.Parse(label_disappear.text);
                value = max * (nob_miss.rectTransform.anchoredPosition.x / SLIDER_MAX);
                value = Truncate(value, 2);
                inputField_miss.text = value.ToString();

                // 値を記録
                pValue_miss = value;
                break;
        }
    }

    // インプットフィールドの値をスライダーに反映する
    public void InputFieldIntoSlider(int trigger)
    {
        float max = 0.0f;
        float value = 0.0f;
        switch (trigger)
        {
            // PERFECT
            case 0:
                // インプットフィールドに値があれば
                if (float.TryParse(inputField_perfect.text, out var perfectValue))
                {
                    // スライダーに反映する
                    max = float.Parse(label_disappear.text);
                    value = Truncate(perfectValue, 2);
                    nob_perfect.rectTransform.anchoredPosition = new Vector2(SLIDER_MAX * (value / max), nob_perfect.rectTransform.anchoredPosition.y);

                    // 値を記録
                    pValue_perfect = value;

                    // ノブの最小値/最大値以内に収まるように補正する
                    if (nob_perfect.FixCursorPos())
                    {
                        // 補正されたなら、改めて値を記録
                        value = max * (nob_perfect.rectTransform.anchoredPosition.x / SLIDER_MAX);
                        value = Truncate(value, 2);
                        pValue_perfect = value;
                        // インプットフィールドに反映
                        inputField_perfect.text = value.ToString();
                    }
                }
                // 値がなければ
                else
                {
                    // 直前の値を設定する
                    inputField_perfect.text = Truncate(pValue_perfect, 2).ToString();
                }
                break;

            // GOOD
            case 1:
                // インプットフィールドに値があれば
                if (float.TryParse(inputField_good.text, out var goodValue))
                {
                    // スライダーに反映する
                    max = float.Parse(label_disappear.text);
                    value = Truncate(goodValue, 2);
                    nob_good.rectTransform.anchoredPosition = new Vector2(SLIDER_MAX * (value / max), nob_good.rectTransform.anchoredPosition.y);

                    // 値を記録
                    pValue_good = value;

                    // ノブの最小値/最大値以内に収まるように補正する
                    if (nob_good.FixCursorPos())
                    {
                        // 補正されたなら、改めて値を記録
                        value = max * (nob_good.rectTransform.anchoredPosition.x / SLIDER_MAX);
                        value = Truncate(value, 2);
                        pValue_good = value;
                        // インプットフィールドに反映
                        inputField_good.text = value.ToString();
                    }
                }
                // 値がなければ
                else
                {
                    // 直前の値を設定する
                    inputField_good.text = Truncate(pValue_good, 2).ToString();
                }
                break;

            // MISS
            case 2:
                // インプットフィールドに値があれば
                if (float.TryParse(inputField_miss.text, out var missValue))
                {
                    // スライダーに反映する
                    max = float.Parse(label_disappear.text);
                    value = Truncate(missValue, 2);
                    nob_miss.rectTransform.anchoredPosition = new Vector2(SLIDER_MAX * (value / max), nob_miss.rectTransform.anchoredPosition.y);

                    // 値を記録
                    pValue_miss = value;

                    // ノブの最小値/最大値以内に収まるように補正する
                    if (nob_miss.FixCursorPos())
                    {
                        // 補正されたなら、改めて値を記録
                        value = max * (nob_miss.rectTransform.anchoredPosition.x / SLIDER_MAX);
                        value = Truncate(value, 2);
                        pValue_miss = value;
                        // インプットフィールドに反映
                        inputField_miss.text = value.ToString();
                    }
                }
                // 値がなければ
                else
                {
                    // 直前の値を設定する
                    inputField_miss.text = Truncate(pValue_miss, 2).ToString();
                }
                break;

            // Disappear
            case 3:
                // インプットフィールドに値があれば
                if(float.TryParse(inputField_disappear.text, out var disappearValue))
                {
                    // 値を記録
                    pValue_disappear = disappearValue;

                    // テキストに反映する
                    value = Truncate(disappearValue, 1);
                    label_disappear.text = value.ToString();
                }
                // 値がなければ
                else
                {
                    // 始めてなら
                    if (pValue_disappear == -1f)
                    {
                        // 値を記録
                        pValue_disappear = DISAPPEAR_START;

                        // 初期値を設定する
                        inputField_disappear.text = Truncate(DISAPPEAR_START, 2).ToString();
                        label_disappear.text = Truncate(DISAPPEAR_START, 1).ToString();
                    }
                    // 以前に入力したことがあれば
                    else
                    {
                        // 直前の値を設定する
                        inputField_disappear.text = Truncate(pValue_disappear, 2).ToString();
                        label_disappear.text = Truncate(pValue_disappear, 1).ToString();
                    }
                }

                // 全体に影響するため、各判定の値を変更する
                SliderIntoInputField(0);
                SliderIntoInputField(1);
                SliderIntoInputField(2);

                break;
        }
    }

    /// <summary>
    /// 小数第n位で切り捨て
    /// </summary>
    /// <param name="number">数字</param>
    /// <param name="decimalPlace">n</param>
    /// <returns></returns>
    private float Truncate(float number, int decimalPlace)
    {
        for(int i = 0; i < decimalPlace; i++)
        {
            number *= 10f;
        }
        number = Mathf.Floor(number);
        for (int i = 0; i < decimalPlace; i++)
        {
            number /= 10f;
        }
        return number;
    }
}

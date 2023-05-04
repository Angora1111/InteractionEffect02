using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DecideFrameWindow : MonoBehaviour
{
    private const float SLIDER_MAX = 750.0f;
    private const int DISAPPEAR_START = 20;
    private const int DISAPPEAR_MIN = 4;
    private const float FPS = 60f;
    private const float NOTESPEED = 30.0f;

    // 各インプットフィールドの、直前の値
    private int pValue_perfect = -1;
    private int pValue_good = -1;
    private int pValue_miss = -1;
    private int pValue_disappear = -1;
    // 直前の全体の値
    private int pValue_c_perfect = 0;
    private int pValue_c_good = 0;
    private int pValue_c_miss = 0;
    private int pValue_c_neglect = 0;

    [Header("ノブ")]
    [SerializeField] FrameBarNob nob_perfect;
    [SerializeField] FrameBarNob nob_good, nob_miss;
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

    private void OnEnable()
    {
        // 全体の値に設定する
        CommonData.perfectDist = pValue_c_perfect / FPS * NOTESPEED;
        CommonData.goodDist = pValue_c_good / FPS * NOTESPEED;
        CommonData.missDist = pValue_c_miss / FPS * NOTESPEED;
        CommonData.neglectDist = pValue_c_neglect / FPS * NOTESPEED;

        // ガイドラインに反映する
        guideline.Draw();
    }

    void Update()
    {
        // インプットフィールドが全て入力されていれば
        if (int.TryParse(inputField_perfect.text, out var value))
        {
            if (int.TryParse(inputField_good.text, out var _value))
            {
                if (int.TryParse(inputField_miss.text, out var __value))
                {
                    if (int.TryParse(label_disappear.text, out var ___value))
                    {
                        // 値が変更されていれば反映する
                        if (pValue_c_perfect != value
                           || pValue_c_good != _value
                           || pValue_c_miss != __value
                           || pValue_c_neglect != ___value)
                        {
                            // 全体の値に設定する
                            CommonData.perfectDist = value / FPS * NOTESPEED;
                            CommonData.goodDist = _value / FPS * NOTESPEED;
                            CommonData.missDist = __value / FPS * NOTESPEED;
                            CommonData.neglectDist = ___value / FPS * NOTESPEED;

                            // ガイドラインに反映する
                            guideline.Draw();

                            // 値を保存する
                            pValue_c_perfect = value;
                            pValue_c_good = _value;
                            pValue_c_miss = __value;
                            pValue_c_neglect = ___value;
                        }
                    }
                }
            }
        }
    }

    // スライダーの値をインプットフィールドに反映する
    public void SliderIntoInputField(int trigger)
    {
        int max = 0;
        int value = 0;
        switch (trigger)
        {
            // PERFECT
            case 0:
                max = int.Parse(label_disappear.text);
                value = Mathf.RoundToInt(max * (nob_perfect.rectTransform.anchoredPosition.x / SLIDER_MAX));
                inputField_perfect.text = value.ToString();

                // 値を記録
                pValue_perfect = value;
                break;

            // GOOD
            case 1:
                max = int.Parse(label_disappear.text);
                value = Mathf.RoundToInt(max * (nob_good.rectTransform.anchoredPosition.x / SLIDER_MAX));
                inputField_good.text = value.ToString();

                // 値を記録
                pValue_good = value;
                break;

            // MISS
            case 2:
                max = int.Parse(label_disappear.text);
                value = Mathf.RoundToInt(max * (nob_miss.rectTransform.anchoredPosition.x / SLIDER_MAX));
                inputField_miss.text = value.ToString();

                // 値を記録
                pValue_miss = value;
                break;
        }
        Debug.Log($"max:{max}, value:{value}");
    }

    // インプットフィールドの値をスライダーに反映する
    public void InputFieldIntoSlider(int trigger)
    {
        int max = 0;
        int value = 0;
        switch (trigger)
        {
            // PERFECT
            case 0:
                // インプットフィールドに値があれば
                if (int.TryParse(inputField_perfect.text, out var perfectValue))
                {
                    // スライダーに反映する
                    max = int.Parse(label_disappear.text);
                    value = perfectValue;
                    nob_perfect.rectTransform.anchoredPosition = new Vector2(SLIDER_MAX * ((float)value / max), nob_perfect.rectTransform.anchoredPosition.y);

                    // 値を記録
                    pValue_perfect = value;

                    // ノブの最小値/最大値以内に収まるように補正する
                    if (nob_perfect.FixCursorPos())
                    {
                        // 補正されたなら、改めて値を記録
                        value = Mathf.RoundToInt(max * nob_perfect.rectTransform.anchoredPosition.x / SLIDER_MAX);
                        pValue_perfect = value;
                        // インプットフィールドに反映
                        inputField_perfect.text = value.ToString();
                    }
                }
                // 値がなければ
                else
                {
                    // 直前の値を設定する
                    inputField_perfect.text = pValue_perfect.ToString();
                }
                break;

            // GOOD
            case 1:
                // インプットフィールドに値があれば
                if (int.TryParse(inputField_good.text, out var goodValue))
                {
                    // スライダーに反映する
                    max = int.Parse(label_disappear.text);
                    value = goodValue;
                    nob_good.rectTransform.anchoredPosition = new Vector2(SLIDER_MAX * ((float)value / max), nob_good.rectTransform.anchoredPosition.y);

                    // 値を記録
                    pValue_good = value;

                    // ノブの最小値/最大値以内に収まるように補正する
                    if (nob_good.FixCursorPos())
                    {
                        // 補正されたなら、改めて値を記録
                        value = Mathf.RoundToInt(max * nob_good.rectTransform.anchoredPosition.x / SLIDER_MAX);
                        pValue_good = value;
                        // インプットフィールドに反映
                        inputField_good.text = value.ToString();
                    }
                }
                // 値がなければ
                else
                {
                    // 直前の値を設定する
                    inputField_good.text = pValue_good.ToString();
                }
                break;

            // MISS
            case 2:
                // インプットフィールドに値があれば
                if (int.TryParse(inputField_miss.text, out var missValue))
                {
                    // スライダーに反映する
                    max = int.Parse(label_disappear.text);
                    value = missValue;
                    nob_miss.rectTransform.anchoredPosition = new Vector2(SLIDER_MAX * ((float)value / max), nob_miss.rectTransform.anchoredPosition.y);

                    // 値を記録
                    pValue_miss = value;

                    // ノブの最小値/最大値以内に収まるように補正する
                    if (nob_miss.FixCursorPos())
                    {
                        // 補正されたなら、改めて値を記録
                        value = Mathf.RoundToInt(max * nob_miss.rectTransform.anchoredPosition.x / SLIDER_MAX);
                        pValue_miss = value;
                        // インプットフィールドに反映
                        inputField_miss.text = value.ToString();
                    }
                }
                // 値がなければ
                else
                {
                    // 直前の値を設定する
                    inputField_miss.text = pValue_miss.ToString();
                }
                break;

            // Disappear
            case 3:
                // インプットフィールドに値があれば
                if (int.TryParse(inputField_disappear.text, out var disappearValue))
                {
                    // 値が最小値より小さければ補正する
                    if (disappearValue < DISAPPEAR_MIN)
                    {
                        disappearValue = DISAPPEAR_MIN;
                        inputField_disappear.text = DISAPPEAR_MIN.ToString();
                    }

                    // 値を記録
                    pValue_disappear = disappearValue;

                    // テキストに反映する
                    value = disappearValue;
                    label_disappear.text = value.ToString();
                }
                // 値がなければ
                else
                {
                    // 始めてなら
                    if (pValue_disappear == -1)
                    {
                        // 値を記録
                        pValue_disappear = DISAPPEAR_START;

                        // 初期値を設定する
                        inputField_disappear.text = DISAPPEAR_START.ToString();
                        label_disappear.text = DISAPPEAR_START.ToString();
                    }
                    // 以前に入力したことがあれば
                    else
                    {
                        // 直前の値を設定する
                        inputField_disappear.text = pValue_disappear.ToString();
                        label_disappear.text = pValue_disappear.ToString();
                    }
                }

                // 全体に影響するため、各判定の値を変更する
                SliderIntoInputField(0);
                SliderIntoInputField(1);
                SliderIntoInputField(2);

                break;
        }
    }
}

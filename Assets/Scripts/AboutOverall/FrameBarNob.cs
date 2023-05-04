using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FrameBarNob : Nob
{
    private const float SLIDER_WIDTH = 750.0f;

    [SerializeField] RectTransform minValueObj;
    [SerializeField] RectTransform maxValueObj;
    [SerializeField] Text label_disappear;

    public RectTransform rectTransform { get { return rt; } }

    private void Update()
    {
        // オブジェクトが設定されてる場合、オブジェクトの座標に合わせる
        if (minValueObj != null)
        {
            minValue = minValueObj.anchoredPosition.x;
        }
        if (maxValueObj != null)
        {
            maxValue = maxValueObj.anchoredPosition.x;
        }
    }

    // 指定範囲内に納める
    public override bool FixCursorPos()
    {
        // 指定範囲内に納める
        var interval = SLIDER_WIDTH / int.Parse(label_disappear.text);
        var min = minValue + interval;
        var max = maxValue - interval;
        var px = rt.anchoredPosition.x;
        rt.anchoredPosition = new Vector2(Mathf.Min(Mathf.Max(rt.anchoredPosition.x, min), max), rt.anchoredPosition.y);

        // フレームが整数値になるように位置を補正する
        var _px = rt.anchoredPosition.x;
        float fixedX = Mathf.RoundToInt(_px / interval) * interval;  // 四捨五入
        rt.anchoredPosition = new Vector2(fixedX, rt.anchoredPosition.y);

        return px != rt.anchoredPosition.x;
    }
}

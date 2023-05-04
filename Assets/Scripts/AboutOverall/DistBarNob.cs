using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistBarNob : Nob
{
    [SerializeField] RectTransform minValueObj;
    [SerializeField] RectTransform maxValueObj;

    public RectTransform rectTransform { get { return rt; } }

    private void Update()
    {
        // オブジェクトが設定されてる場合、オブジェクトの座標に合わせる
        if(minValueObj != null)
        {
            minValue = minValueObj.anchoredPosition.x;
        }
        if(maxValueObj != null)
        {
            maxValue = maxValueObj.anchoredPosition.x;
        }
    }
}

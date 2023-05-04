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
        // �I�u�W�F�N�g���ݒ肳��Ă�ꍇ�A�I�u�W�F�N�g�̍��W�ɍ��킹��
        if (minValueObj != null)
        {
            minValue = minValueObj.anchoredPosition.x;
        }
        if (maxValueObj != null)
        {
            maxValue = maxValueObj.anchoredPosition.x;
        }
    }

    // �w��͈͓��ɔ[�߂�
    public override bool FixCursorPos()
    {
        // �w��͈͓��ɔ[�߂�
        var interval = SLIDER_WIDTH / int.Parse(label_disappear.text);
        var min = minValue + interval;
        var max = maxValue - interval;
        var px = rt.anchoredPosition.x;
        rt.anchoredPosition = new Vector2(Mathf.Min(Mathf.Max(rt.anchoredPosition.x, min), max), rt.anchoredPosition.y);

        // �t���[���������l�ɂȂ�悤�Ɉʒu��␳����
        var _px = rt.anchoredPosition.x;
        float fixedX = Mathf.RoundToInt(_px / interval) * interval;  // �l�̌ܓ�
        rt.anchoredPosition = new Vector2(fixedX, rt.anchoredPosition.y);

        return px != rt.anchoredPosition.x;
    }
}

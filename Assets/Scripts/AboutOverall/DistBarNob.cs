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
        // �I�u�W�F�N�g���ݒ肳��Ă�ꍇ�A�I�u�W�F�N�g�̍��W�ɍ��킹��
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

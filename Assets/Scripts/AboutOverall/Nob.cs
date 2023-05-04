using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nob : MonoBehaviour
{
    private const float MIN_INTERVAL_BETWEEN_VALUES = 1f;

    [SerializeField] protected float minValue = 0.0f;
    [SerializeField] protected float maxValue = 750.0f;
    protected RectTransform rt;

    private void Awake()
    {
        rt = GetComponent<RectTransform>();
    }

    // �J�[�\���𓮂���
    public void MoveCursor()
    {
        // �őO�ʂɈړ�����
        transform.SetAsLastSibling();

        // �J�[�\�����}�E�X��X���W�ɍ��킹��
        var mousePosition = Input.mousePosition;
        rt.position = new Vector3(mousePosition.x, rt.position.y, rt.position.z);

        // �w��͈͓��Ɏ��܂�悤�ɕ␳����
        FixCursorPos();
    }

    // �w��͈͓��ɔ[�߂�
    public virtual bool FixCursorPos()
    {
        var min = minValue + MIN_INTERVAL_BETWEEN_VALUES;
        var max = maxValue - MIN_INTERVAL_BETWEEN_VALUES;
        var px = rt.anchoredPosition.x;
        rt.anchoredPosition = new Vector2(Mathf.Min(Mathf.Max(rt.anchoredPosition.x, min), max), rt.anchoredPosition.y);
        return px != rt.anchoredPosition.x;
    }
}

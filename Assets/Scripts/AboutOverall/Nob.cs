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

    // カーソルを動かす
    public void MoveCursor()
    {
        // 最前面に移動する
        transform.SetAsLastSibling();

        // カーソルをマウスのX座標に合わせる
        var mousePosition = Input.mousePosition;
        rt.position = new Vector3(mousePosition.x, rt.position.y, rt.position.z);

        // 指定範囲内に収まるように補正する
        FixCursorPos();
    }

    // 指定範囲内に納める
    public virtual bool FixCursorPos()
    {
        var min = minValue + MIN_INTERVAL_BETWEEN_VALUES;
        var max = maxValue - MIN_INTERVAL_BETWEEN_VALUES;
        var px = rt.anchoredPosition.x;
        rt.anchoredPosition = new Vector2(Mathf.Min(Mathf.Max(rt.anchoredPosition.x, min), max), rt.anchoredPosition.y);
        return px != rt.anchoredPosition.x;
    }
}

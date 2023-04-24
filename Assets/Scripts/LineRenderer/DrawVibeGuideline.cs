using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawVibeGuideline : MonoBehaviour
{
    private const float WIDTH = 1.0f;           // 線の太さ
    private readonly Color COLOR = new Color(0.4f, 0.4f, 1.0f); // 線の色（薄い青）
    private const float RADIUS = 7.2f;          // 円の半径
    private const int SEGMENT = 50;             // 円の角の数

    [SerializeField] LineRenderer lr;           // 中心の線
    [SerializeField] CircleLineRenderer clr_l;  // 左の円
    [SerializeField] CircleLineRenderer clr_r;  // 右の円
    [SerializeField] InputWindow iw;            // カスタムのデータ

    public void Draw()
    {
        var offsetList = iw.GetValueFromInputField();
        if (offsetList.Count != 2) return;

        Vector3 offset = new Vector3(offsetList[0], offsetList[1], 0);

        // 中心の線
        lr.enabled = true;
        Vector3 pos1 = new Vector3(-offset.x, -offset.y, 0.0f);
        Vector3 pos2 = new Vector3(offset.x, offset.y, 0.0f);

        lr.SetPosition(0, pos1);
        lr.SetPosition(1, pos2);
        lr.positionCount = 2;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = Color.blue;
        lr.endColor = Color.blue;

        // 左の円
        clr_l.DrawCircle(RADIUS, SEGMENT, WIDTH, COLOR, -offset);

        // 右の円
        clr_r.DrawCircle(RADIUS, SEGMENT, WIDTH, COLOR, offset);
    }

    public void Erase()
    {
        lr.enabled = false;
        clr_l.Erase();
        clr_r.Erase();
    }
}

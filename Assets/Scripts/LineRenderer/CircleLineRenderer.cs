using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]

public class CircleLineRenderer : MonoBehaviour
{
    private LineRenderer lr;

    /// <summary>
    /// 円を描く
    /// </summary>
    /// <param name="radius">半径</param>
    /// <param name="segment">角の数</param>
    /// <param name="width">線の太さ</param>
    /// <param name="color">色</param>
    /// <param name="offset">中心の座標</param>
    public void DrawCircle(float radius, int segment, float width, Color color, Vector3 offset)
    {
        lr = GetComponent<LineRenderer>();
        lr.enabled = true;

        // 円を構成する各頂点の座標を決定する
        var vertices = new Vector3[segment + 2];
        for (int vertNum = 0; vertNum < segment; vertNum++)
        {
            var radian = Mathf.PI * ((float)vertNum / segment) * 2.0f;
            var x = radius * Mathf.Cos(radian) + offset.x;
            var y = radius * Mathf.Sin(radian) + offset.y;
            vertices[vertNum] = new Vector3(x, y, 0.0f);
        }
        vertices[segment] = vertices[0];
        vertices[segment + 1] = vertices[1];

        // 頂点をセットする
        lr.positionCount = vertices.Length;
        lr.SetPositions(vertices);

        // 色を決定する
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = color;
        lr.endColor = color;

        // 線の太さを決定する
        lr.startWidth = width;
        lr.endWidth = width;
    }

    /// <summary>
    /// 円を消す
    /// </summary>
    public void Erase()
    {
        lr = GetComponent<LineRenderer>();
        lr.enabled = false;
    }
}

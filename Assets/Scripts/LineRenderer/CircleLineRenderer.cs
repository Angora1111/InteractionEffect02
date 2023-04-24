using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]

public class CircleLineRenderer : MonoBehaviour
{
    private LineRenderer lr;

    /// <summary>
    /// �~��`��
    /// </summary>
    /// <param name="radius">���a</param>
    /// <param name="segment">�p�̐�</param>
    /// <param name="width">���̑���</param>
    /// <param name="color">�F</param>
    /// <param name="offset">���S�̍��W</param>
    public void DrawCircle(float radius, int segment, float width, Color color, Vector3 offset)
    {
        lr = GetComponent<LineRenderer>();
        lr.enabled = true;

        // �~���\������e���_�̍��W�����肷��
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

        // ���_���Z�b�g����
        lr.positionCount = vertices.Length;
        lr.SetPositions(vertices);

        // �F�����肷��
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = color;
        lr.endColor = color;

        // ���̑��������肷��
        lr.startWidth = width;
        lr.endWidth = width;
    }

    /// <summary>
    /// �~������
    /// </summary>
    public void Erase()
    {
        lr = GetComponent<LineRenderer>();
        lr.enabled = false;
    }
}

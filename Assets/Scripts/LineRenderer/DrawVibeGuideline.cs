using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawVibeGuideline : MonoBehaviour
{
    private const float WIDTH = 1.0f;           // ���̑���
    private readonly Color COLOR = new Color(0.4f, 0.4f, 1.0f); // ���̐F�i�����j
    private const float RADIUS = 7.2f;          // �~�̔��a
    private const int SEGMENT = 50;             // �~�̊p�̐�

    [SerializeField] LineRenderer lr;           // ���S�̐�
    [SerializeField] CircleLineRenderer clr_l;  // ���̉~
    [SerializeField] CircleLineRenderer clr_r;  // �E�̉~
    [SerializeField] InputWindow iw;            // �J�X�^���̃f�[�^

    public void Draw()
    {
        var offsetList = iw.GetValueFromInputField();
        if (offsetList.Count != 2) return;

        Vector3 offset = new Vector3(offsetList[0], offsetList[1], 0);

        // ���S�̐�
        lr.enabled = true;
        Vector3 pos1 = new Vector3(-offset.x, -offset.y, 0.0f);
        Vector3 pos2 = new Vector3(offset.x, offset.y, 0.0f);

        lr.SetPosition(0, pos1);
        lr.SetPosition(1, pos2);
        lr.positionCount = 2;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = Color.blue;
        lr.endColor = Color.blue;

        // ���̉~
        clr_l.DrawCircle(RADIUS, SEGMENT, WIDTH, COLOR, -offset);

        // �E�̉~
        clr_r.DrawCircle(RADIUS, SEGMENT, WIDTH, COLOR, offset);
    }

    public void Erase()
    {
        lr.enabled = false;
        clr_l.Erase();
        clr_r.Erase();
    }
}

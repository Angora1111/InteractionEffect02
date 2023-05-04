using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawJudgeGuideline : MonoBehaviour
{
    private float WIDTH = 0.3f;     // ëæÇ≥
    private float RADIUS = 1.2f;    // îºåa
    private const int SEGMENT = 50; // â~ÇÃäpÇÃêî
    private const float LINE_HEIGHT = 5.0f; // ècê¸ÇÃí∑Ç≥
    private const float CROSS_WIDTH = 0.8f; // Å~ÇÃïù
    private readonly Color color_Perfect = new Color(1, 1, 0);          // yellow
    private readonly Color color_Good = new Color(0, 1, 0.255f);        // green
    private readonly Color color_Miss = new Color(0.317f, 0.716f, 1);   // rightBlue
    private readonly Color color_Neglect = new Color(0.7f, 0.7f, 0.7f); // gray
    private readonly Color color_Center = new Color(1f, 0.526f, 0f);    // orange

    [Header("â~ÇÃRenderer")]
    [SerializeField] CircleLineRenderer cr_neglect;
    [SerializeField] CircleLineRenderer cr_miss_late, cr_good_late, cr_perfect_late, cr_perfect_early, cr_good_early, cr_miss_early;
    [Header("â°ê¸ÇÃRenderer")]
    [SerializeField] LineRenderer lr_ml_gl;
    [SerializeField] LineRenderer lr_gl_pl, lr_pl_pe, lr_pe_ge, lr_ge_me;
    [Header("ècê¸ÇÃRenderer")]
    [SerializeField] LineRenderer lr_n;
    [SerializeField] LineRenderer lr_ml, lr_gl, lr_pl, lr_pe, lr_ge, lr_me;
    [Header("íÜêSÇÃÅ~")]
    [SerializeField] LineRenderer lr_rightdown;
    [SerializeField] LineRenderer lr_rightup;

    [ContextMenu("çƒï`âÊ")]
    public void ReDraw()
    {
        Draw();
    }

    public void Draw()
    {
        // äeîªíËÇÃãóó£ÇéÛÇØéÊÇÈ
        float perfectDist = CommonData.perfectDist;
        float goodDist = CommonData.goodDist;
        float missDist = CommonData.missDist;
        float neglectDist = CommonData.neglectDist;

        // äeâ~Çï`Ç≠
        cr_neglect.DrawCircle(RADIUS, SEGMENT, WIDTH, color_Neglect, Vector3.left * neglectDist);
        cr_miss_late.DrawCircle(RADIUS, SEGMENT, WIDTH, color_Miss, Vector3.left * missDist);
        cr_good_late.DrawCircle(RADIUS, SEGMENT, WIDTH, color_Good, Vector3.left * goodDist);
        cr_perfect_late.DrawCircle(RADIUS, SEGMENT, WIDTH, color_Perfect, Vector3.left * perfectDist);
        cr_perfect_early.DrawCircle(RADIUS, SEGMENT, WIDTH, color_Perfect, Vector3.right * perfectDist);
        cr_good_early.DrawCircle(RADIUS, SEGMENT, WIDTH, color_Good, Vector3.right * goodDist);
        cr_miss_early.DrawCircle(RADIUS, SEGMENT, WIDTH, color_Miss, Vector3.right * missDist);

        // äeÉàÉRê¸Çï`Ç≠
        DrawLine(lr_ml_gl, Vector2.left * missDist, Vector2.left * goodDist, color_Miss);
        DrawLine(lr_gl_pl, Vector2.left * goodDist, Vector2.left * perfectDist, color_Good);
        DrawLine(lr_pl_pe, Vector2.left * perfectDist, Vector2.right * perfectDist, color_Perfect);
        DrawLine(lr_pe_ge, Vector2.right * perfectDist, Vector2.right * goodDist, color_Good);
        DrawLine(lr_ge_me, Vector2.right * goodDist, Vector2.right * missDist, color_Miss);

        // äeÉ^Éeê¸Çï`Ç≠
        DrawLine(lr_n, new Vector2(-neglectDist, LINE_HEIGHT * 1.4f), new Vector2(-neglectDist, -LINE_HEIGHT * 1.4f), color_Neglect);
        DrawLine(lr_ml, new Vector2(-missDist, LINE_HEIGHT), new Vector2(-missDist, -LINE_HEIGHT), color_Miss);
        DrawLine(lr_gl, new Vector2(-goodDist, LINE_HEIGHT), new Vector2(-goodDist, -LINE_HEIGHT), color_Good);
        DrawLine(lr_pl, new Vector2(-perfectDist, LINE_HEIGHT), new Vector2(-perfectDist, -LINE_HEIGHT), color_Perfect);
        DrawLine(lr_pe, new Vector2(perfectDist, LINE_HEIGHT), new Vector2(perfectDist, -LINE_HEIGHT), color_Perfect);
        DrawLine(lr_ge, new Vector2(goodDist, LINE_HEIGHT), new Vector2(goodDist, -LINE_HEIGHT), color_Good);
        DrawLine(lr_me, new Vector2(missDist, LINE_HEIGHT), new Vector2(missDist, -LINE_HEIGHT), color_Miss);

        // íÜêSÇÃÅ~Çï`Ç≠
        DrawLine(lr_rightdown, new Vector2(-CROSS_WIDTH, CROSS_WIDTH), new Vector2(CROSS_WIDTH, -CROSS_WIDTH), color_Center);
        DrawLine(lr_rightup, new Vector2(-CROSS_WIDTH, -CROSS_WIDTH), new Vector2(CROSS_WIDTH, CROSS_WIDTH), color_Center);
    }

    public void Erase()
    {
        cr_neglect.Erase();
        cr_miss_late.Erase();
        cr_good_late.Erase();
        cr_perfect_late.Erase();
        cr_perfect_early.Erase();
        cr_good_early.Erase();
        cr_miss_early.Erase();

        lr_ml_gl.enabled = false;
        lr_gl_pl.enabled = false;
        lr_pl_pe.enabled = false;
        lr_pe_ge.enabled = false;
        lr_ge_me.enabled = false;

        lr_n.enabled = false;
        lr_ml.enabled = false;
        lr_gl.enabled = false;
        lr_pl.enabled = false;
        lr_pe.enabled = false;
        lr_ge.enabled = false;
        lr_me.enabled = false;

        lr_rightup.enabled = false;
        lr_rightdown.enabled = false;
    }

    // ê¸Çà¯Ç≠
    private void DrawLine(LineRenderer argLr, Vector2 startPos, Vector2 endPos, Color color)
    {
        // íÜêSÇÃê¸
        argLr.enabled = true;
        Vector3 pos1 = new Vector3(startPos.x, startPos.y, 0.0f);
        Vector3 pos2 = new Vector3(endPos.x, endPos.y, 0.0f);

        argLr.SetPosition(0, pos1);
        argLr.SetPosition(1, pos2);
        argLr.positionCount = 2;
        argLr.material = new Material(Shader.Find("Sprites/Default"));
        argLr.startWidth = 0.3f;
        argLr.endWidth = 0.3f;
        argLr.startColor = color;
        argLr.endColor = color;
    }
}

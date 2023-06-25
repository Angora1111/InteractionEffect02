using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongNotes : Notes
{
    private const float LTOG_LONGNOTES = 14.9f; // ローカル座標をノーツにおけるグローバル座標に変換するための値

    [SerializeField] Transform endObj;
    [SerializeField] Transform passObj;
    private bool isHolding = false; // 保持中かどうか
    private bool isEndObj = false;  // 終点オブジェクトが生成されているかどうか
    private bool isAtRelease = false;   // 判定処理が「離した」ことによるものかどうか

    protected override void CatchProcess()
    {
        EnumData.Judgement judgementResult = JudgementProcess();
        // 始点をMISSしたらノーツを削除する
        if (judgementResult == EnumData.Judgement.MISS)
        {
            Disappear();
        }
        // それ以外の判定なら取れたことにする
        else if (judgementResult != EnumData.Judgement.NONE)
        {
            isHolding = true;
            // 始点ノーツの位置を判定バーに合わせる
            float gap = JUDGE_POS - transform.localPosition.x;
            transform.localPosition = new Vector3(JUDGE_POS, transform.localPosition.y, transform.localPosition.z);
            endObj.localPosition = new Vector3(endObj.localPosition.x - gap / LTOG_LONGNOTES, endObj.localPosition.y, endObj.localPosition.z);
        }
    }

    protected override void ReleaseProcess()
    {
        // 始点を取れている状態なら
        if (isHolding)
        {
            // フラグを立てる
            isAtRelease = true;

            // 始点と終点の距離から、判定処理を行う
            float rangeOfStartAndEnd = Mathf.Abs(passObj.localPosition.x * LTOG_LONGNOTES);
            //JudgementByDistance(rangeOfStartAndEnd, CommonData.missDist);
            JudgementByConstDistance(rangeOfStartAndEnd, CommonData.perfectDist, CommonData.missDist);

            Disappear();
        }
    }

    protected override void MoveOnLane()
    {
        base.MoveOnLane();

        // ノーツの落下処理を制御する
        // 始点を取った後
        if (isHolding)
        {
            // 始点を固定する
            transform.localPosition = new Vector3(transform.localPosition.x + speed * Time.deltaTime, transform.localPosition.y, transform.localPosition.z);
            endObj.localPosition = new Vector3(endObj.localPosition.x - speed / LTOG_LONGNOTES * Time.deltaTime, endObj.localPosition.y, endObj.localPosition.z);
        }
        // 終点が生成される前
        if (!isEndObj)
        {
            // 終点を固定する
            endObj.localPosition = new Vector3(endObj.localPosition.x + speed / LTOG_LONGNOTES * Time.deltaTime, endObj.localPosition.y, endObj.localPosition.z);
        }
    }

    protected override void EndOfUpdateProcess()
    {
        // パスオブジェクトを制御する
        float rangeOfStartAndEnd = Mathf.Max(endObj.localPosition.x, 0);// 負の値にはならないようにする
        passObj.transform.localPosition = new Vector3(rangeOfStartAndEnd / 2f, passObj.transform.localPosition.y, passObj.transform.localPosition.z);
        passObj.localScale = new Vector3(rangeOfStartAndEnd, passObj.localScale.y, passObj.localScale.z);

        // 終点が指定の位置まで来たら MISS にする
        if (endObj.localPosition.x < (-CommonData.neglectDist - JUDGE_POS) / LTOG_LONGNOTES) { MissByNeglect(); }
    }

    protected override void JudgeDirection(EnumData.Judgement argJudgement)
    {
        // キャッチするときのMISSはタイプノーツと同じ演出にする
        if (!isAtRelease && argJudgement == EnumData.Judgement.MISS)
        {
            gm.TypeAction(argJudgement, true, true);
        }
        else
        {
            gm.HoldAction(argJudgement);
        }
    }

    public override void SetLayer(int num)
    {
        // 始点ノーツ
        var sr = GetComponent<SpriteRenderer>();
        sr.sortingOrder = START_SORTINGORDER - num * 11 - 9;
        int childNum = 0;
        foreach (Transform child in transform)
        {
            childNum++;
            var csr = child.gameObject.GetComponent<SpriteRenderer>();
            csr.sortingOrder = START_SORTINGORDER - num * 11 - (9 - childNum);
            if(childNum == 4) { break; }
        }
        // 終点ノーツ
        transform.GetChild(4).GetComponent<SpriteRenderer>().sortingOrder = START_SORTINGORDER - num * 11 - 4;
        childNum = 0;
        foreach (Transform child in transform.GetChild(4))
        {
            childNum++;
            var csr = child.gameObject.GetComponent<SpriteRenderer>();
            csr.sortingOrder = START_SORTINGORDER - num * 11 - (4 - childNum);
        }
        // パス
        transform.GetChild(5).GetComponent<SpriteRenderer>().sortingOrder = START_SORTINGORDER - num * 11 - 10;
    }

    /// <summary>
    /// 終点オブジェクトが生成されているかどうかのフラグを true にする
    /// </summary>
    public void SetOnIsEndObj()
    {
        isEndObj = true;
    }

    protected override void EndTimeProcess()
    {
        SetOnIsEndObj();
    }
}

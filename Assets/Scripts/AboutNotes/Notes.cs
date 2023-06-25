using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Notes : MonoBehaviour
{
    protected const int START_SORTINGORDER = 30000; // 最前面のレイヤーの値

    protected const float JUDGE_POS = 0f;           // 判定バーの位置
    protected const float LOCAL_TO_GLOBAL = 16f;    // レーン上のlocalな長さからglobalな長さに変換するための値

    protected GameManager gm;                       // GameManagerクラス
    private Transform laneObj;                      // 対応するレーンのtransform
    private Transform judgeBarCircle;               // 対応するレーンの判定枠のtransform
    private float totalTime = 0;                    // 生成からの合計時間
    protected float startTime = -1;                 // ノーツが動き出すタイミング
    protected float endTime = -1;                   // あれば、終点ノーツが動き出すタイミング
    protected int lane = -1;                        // レーン番号
    protected int kindOfDirection = -1;             // 演出の識別番号
    protected int id = -1;                          // ID番号
    protected float speed = 0;                      // 移動速度

    void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void Update()
    {
        totalTime += Time.deltaTime;

        if (startTime != -1 && startTime <= totalTime)
        {
            if(laneObj == null)
            {
                SetLaneObj();
            }

            // ノーツの判定処理 ------------------------
            // 対応するキーを押したとき
            if (Input.GetKeyDown(KeyWithLane()))
            {
                CatchProcess();
            }

            // 対応するキーを押している間
            if (Input.GetKey(KeyWithLane()))
            {
                HoldProcess();
            }

            // 対応するキーを離したとき
            if (Input.GetKeyUp(KeyWithLane()))
            {
                ReleaseProcess();
            }

            // ノーツの移動処理 ------------------------
            // ノーツが消滅する位置より上だったら
            if (transform.localPosition.x > -CommonData.neglectDist)
            {
                MoveOnLane();
            }
            //そうでなければ
            else
            {
                MissByNeglect();
            }

            EndOfUpdateProcess();
        }

        if(endTime != -1 && endTime <= totalTime)
        {
            EndTimeProcess();
        }
    }

    /// <summary>
    /// レーン上を動く（派生クラスを優先）
    /// </summary>
    protected virtual void MoveOnLane()
    {
        var plPos = transform.localPosition;
        transform.localPosition = new Vector3(plPos.x - speed * Time.deltaTime, plPos.y, plPos.z);
    }

    /// <summary>
    /// 消滅する
    /// </summary>
    protected void Disappear()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// ノーツを取り逃したことによる MISS 判定
    /// </summary>
    protected void MissByNeglect()
    {
        Debug.Log($"【lane:{lane}, id:{id}】ノーツを取り逃しました...");
        gm.ShowJudge(EnumData.Judgement.MISS);
        Disappear();
    }

    /// <summary>
    /// レーン番号に対応するKeyCodeを返す
    /// </summary>
    /// <returns></returns>
    private KeyCode KeyWithLane()
    {
        return KeyCode.Space;
    }

    /// <summary>
    /// 最も下にあるノーツが一致しているかどうか調べる
    /// </summary>
    /// <param name="argHit2Ds"></param>
    /// <param name="hit2D"></param>
    /// <returns></returns>
    private bool IsMatchClosestNote(ref RaycastHit2D[] argHit2Ds, ref RaycastHit2D hit2D)
    {
        // 最もレーンにおいて下にあるノーツを取り出す
        foreach (var _hit2D in argHit2Ds)
        {
            if (hit2D != default)
            {
                if (_hit2D.collider.transform.localPosition.x < hit2D.collider.transform.localPosition.x)
                {
                    hit2D = _hit2D;
                }
            }
            else { hit2D = _hit2D; }
        }
        // 取り出したノーツが一致するかどうかを確認する
        if (hit2D == default) { return false; } // 何も取り出されなければfalse
        if (hit2D.collider.gameObject.TryGetComponent<Notes>(out var noteData))
        {
            return noteData.GetLane() == lane && noteData.GetId() == id;
        }
        else { return false; }
    }

    /// <summary>
    /// 基本的なノーツの判定処理
    /// </summary>
    /// <returns>判定結果</returns>
    protected EnumData.Judgement JudgementProcess()
    {
        // 既に同一レーンで判定処理がなされていたら判定処理をしない
        if (StrikeNoteManager.GetIsStruck(lane)) { return EnumData.Judgement.NONE; }

        float radius = CommonData.missDist;
        RaycastHit2D[] hit2Ds = Physics2D.CapsuleCastAll(judgeBarCircle.position, new Vector2(radius * 2, 1f), CapsuleDirection2D.Horizontal, laneObj.localEulerAngles.z, Vector2.zero);
        RaycastHit2D hit2D = default;
        // 最もレーンにおいて下にあるノーツが自分自身であるかどうかを確認する
        if (IsMatchClosestNote(ref hit2Ds, ref hit2D))
        {
            // 該当レーンのフラグを true にする
            StrikeNoteManager.SetIsStruck(lane);

            // ノーツと判定枠の距離を格納
            float distance = Mathf.Abs(hit2D.transform.localPosition.x - judgeBarCircle.localPosition.x);

            // 距離から判定処理を行う
            return JudgementByConstDistance(distance, CommonData.perfectDist, CommonData.goodDist);
        }
        else 
        {
            return EnumData.Judgement.NONE; 
        }
    }

    /// <summary>
    /// 距離と基準を渡して、割合で判定を受け取る
    /// </summary>
    /// <param name="distance">判定対象の距離</param>
    /// <param name="creteria">判定基準の距離</param>
    /// <returns>判定結果</returns>
    protected EnumData.Judgement JudgementByDistance(float distance, float creteria)
    {
        if (distance < creteria * 2 / 8)
        {
            // PERFECT
            gm.ShowJudge(EnumData.Judgement.PERFECT);
            JudgeDirection(EnumData.Judgement.PERFECT);
            return EnumData.Judgement.PERFECT;
        }
        else if (distance < creteria * 6 / 8)
        {
            // GOOD
            gm.ShowJudge(EnumData.Judgement.GOOD);
            JudgeDirection(EnumData.Judgement.GOOD);
            return EnumData.Judgement.GOOD;
        }
        else
        {
            // MISS
            gm.ShowJudge(EnumData.Judgement.MISS);
            JudgeDirection(EnumData.Judgement.MISS);
            return EnumData.Judgement.MISS;
        }
    }

    /// <summary>
    /// 距離と各判定の基準を渡して、判定を受け取る
    /// </summary>
    /// <param name="distance">判定対象の距離</param>
    /// <param name="creteria_perfect">PERFECT基準の距離</param>
    /// <param name="creteria_good">GOOD基準の距離</param>
    /// <returns></returns>
    protected EnumData.Judgement JudgementByConstDistance(float distance, float creteria_perfect, float creteria_good)
    {
        if (distance < creteria_perfect)
        {
            // PERFECT
            gm.ShowJudge(EnumData.Judgement.PERFECT);
            JudgeDirection(EnumData.Judgement.PERFECT);
            return EnumData.Judgement.PERFECT;
        }
        else if (distance < creteria_good)
        {
            // GOOD
            gm.ShowJudge(EnumData.Judgement.GOOD);
            JudgeDirection(EnumData.Judgement.GOOD);
            return EnumData.Judgement.GOOD;
        }
        else
        {
            // MISS
            gm.ShowJudge(EnumData.Judgement.MISS);
            JudgeDirection(EnumData.Judgement.MISS);
            return EnumData.Judgement.MISS;
        }
    }

    /// <summary>
    /// ノーツを取ったときの処理（派生クラスを優先）
    /// </summary>
    protected virtual void CatchProcess() { }

    /// <summary>
    /// ノーツをとり続けているときの処理（派生クラスを優先）
    /// </summary>
    protected virtual void HoldProcess() { }

    /// <summary>
    /// ノーツを離したときの処理（派生クラスを優先）
    /// </summary>
    protected virtual void ReleaseProcess() { }

    /// <summary>
    /// ノーツを取った際の演出を実行する処理（派生クラスを優先）
    /// </summary>
    protected virtual void JudgeDirection(EnumData.Judgement argJudgement) { }

    /// <summary>
    /// Update()関数内の各処理が終了した後に実行されるその他の処理（派生クラスを優先）
    /// </summary>
    protected virtual void EndOfUpdateProcess() { }

    /// <summary>
    /// endTimeが設定されていて、かつその時間になったときに実行される処理（派生クラスを優先）
    /// </summary>
    protected virtual void EndTimeProcess() { }

    /// <summary>
    /// レーン番号とIDを設定する
    /// </summary>
    /// <param name="argLane">設定後のレーン番号</param>
    /// <param name="argId">設定後のID</param>
    public void SetValues(float argStartTime, float argEndTime, int argLane, int argId, float argSpeed, int argKindOfDirection = -1)
    {
        startTime = argStartTime;
        endTime = argEndTime;
        lane = argLane;
        id = argId;
        kindOfDirection = argKindOfDirection;
        SetSpeed(argSpeed);
    }

    /// <summary>
    /// レーンと判定バーのtarnsformをセットする
    /// </summary>
    private void SetLaneObj()
    {
        var laneGroup = GameObject.Find("LaneGroup").transform;
        if (laneGroup.childCount > lane)
        {
            laneObj = laneGroup.GetChild(lane);
            judgeBarCircle = laneObj.GetChild(2).transform;

            transform.SetParent(laneGroup.transform.GetChild(lane).transform);
            transform.localPosition = new Vector3(NotesGenerator.StartPos, 0, 0);
        }
        else { Debug.Log($"【lane:{lane}】対応するレーンが存在しません"); }
    }

    /// <summary>
    /// speedを指定の値に設定する
    /// </summary>
    /// <param name="argSpeed">設定後の速度</param>
    public void SetSpeed(float argSpeed)
    {
        speed = argSpeed;
    }

    /// <summary>
    /// レイヤーをノーツ内で最背面にする（派生クラスを優先）
    /// </summary>
    /// <param name="num"></param>
    public virtual void SetLayer(int num)
    {
        var sr = GetComponent<SpriteRenderer>();
        sr.sortingOrder = START_SORTINGORDER - num * 11 - 3;
        int childNum = 0;
        foreach (Transform child in transform)
        {
            childNum++;
            var csr = child.gameObject.GetComponent<SpriteRenderer>();
            csr.sortingOrder = START_SORTINGORDER - num * 11 - (3 - childNum);
        }
    }

    /// <summary>
    /// laneのゲッター
    /// </summary>
    /// <returns></returns>
    public int GetLane() { return lane; }

    /// <summary>
    /// idのゲッター
    /// </summary>
    /// <returns></returns>
    public int GetId() { return id; }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using AngoraUtility;

public class GameManager : MonoBehaviour
{
    // --- 定数 ---
    private readonly Color color_Perfect = new Color(1, 1, 0);  // yellow
    private readonly Color color_Good = new Color(0, 1, 0.255f);// green
    private readonly Color color_Miss = new Color(0, 0.583f, 1);// rightBlue
    private const int FIXANIMMODE_INTERUPT = 0;
    private const int FIXANIMMODE_COMPLETE = 1;
    private const int FIXANIMMODE_FIXSPEED = 2;

    public enum ChangeMode
    {
        NONE,
        TYPE,
        HOLD,
    }

    public enum EffectModeType
    {
        NONE,
        LANE_ROTATE_1,
        LANE_SLIME_1,
        LANE_VIBE_1,
        BACK_EXPAND_WITH_ROTATION_1,
        BACK_FLASH_1,
        MAX,
    }

    public enum EffectModeHold
    {
        NONE,
        LANE_ROTATE_1,
        LANE_ARROW_1,
        BACK_PARGE_1,
        MAX,
    }

    enum MovementType
    {
        STRAIGHT,
        QUICK,
        SLOW,
        WARP,
    }

    // --- 変数 ---
    [SerializeField] PlayableDirector playableDirector; // タイムライン
    [SerializeField] NotesGenerator notesGenerator;     // ノーツ生成オブジェクト
    [SerializeField] ChangeTimeLine changeTimeLine;     // 楽曲をセットするためのクラス
    [SerializeField] GameObject canvas;     // UI
    [SerializeField] Transform laneGroup;   // レーン全て
    [SerializeField] Transform judgeCircle; // 判定枠だけ
    [SerializeField] Transform[] judgeObjs; // それぞれのレーン全て
    private SpriteRenderer judgeSr;         // 判定枠のSpriteRenderer
    public static EffectModeType[] effectMode_Type = new EffectModeType[(int)EnumData.Judgement.MAX]; // 演出の種類
    public static EffectModeHold[] effectMode_Hold = new EffectModeHold[(int)EnumData.Judgement.MAX];
    private EffectModeType currentEffectMode_Type = EffectModeType.NONE;                              // 現在実行中の演出
    private EffectModeHold currentEffectMode_Hold = EffectModeHold.NONE;
    private static int _selectingEffectIndex = (int)EnumData.Judgement.PERFECT;                       // 演出を変更中のインデックス番号
    public static int selectingEffectIndex { get { return _selectingEffectIndex; } }
    private int orderNum_Type = 0;  // 順番管理 
    private int orderNum_Hold = 0;
    [SerializeField] GameObject[] Squares;// 背景の格納用
    [SerializeField] FlashSquare flashSquare;
    [SerializeField] AppearSquare square_Appear;    // 内→外の四角
    [SerializeField] PargeSquare square_Parge;      // パージする四角
    private GameObject pargingSquare;
    [SerializeField] GameObject squareGroup;        // 四角が入れられるところ

    private List<Color>[] colorPreviews_appear;       // 色を格納するところ
    private List<Color>[] colorPreviews_parge;        // 色を格納するところ
    private List<bool>[] boolPreviews;                // bool値を格納するところ
    private List<int>[] modePreviews;                 // dropdownのValueの値を格納するところ
    private List<float>[] inputFieldPreviews_type;    // typeにおけるinputFieldの値を格納するところ
    private List<float>[] inputFieldPreviews_long;    // longにおけるinputFieldの値を格納するところ
    private EnumData.Judgement catchLongJudge;        // ロングノーツ始点における判定

    private bool isSetting = false; // 設定中かどうか
    private bool canStart = true;   // プレイ中か否か
    private float fixedActionSpeed = 1.0f;  // 変化の速度
    private Coroutine savedCoroutine = default;     // コルーチンの保存用
    private Coroutine[] savedTypeActions = default;
    private Coroutine[] savedHoldActions = default;
    private Vector3 completedLaneGroupPos;      // 変化が完了した後の判定グループの位置
    private Vector3 completedJudgeCirclePos;    // 変化が完了した後の判定枠の位置
    private Vector3 completedJudgeCircleScale;  // 変化が完了した後の判定枠の大きさ
    private Vector3 completedLaneGroupRot;      // 変化が完了した後の判定グループの角度
    [SerializeField] GameObject[] hideObjAtSetMiss; // Missの演出を設定するときに非表示にするオブジェクト

    public static ChangeMode showingSettingPage = ChangeMode.TYPE;    // 現在設定画面に表示中のページ番号
    [SerializeField] Text settingPageHeaderText;                      // 設定画面のヘッダーText
    [SerializeField] GameObject settingPageNext;                      // 設定画面の進む矢印
    [SerializeField] GameObject settingPageBack;                      // 設定画面の戻る矢印

    [HideInInspector] public int fixAnimationMode = FIXANIMMODE_COMPLETE;   // 演出補正の方法
    private float nextRotAnimDeg = 0;                                       // 回転演出の次の角度

    [Header("テスト用 --------------------------------------------------------------------")]
    [SerializeField] EffectModeType testMode_Type = EffectModeType.NONE;
    [SerializeField] EffectModeHold testMode_Hold = EffectModeHold.NONE;
    [SerializeField] bool test = false; // テスト用か否か

    void Start()
    {
        judgeSr = GameObject.Find("JudgeCircle").GetComponent<SpriteRenderer>();
        colorPreviews_appear = new List<Color>[(int)(EnumData.Judgement.MAX - 1) * (int)(EffectModeType.MAX - 1)];
        colorPreviews_parge = new List<Color>[(int)(EnumData.Judgement.MAX - 1) * (int)(EffectModeHold.MAX - 1)];
        boolPreviews = new List<bool>[(int)(EnumData.Judgement.MAX - 1) * (int)(EffectModeType.MAX - 1)];
        modePreviews = new List<int>[(int)(EnumData.Judgement.MAX - 1) * (int)(EffectModeType.MAX - 1)];
        inputFieldPreviews_type = new List<float>[(int)(EnumData.Judgement.MAX - 1) * (int)(EffectModeType.MAX - 1)];
        inputFieldPreviews_long = new List<float>[(int)(EnumData.Judgement.MAX - 1) * (int)(EffectModeHold.MAX - 1)];
        for(int i = 0; i < (int)(EnumData.Judgement.MAX - 1) * (int)(EffectModeType.MAX - 1); i++)
        {
            colorPreviews_appear[i] = new List<Color>();
            boolPreviews[i] = new List<bool>();
            modePreviews[i] = new List<int>();
            inputFieldPreviews_type[i] = new List<float>();
        }
        for(int i = 0; i < (int)(EnumData.Judgement.MAX - 1) * (int)(EffectModeHold.MAX - 1); i++)
        {
            colorPreviews_parge[i] = new List<Color>();
            inputFieldPreviews_long[i] = new List<float>();
        }
        savedTypeActions = new Coroutine[(int)EffectModeType.MAX];
        savedHoldActions = new Coroutine[(int)EffectModeHold.MAX];
        completedLaneGroupPos = laneGroup.position;
        completedJudgeCirclePos = judgeCircle.position;
        completedJudgeCircleScale = judgeCircle.localScale;
        completedLaneGroupRot = laneGroup.eulerAngles;
        changeTimeLine.SetSong();

        int effectLength = (int)EnumData.Judgement.MAX;
        for (int i = 0; i < effectLength; i++)
        {
            effectMode_Type[i] = EffectModeType.NONE;
            effectMode_Hold[i] = EffectModeHold.NONE;
        }
    }

    void Update()
    {
        // 設定用ページの更新
        if(showingSettingPage == ChangeMode.TYPE)
        {
            settingPageHeaderText.text = "1回押し";
            settingPageNext.SetActive(true);
            settingPageBack.SetActive(false);
        }
        if(showingSettingPage == ChangeMode.HOLD)
        {
            settingPageHeaderText.text = "長押し→離す";
            settingPageNext.SetActive(false);
            settingPageBack.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.Alpha0) && canStart && !isSetting)
        {
            if (test)//テスト用
            {
                int effectLength = (int)EnumData.Judgement.MAX;
                for(int i = 0; i < effectLength; i++)
                {
                    effectMode_Type[i] = testMode_Type;
                    effectMode_Hold[i] = testMode_Hold;
                }
            }

            // 値の初期化
            nextRotAnimDeg = 0;

            canStart = false;
            playableDirector.Stop();
            playableDirector.Play();
            notesGenerator.StartGame();
        }

        if (canStart)
        {
            canvas.SetActive(true);
        }
        else
        {
            canvas.SetActive(false);
        }

        // 判定枠の色を戻す
        FadeOutJudgeColor();
    }

    #region 変化の実行部分
    /// <summary>
    /// 変化中に再び変化を行うためのリセット処理
    /// </summary>
    /// <param name="argTypeMode">変化の種類（タイプ）</param>
    /// <param name="isPosReset">位置をリセットするかどうか</param>
    /// <param name="isScaleReset">大きさをリセットするかどうか</param>
    /// <param name="isRotReset">角度をリセットするかどうか</param>
    private void RestartAction(EffectModeType argTypeMode, bool isPosReset, bool isScaleReset, bool isRotReset)
    {
        if (savedTypeActions[(int)argTypeMode] != null)
        {
            StopCoroutine(savedTypeActions[(int)argTypeMode]);
            if (isPosReset) { laneGroup.position = completedLaneGroupPos; judgeCircle.position = completedJudgeCirclePos; }
            if (isScaleReset) { judgeCircle.localScale = completedJudgeCircleScale; }
            if (isRotReset) { laneGroup.eulerAngles = completedLaneGroupRot; }
        }
    }

    /// <summary>
    /// 変化中に再び変化を行うためのリセット処理
    /// </summary>
    /// <param name="argHoldMode">変化の種類（ホールド）</param>
    /// <param name="isPosReset">位置をリセットするかどうか</param>
    /// <param name="isScaleReset">大きさをリセットするかどうか</param>
    /// <param name="isRotReset">角度をリセットするかどうか</param>
    private void RestartAction(EffectModeHold argHoldMode, bool isPosReset, bool isScaleReset, bool isRotReset)
    {
        if (savedHoldActions[(int)argHoldMode] != null)
        {
            StopCoroutine(savedHoldActions[(int)argHoldMode]);
            if (isPosReset) { laneGroup.position = completedLaneGroupPos; judgeCircle.position = completedJudgeCirclePos; }
            if (isScaleReset) { judgeCircle.localScale = completedJudgeCircleScale; }
            if (isRotReset) { laneGroup.eulerAngles = completedLaneGroupRot; }
        }
    }

    public void TypeAction(EnumData.Judgement argJudgement, bool isAction = true, bool notAddOrderNum = false)
    {
        // 最新の演出を保存
        currentEffectMode_Type = effectMode_Type[(int)argJudgement];
        // 判定の番号 + 1を保存
        int judgeIndex = (int)argJudgement;
        // 演出の番号を保存
        int directionIndex = (int)currentEffectMode_Type - 1;

        Debug.Log($"judge:{judgeIndex}, dir:{directionIndex}");

        // ノーツ数の更新
        NotesGenerator.CountUpNoteNum();

        switch (currentEffectMode_Type)
        {
            case EffectModeType.NONE:
                break;
            case EffectModeType.LANE_ROTATE_1:
                if (isAction)
                {
                    int deg = Mathf.RoundToInt(inputFieldPreviews_type[judgeIndex * directionIndex][0]);
                    float animTime = inputFieldPreviews_type[judgeIndex * directionIndex][1];

                    if (fixAnimationMode == FIXANIMMODE_FIXSPEED)
                    {
                        RestartAction(currentEffectMode_Type, false, false, false);

                        nextRotAnimDeg += deg;
                        int rotDeg = Mathf.RoundToInt(nextRotAnimDeg - ExTransform.GetFixEulerAngles(laneGroup, ExTransform.AXIS.X, 0).z);
                        if (rotDeg < 0) rotDeg += 360;
                        if(nextRotAnimDeg >= 360f) nextRotAnimDeg = 0;

                        float waitTime = NotesGenerator.DeltaTimeToNext;
                        Debug.Log($"waitTime:{waitTime}");
                        if (waitTime == -1f) waitTime = 0.3f;
                        savedTypeActions[(int)currentEffectMode_Type] = StartCoroutine(RotateJudgeCircleGap(rotDeg, waitTime));
                    }
                    else if(fixAnimationMode == FIXANIMMODE_COMPLETE)
                    {
                        RestartAction(currentEffectMode_Type, false, false, true);

                        float waitTime = animTime;
                        savedTypeActions[(int)currentEffectMode_Type] = StartCoroutine(RotateJudgeCircleGap(deg, waitTime));
                    }
                    else if(fixAnimationMode == FIXANIMMODE_INTERUPT)
                    {
                        RestartAction(currentEffectMode_Type, false, false, false);

                        float waitTime = animTime;
                        savedTypeActions[(int)currentEffectMode_Type] = StartCoroutine(RotateJudgeCircleGap(deg, waitTime));
                    }

                    //「orderNum」で一部変化させることが可能
                }
                break;
            case EffectModeType.LANE_SLIME_1:
                if (isAction)
                {
                    RestartAction(currentEffectMode_Type, false, true, false);

                    savedTypeActions[(int)currentEffectMode_Type] = StartCoroutine(SlimeCircle(0.09f, 0.09f, 20, 16));

                    //「orderNum」で一部変化させることが可能
                }
                break;
            case EffectModeType.LANE_VIBE_1:
                if (isAction)
                {
                    float customX = inputFieldPreviews_type[judgeIndex * directionIndex][0];
                    float customY = inputFieldPreviews_type[judgeIndex * directionIndex][1];

                    RestartAction(currentEffectMode_Type, true, false, false);

                    Vector3 moveVec = Vector3.zero;
                    switch (modePreviews[judgeIndex * directionIndex][0])
                    {
                        case 0: moveVec = new Vector3(1.5f, 0, 0); break;           //平行
                        case 1: moveVec = new Vector3(0, 1.5f, 0); break;           //垂直
                        case 2: moveVec = new Vector3(customX, customY, 0); break;  //カスタム
                    }

                    savedTypeActions[(int)currentEffectMode_Type] = StartCoroutine(VibeJudgeCircle(moveVec, boolPreviews[judgeIndex * directionIndex][0], boolPreviews[judgeIndex * directionIndex][1], boolPreviews[judgeIndex * directionIndex][2]));
                }
                break;
            case EffectModeType.BACK_EXPAND_WITH_ROTATION_1:
                if (isAction)
                {
                    switch (orderNum_Type % 2)
                    {
                        case 0:
                            SquareAppearInit(colorPreviews_appear[judgeIndex * directionIndex][0]);
                            break;
                        case 1:
                            SquareAppearInit(colorPreviews_appear[judgeIndex * directionIndex][1]);
                            break;
                    }
                }
                break;
            case EffectModeType.BACK_FLASH_1:
                if (isAction)
                {
                    flashSquare.gameObject.SetActive(false);
                    flashSquare.gameObject.SetActive(true);
                    Debug.Log(colorPreviews_appear[judgeIndex * directionIndex].Count);
                    flashSquare.ChangeColor(colorPreviews_appear[judgeIndex * directionIndex][0]);
                }
                break;
        }

        if(!notAddOrderNum) orderNum_Type++;
    }

    public void HoldAction(EnumData.Judgement argJudgement, bool isAction = true)
    {
        // 最新の演出と始点の判定を保存（最初だけ）
        if (orderNum_Hold == 0)
        {
            currentEffectMode_Hold = effectMode_Hold[(int)argJudgement];
            catchLongJudge = argJudgement;
        }
        // 判定の番号を保存
        int judgeIndex = (int)argJudgement;
        // 演出の番号を保存
        int directionIndex = (int)currentEffectMode_Hold - 1;

        switch (currentEffectMode_Hold)
        {
            case EffectModeHold.NONE:
                break;
            case EffectModeHold.LANE_ROTATE_1:
                if (isAction && (orderNum_Hold == 0 || (orderNum_Hold > 0 && argJudgement != EnumData.Judgement.MISS)))
                {
                    RestartAction(currentEffectMode_Hold, false, false, false);//直前の変化をリセットしない
                    RestartAction(EffectModeType.LANE_ROTATE_1, false, false, false);//直前の変化をリセットしない
                    RestartAction(EffectModeType.LANE_VIBE_1, true, false, false);
                    RestartAction(EffectModeType.LANE_SLIME_1, false, true, false);

                    switch (orderNum_Hold)
                    {
                        case 0:
                            savedHoldActions[(int)currentEffectMode_Hold] = StartCoroutine(RotateJudgeCircleGap(-120, 0.8f));
                            break;
                        case 1:
                            savedHoldActions[(int)currentEffectMode_Hold] = StartCoroutine(RotateJudgeCircleGap(360 - (int)laneGroup.eulerAngles.z + 360, 0.3f));
                            break;
                    }
                }
                break;
            case EffectModeHold.LANE_ARROW_1:
                float drawDistance = inputFieldPreviews_long[(int)catchLongJudge * directionIndex][0];
                float fireMulInSuccess = inputFieldPreviews_long[(int)catchLongJudge * directionIndex][1];
                float fireMulInFailure = inputFieldPreviews_long[(int)catchLongJudge * directionIndex][2];

                if (isAction && (orderNum_Hold == 0 || (orderNum_Hold > 0 && argJudgement != EnumData.Judgement.MISS)))
                {
                    RestartAction(EffectModeType.LANE_ROTATE_1, false, false, false);

                    switch (orderNum_Hold)
                    {
                        case 0:
                            RestartAction(EffectModeType.LANE_VIBE_1, true, false, false);
                            RestartAction(EffectModeType.LANE_SLIME_1, false, true, false);

                            float laneRot = laneGroup.transform.eulerAngles.z;
                            savedCoroutine = StartCoroutine(MoveJudgeCircle(new Vector3(-drawDistance, 0, 0), 1.0f, MovementType.STRAIGHT, true));
                            break;
                        case 1:
                            StopCoroutine(savedCoroutine);
                            savedHoldActions[(int)currentEffectMode_Hold] = StartCoroutine(TurnMoveJudgeCircle(new Vector3(laneGroup.position.magnitude * fireMulInSuccess, 0, 0), new Vector3(laneGroup.position.magnitude * -(fireMulInSuccess - 1f), 0, 0), 0.2f, MovementType.QUICK, MovementType.SLOW, true));
                            break;
                    }
                }
                else if (orderNum_Hold == 1)//途中で離した場合
                {
                    StopCoroutine(savedCoroutine);
                    savedHoldActions[(int)currentEffectMode_Hold] = StartCoroutine(TurnMoveJudgeCircle(new Vector3(laneGroup.position.magnitude * fireMulInFailure, 0, 0), new Vector3(laneGroup.position.magnitude * -(fireMulInFailure - 1f), 0, 0), 0.2f, MovementType.QUICK, MovementType.SLOW, true));
                }
                break;
            case EffectModeHold.BACK_PARGE_1:
                if (isAction && (orderNum_Hold == 0 || (orderNum_Hold > 0 && argJudgement != EnumData.Judgement.MISS)))
                {
                    switch (orderNum_Hold)
                    {
                        case 0:
                            SquarePargeInit(colorPreviews_parge[judgeIndex * directionIndex][0]);
                            break;
                        case 1:
                            pargingSquare.GetComponent<PargeSquare>().SetKeepingFalse();
                            break;
                    }
                }
                break;
        }

        orderNum_Hold++;
    }

    /// <summary>
    /// 判定に合わせて判定枠の色を変更する
    /// </summary>
    /// <param name="judge">判定</param>
    public void ShowJudge(EnumData.Judgement judge)
    {
        switch (judge)
        {
            case EnumData.Judgement.PERFECT:
                Debug.Log("PERFECT!!");
                judgeSr.color = color_Perfect;
                break;
            case EnumData.Judgement.GOOD:
                Debug.Log("GOOD!");
                judgeSr.color = color_Good;
                break;
            case EnumData.Judgement.MISS:
                Debug.Log("MISS...");
                judgeSr.color = color_Miss;
                break;
        }
    }

    /// <summary>
    /// 判定枠の色を少しづつ元に戻す
    /// </summary>
    private void FadeOutJudgeColor()
    {
        if (judgeSr.color != Color.white)
        {
            Color color = new Color(1f - judgeSr.color.r, 1f - judgeSr.color.g, 1f - judgeSr.color.b);
            Color _color = new Color(color.r * 0.03f, color.g * 0.03f, color.b * 0.03f);
            if (judgeSr.color.r > 0.9f && judgeSr.color.g > 0.9f && judgeSr.color.b > 0.9f)
            {
                judgeSr.color = Color.white;
            }
            else
            {
                judgeSr.color += _color;
            }
        }
    }
    #endregion

    #region 実行前の準備
    /// <summary>
    /// 予定の色を、以前のものに合わせる
    /// </summary>
    /// <param name="button"></param>
    public void SetPreviousColors(Transform button)
    {
        var list = new List<Color>();
        var _buttonData = button.gameObject.GetComponent<ModeButton>();
        switch (_buttonData.Getmode())
        {
            case ChangeMode.TYPE:
                list = colorPreviews_appear[selectingEffectIndex * (int)(_buttonData.GetEffectTypeMode() - 1)];
                break;
            case ChangeMode.HOLD:
                list = colorPreviews_parge[selectingEffectIndex * (int)(_buttonData.GetEffectHoldMode() - 1)];
                break;
            default:
                break;
        }

        // 未設定なら抜ける
        if (list == null) return;
        if (list.Count == 0) return;

        int _listIndex = 0;
        foreach (Transform child in button)
        {
            if (child.gameObject.TryGetComponent<ColorImageData>(out var data) && child.gameObject.TryGetComponent<Image>(out Image image))
            {
                if (_listIndex >= list.Count) return;
                image.color = list[_listIndex];
                _listIndex++;
            }
        }
    }
    /// <summary>
    /// 予定の色をセットする
    /// </summary>
    /// <param name="button"></param>
    public void SetColors(Transform button)
    {
        var list = new List<Color>();
        var _newList = new List<Color>();
        // 該当要素のデータを受け取る
        foreach (Transform child in button)
        {
            if (child.gameObject.TryGetComponent<ColorImageData>(out var data) && child.gameObject.TryGetComponent<Image>(out Image image))
            {
                _newList.Add(image.color);
            }
        }

        // 受け取ったデータがあれば反映
        if(_newList.Count != 0)
        {
            var _buttonData = button.gameObject.GetComponent<ModeButton>();
            switch (_buttonData.Getmode())
            {
                case ChangeMode.TYPE:
                    list = colorPreviews_appear[selectingEffectIndex * (int)(_buttonData.GetEffectTypeMode() - 1)];
                    break;
                case ChangeMode.HOLD:
                    list = colorPreviews_parge[selectingEffectIndex * (int)(_buttonData.GetEffectHoldMode() - 1)];
                    break;
                default:
                    break;
            }
            if (list != null)
            {
                list.Clear();
            }
            for (int i = 0; i < _newList.Count; i++)
            {
                list.Add(_newList[i]);
            }
        }
    }

    /// <summary>
    /// 予定のbool値を、以前のものに合わせる
    /// </summary>
    /// <param name="button"></param>
    public void SetPreviousBools(Transform button)
    {
        int _effectIndex = 0;
        var _buttonData = button.gameObject.GetComponent<ModeButton>();
        switch (_buttonData.Getmode())
        {
            case ChangeMode.TYPE:
                _effectIndex = (int)(_buttonData.GetEffectTypeMode() - 1);
                break;
            case ChangeMode.HOLD:
                _effectIndex = (int)(_buttonData.GetEffectHoldMode() - 1);
                break;
            default:
                break;
        }

        // 未設定なら初期値にする
        if (boolPreviews[selectingEffectIndex * _effectIndex] == null) return;
        if (boolPreviews[selectingEffectIndex * _effectIndex].Count == 0) return;

        int _index = 0;
        foreach (Transform child in button)
        {
            if (child.gameObject.TryGetComponent<BoolWindow>(out BoolWindow bw))
            {
                if (_index >= boolPreviews[selectingEffectIndex * _effectIndex].Count) return;
                bw.SetBool(boolPreviews[selectingEffectIndex * _effectIndex][_index]);
                _index++;
            }
        }
    }
    /// <summary>
    /// 予定のbool値をセットする
    /// </summary>
    /// <param name="button"></param>
    public void SetBools(Transform button)
    {
        var list = new List<bool>();
        var _newList = new List<bool>();
        // 該当要素のデータを受け取る
        foreach (Transform child in button)
        {
            if (child.gameObject.TryGetComponent<BoolWindow>(out BoolWindow bw))
            {
                _newList.Add(bw.GetBool());
            }
        }

        // 受け取ったデータがあれば反映
        if (_newList.Count != 0)
        {
            var _buttonData = button.gameObject.GetComponent<ModeButton>();
            switch (_buttonData.Getmode())
            {
                case ChangeMode.TYPE:
                    list = boolPreviews[selectingEffectIndex * (int)(_buttonData.GetEffectTypeMode() - 1)];
                    break;
                case ChangeMode.HOLD:
                    list = boolPreviews[selectingEffectIndex * (int)(_buttonData.GetEffectHoldMode() - 1)];
                    break;
                default:
                    break;
            }
            if (list != null)
            {
                list.Clear();
            }
            for (int i = 0; i < _newList.Count; i++)
            {
                list.Add(_newList[i]);
            }
        }
    }

    /// <summary>
    /// 予定のint値を、以前のものに合わせる
    /// </summary>
    /// <param name="button"></param>
    public void SetPreviousIntFromDropdown(Transform button)
    {
        int _effectIndex = 0;
        var _buttonData = button.gameObject.GetComponent<ModeButton>();
        switch (_buttonData.Getmode())
        {
            case ChangeMode.TYPE:
                _effectIndex = (int)(_buttonData.GetEffectTypeMode() - 1);
                break;
            case ChangeMode.HOLD:
                _effectIndex = (int)(_buttonData.GetEffectHoldMode() - 1);
                break;
            default:
                break;
        }

        // 未設定なら抜ける
        if (modePreviews[selectingEffectIndex * _effectIndex] == null) return;
        if (modePreviews[selectingEffectIndex * _effectIndex].Count == 0) return;

        int _index = 0;
        foreach (Transform child in button)
        {
            if (child.gameObject.TryGetComponent<DropdownWindow>(out DropdownWindow ddw))
            {
                if (_index >= modePreviews[selectingEffectIndex * _effectIndex].Count) return;
                ddw.SetValue(modePreviews[selectingEffectIndex * _effectIndex][_index]);
                _index++;
            }
        }
    }
    /// <summary>
    /// 予定のint値を、Dropdownから受け取ってセットする
    /// </summary>
    /// <param name="button"></param>
    public void SetIntFromDropdown(Transform button)
    {
        var list = new List<int>();
        var _newList = new List<int>();
        // 該当要素のデータを受け取る
        foreach (Transform child in button)
        {
            if (child.gameObject.TryGetComponent<DropdownWindow>(out DropdownWindow ddw))
            {
                _newList.Add(ddw.GetValueFromDropdown());
            }
        }

        // 受け取ったデータがあれば反映
        if (_newList.Count != 0)
        {
            var _buttonData = button.gameObject.GetComponent<ModeButton>();
            switch (_buttonData.Getmode())
            {
                case ChangeMode.TYPE:
                    list = modePreviews[selectingEffectIndex * (int)(_buttonData.GetEffectTypeMode() - 1)];
                    break;
                case ChangeMode.HOLD:
                    list = modePreviews[selectingEffectIndex * (int)(_buttonData.GetEffectHoldMode() - 1)];
                    break;
                default:
                    break;
            }
            if (list != null)
            {
                list.Clear();
            }
            for (int i = 0; i < _newList.Count; i++)
            {
                list.Add(_newList[i]);
            }
        }

    }

    /// <summary>
    /// 予定のfloat値を、以前のものに合わせる
    /// </summary>
    /// <param name="button"></param>
    public void SetPreviousFloatFromInputField(Transform button)
    {
        var list = new List<float>();
        var _buttonData = button.GetComponent<ModeButton>();
        if (_buttonData.Getmode() == ChangeMode.TYPE)
        {
            if (inputFieldPreviews_type != null)
            {
                list = inputFieldPreviews_type[selectingEffectIndex * (int)(_buttonData.GetEffectTypeMode() - 1)];
            }
        }
        else if (button.GetComponent<ModeButton>().Getmode() == ChangeMode.HOLD)
        {
            if (inputFieldPreviews_long != null)
            {
                list =inputFieldPreviews_long[selectingEffectIndex * (int)(_buttonData.GetEffectHoldMode() - 1)];
            }
        }

        // 未設定なら抜ける
        if (list == null) return;
        if (list.Count == 0) return;

        int _overallIndex = 0;
        int _firstIndex = 0;
        int _morereadIndex = 0;
        foreach (Transform child in button)
        {
            if (child.gameObject.TryGetComponent<InputWindow>(out var iw))
            {
                for (int i = 0; i < iw.inputFieldCount; i++)
                {
                    if (_overallIndex >= list.Count) return;
                    iw.SetValue(list[_overallIndex], _firstIndex);
                    _overallIndex++;
                    _firstIndex++;
                }
            }
            else if (child.gameObject.CompareTag("MoreRead"))
            {
                foreach (Transform _child in child)
                {
                    if (_child.gameObject.TryGetComponent<InputWindow>(out var _iw))
                    {
                        for (int i = 0; i < _iw.inputFieldCount; i++)
                        {
                            if (_overallIndex >= list.Count) return;
                            _iw.SetValue(list[_overallIndex], _morereadIndex);
                            _overallIndex++;
                            _morereadIndex++;
                        }
                    }
                }
            }
        }
    }
    /// <summary>
    /// 予定のfloat値を、InputFieldから受け取ってセットする
    /// </summary>
    /// <param name="button"></param>
    public void SetFloatFromInputField(Transform button)
    {
        var list = new List<float>();
        var _newList = new List<float>();
        // 該当要素のデータを受け取る
        foreach (Transform child in button)
        {
            if (child.gameObject.TryGetComponent<InputWindow>(out var iw))
            {
                var valueList = iw.GetValueFromInputField();
                for (int i = 0; i < valueList.Count; i++)
                {
                    _newList.Add(valueList[i]);
                }
            }
            else if (child.gameObject.CompareTag("MoreRead"))
            {
                foreach (Transform _child in child)
                {
                    if (_child.gameObject.TryGetComponent<InputWindow>(out var _iw))
                    {
                        var _valueList = _iw.GetValueFromInputField();
                        for (int i = 0; i < _valueList.Count; i++)
                        {
                            _newList.Add(_valueList[i]);
                        }
                    }
                }
            }
        }

        // 受け取ったデータがあれば反映
        if (_newList.Count != 0)
        {
            var _buttonData = button.GetComponent<ModeButton>();
            if (_buttonData.Getmode() == ChangeMode.TYPE)
            {
                if (inputFieldPreviews_type != null)
                {
                    list = inputFieldPreviews_type[selectingEffectIndex * (int)(_buttonData.GetEffectTypeMode() - 1)];
                }
            }
            else if (_buttonData.Getmode() == ChangeMode.HOLD)
            {
                if (inputFieldPreviews_long != null)
                {
                    list = inputFieldPreviews_long[selectingEffectIndex * (int)(_buttonData.GetEffectHoldMode() - 1)];
                }
            }
            if (list != null)
            {
                list.Clear();
            }
            for(int i = 0; i < _newList.Count; i++)
            {
                list.Add(_newList[i]);
            }
        }
    }

    /// <summary>
    /// 変化の速さを設定する
    /// </summary>
    /// <param name="slider"></param>
    public void SetActionSpeed(float argSpeed)
    {
        fixedActionSpeed = 1f / argSpeed;
    }

    /// <summary>
    /// fixedActionSpeedを取得する
    /// </summary>
    /// <returns>変化のスピード</returns>
    public float GetFixedActionSpeed()
    {
        return fixedActionSpeed;
    }

    /// <summary>
    /// 設定中かどうかのフラグを指定通りに切り替える
    /// </summary>
    /// <param name="argIsSetting"></param>
    public void SetIsSetting(bool argIsSetting)
    {
        isSetting = argIsSetting;
    }

    /// <summary>
    /// 演出を設定中の判定を変更する
    /// </summary>
    /// <param name="argJudgeNum"></param>
    public void SetChaningJudgement(int argJudgeNum)
    {
        // インデックス番号が 1～3 であるため、補正する
        int _judgeNum = argJudgeNum + 1;

        _selectingEffectIndex = _judgeNum;

        // MISSの場合は、一部のボタンを隠す
        foreach(var obj in hideObjAtSetMiss)
        {
            obj.SetActive(argJudgeNum != 2);
        }
    }

    public void GoNextSettingPage()
    {
        showingSettingPage = (ChangeMode)Mathf.Min((float)(showingSettingPage + 1), (float)ChangeMode.HOLD);
    }
    public void GoBackSettingPage()
    {
        showingSettingPage = (ChangeMode)Mathf.Max((float)(showingSettingPage - 1), (float)ChangeMode.TYPE);
    }

    /// <summary>
    /// 演出の補正方法を設定する
    /// </summary>
    /// <param name="argNum"></param>
    public void SetFixAnimMode(int argNum)
    {
        fixAnimationMode = argNum;
    }
    #endregion

    #region 変化ごとの処理
    /// <summary>
    /// 適切な実行間隔になるように実行回数を調整する
    /// </summary>
    /// <param name="argWaitTime">変化の合計時間</param>
    /// <param name="argExeTimes">調整前の実行回数</param>
    /// <returns></returns>
    public float SetExeTimes(float argWaitTime, float argExeTimes)
    {
        float changed_exe_times = argExeTimes;
        //実行時間が 0.03秒以下 になるように調整
        for (int i = 0; changed_exe_times < argExeTimes * 100f; i++)
        {
            if (argWaitTime * fixedActionSpeed / changed_exe_times > 0.03f) { changed_exe_times++; }
            else { break; }
        }
        //実行間隔が 0.005秒以上 になるように調整
        for (int i = 0; changed_exe_times > 1; i++)
        {
            if (argWaitTime * fixedActionSpeed / changed_exe_times < 0.005f) { changed_exe_times--; }
            else { break; }
        }
        return changed_exe_times;
    }

    private void SquareAppearInit(Color color)
    {
        GameObject square = Instantiate(square_Appear.gameObject, squareGroup.transform);
        square.GetComponent<AppearSquare>().SetColor(color, orderNum_Type + orderNum_Hold);
    }

    private void SquarePargeInit(Color color)
    {
        GameObject square = Instantiate(square_Parge.gameObject, squareGroup.transform);
        square.GetComponent<PargeSquare>().SetColor(color, orderNum_Type + orderNum_Hold);
        pargingSquare = square;
    }

    /// <summary>
    /// 判定枠を回転させる
    /// </summary>
    /// <param name="angleGap">回転する角度(度)</param>
    /// <param name="waitTime">変化が完了するまでの時間</param>
    /// <returns></returns>
    IEnumerator RotateJudgeCircleGap(int angleGap, float waitTime)
    {
        completedLaneGroupRot = laneGroup.eulerAngles + new Vector3(0, 0, angleGap);//変化後の角度をリセット位置として設定
        int originalAngle = (int)laneGroup.transform.eulerAngles.z;
        yield return StartCoroutine(new AnimCoroutine(waitTime * fixedActionSpeed, AnimCoroutine.TFunctionType.FastToSlow, t =>
        {
            laneGroup.transform.eulerAngles = new Vector3(0, 0, originalAngle + angleGap * t);
        }).Anim());
    }

    /// <summary>
    /// 判定枠をパルスさせる
    /// </summary>
    /// <param name="waitTime_Ex">拡大が完了するまでの時間</param>
    /// <param name="waitTime_Sh">縮小が完了するまでの時間</param>
    /// <param name="maxScale">拡大完了時の大きさ</param>
    /// <param name="minScale">縮小完了時の大きさ</param>
    /// <returns></returns>
    IEnumerator SlimeCircle(float waitTime_Ex, float waitTime_Sh, float maxScale, float minScale)
    {
        completedJudgeCircleScale = judgeCircle.localScale;//変化前の大きさをリセット位置として設定

        //Expand
        if (maxScale >= 0)
        {
            Vector3 originalScale = judgeCircle.localScale;
            float gap = maxScale - originalScale.x;
            yield return StartCoroutine(new AnimCoroutine(waitTime_Ex * fixedActionSpeed, AnimCoroutine.TFunctionType.Liner, t =>
            {
                judgeCircle.localScale = originalScale + new Vector3(gap * t, gap * t, 0);
            }).Anim());
        }

        //Shrink
        if (minScale >= 0)
        {
            Vector3 _originalScale = judgeCircle.localScale;
            float _gap = _originalScale.x - minScale;
            yield return StartCoroutine(new AnimCoroutine(waitTime_Ex * fixedActionSpeed, AnimCoroutine.TFunctionType.Liner, t =>
            {
                judgeCircle.localScale = _originalScale + new Vector3(_gap * t, _gap * t, 0);
            }).Anim());
        }

        yield return null;
    }

    /// <summary>
    /// 判定枠を移動させる
    /// </summary>
    /// <param name="gap">移動距離も含めた方向ベクトル</param>
    /// <param name="waitTime">変化が完了するまでの時間</param>
    /// <param name="moveType">移動の方法</param>
    /// <param name="withLocal">レーンの向きに沿うかどうか</param>
    /// <returns></returns>
    IEnumerator MoveJudgeCircle(Vector3 gap, float waitTime, MovementType moveType, bool withLocal)
    {
        if (withLocal)
        {
            //Debug.Log($"{gap.x}, {gap.y} =>");
            float laneRot = laneGroup.transform.eulerAngles.z;
            float gapLength = gap.magnitude;
            float gapAngle = default;
            if (gap.x != 0)
            {
                gapAngle = Mathf.Atan2(gap.y, gap.x);
            }
            else
            {
                if (gap.y > 0)
                {
                    gapAngle = Mathf.PI / 2f;
                }
                else
                {
                    gapAngle = Mathf.PI * 3f / 2f;
                }
            }
            gap = new Vector3(gapLength * Mathf.Cos((Mathf.PI * laneRot / 180) + gapAngle), gapLength * Mathf.Sin((Mathf.PI * laneRot / 180) + gapAngle), 0);
            //Debug.Log($"{gap.x}, {gap.y}");
        }
        Vector3 pos = laneGroup.transform.position;
        switch (moveType)
        {
            case MovementType.STRAIGHT:
                yield return StartCoroutine(new AnimCoroutine(waitTime * fixedActionSpeed, AnimCoroutine.TFunctionType.Liner, t =>
                {
                    laneGroup.transform.position = pos + gap * t;
                }).Anim());
                break;

            case MovementType.QUICK:
                yield return StartCoroutine(new AnimCoroutine(waitTime * fixedActionSpeed, AnimCoroutine.TFunctionType.FastToSlow, t =>
                {
                    laneGroup.transform.position = pos + gap * t;
                }).Anim());
                break;

            case MovementType.SLOW:
                yield return StartCoroutine(new AnimCoroutine(waitTime * fixedActionSpeed, AnimCoroutine.TFunctionType.SlowToFast, t =>
                {
                    laneGroup.transform.position = pos + gap * t;
                }).Anim());
                break;

            case MovementType.WARP:
                yield return new WaitForSecondsRealtime(waitTime * fixedActionSpeed);
                laneGroup.transform.position = pos + gap;
                break;
        }



        yield return null;
    }

    /// <summary>
    /// 判定枠を往復移動させる
    /// </summary>
    /// <param name="toGap">行きの、移動距離も含めた方向ベクトル</param>
    /// <param name="returnGap">帰りの、移動距離も含めた方向ベクトル</param>
    /// <param name="waitTime">片道にかかる時間</param>
    /// <param name="toMovetype">行きの、移動方法</param>
    /// <param name="returnMovetype">帰りの、移動方法</param>
    /// <param name="withLocal">レーンの向きに沿うかどうか</param>
    /// <returns></returns>
    IEnumerator TurnMoveJudgeCircle(Vector3 toGap, Vector3 returnGap, float waitTime, MovementType toMovetype, MovementType returnMovetype, bool withLocal)
    {
        yield return StartCoroutine(MoveJudgeCircle(toGap, waitTime, toMovetype, withLocal));
        StartCoroutine(MoveJudgeCircle(returnGap, waitTime, returnMovetype, withLocal));
    }

    /// <summary>
    /// 判定枠を振動させる
    /// </summary>
    /// <param name="originalAmplitude">振動の振幅の半分</param>
    /// <param name="attenuation">減衰するかどうか</param>
    /// <param name="local">レーンの向きに沿うかどうか</param>
    /// <param name="withLane">レーンも振動させるかどうか</param>
    /// <returns></returns>
    IEnumerator VibeJudgeCircle(Vector3 originalAmplitude, bool attenuation, bool local, bool withLane)
    {
        completedLaneGroupPos = laneGroup.position;//変化前の位置をリセット位置として設定
        completedJudgeCirclePos = judgeCircle.position;//変化前の位置をリセット位置として設定

        Transform vibeObj = judgeCircle;
        if (withLane) { vibeObj = laneGroup; }//withLaneがtrueなら、レーンも動く
        float degZ = vibeObj.eulerAngles.z;
        if (local)
        {
            originalAmplitude = new Vector3(originalAmplitude.x * CosbyDeg(degZ) - originalAmplitude.y * SinbyDeg(degZ), originalAmplitude.x * SinbyDeg(degZ) + originalAmplitude.y * CosbyDeg(degZ), originalAmplitude.z);
        }
        Vector3 gap = Vector3.zero;
        if (attenuation)
        {
            gap = originalAmplitude * (1 / 7f);
        }
        float waitTime = 0.03f * fixedActionSpeed;
        Vector3 amplitude = originalAmplitude;
        Vector3 pos = vibeObj.position;
        Vector3 originalPos = pos;

        yield return StartCoroutine(new AnimCoroutine(waitTime, AnimCoroutine.TFunctionType.Liner, t =>
        {
            vibeObj.position = pos + amplitude * t;
        }).Anim());
        for (int j = 0; j < 3; j++)
        {
            amplitude -= gap;//移動幅を小さくする
            //順方向
            pos = vibeObj.position;
            yield return StartCoroutine(new AnimCoroutine(waitTime / 2f, AnimCoroutine.TFunctionType.Liner, t =>
            {
                vibeObj.position = pos + amplitude * t * -2f;
            }).Anim());
            amplitude -= gap;//移動幅を小さくする
            //逆方向
            pos = vibeObj.position;
            yield return StartCoroutine(new AnimCoroutine(waitTime / 2f, AnimCoroutine.TFunctionType.Liner, t =>
            {
                vibeObj.position = pos + amplitude * t * 2f;
            }).Anim());
        }
        pos = vibeObj.position;
        yield return StartCoroutine(new AnimCoroutine(waitTime, AnimCoroutine.TFunctionType.Liner, t =>
        {
            vibeObj.position = pos + amplitude * t * -1f;
        }).Anim());
        vibeObj.position = originalPos;
    }
    #endregion

    #region 状態のリセット
    /// <summary>
    /// 外側から呼び出せる、リセットの開始
    /// </summary>
    public void Reset()
    {
        StartCoroutine(ResetAction());
    }

    /// <summary>
    /// リセットの内容
    /// </summary>
    /// <returns></returns>
    IEnumerator ResetAction()
    {
        //演出の速度に合わせて一定時間待つ
        yield return new WaitForSeconds(0.8f * GetFixedActionSpeed());

        //リセット処理
        for (int i = 0; i < (int)EffectModeType.MAX; i++)
        {
            if (savedTypeActions[i] != null) { StopCoroutine(savedTypeActions[i]); }
        }
        for (int i = 0; i < (int)EffectModeHold.MAX; i++)
        {
            if (savedHoldActions[i] != null) { StopCoroutine(savedHoldActions[i]); }
        }

        switch (currentEffectMode_Type)
        {
            case EffectModeType.LANE_ROTATE_1:
                if (currentEffectMode_Hold != EffectModeHold.LANE_ROTATE_1)
                {
                    if ((int)laneGroup.eulerAngles.z != 0)
                    {
                        StartCoroutine(RotateJudgeCircleGap(360 - (int)laneGroup.eulerAngles.z, 0.3f));
                    }
                }
                break;
            case EffectModeType.BACK_EXPAND_WITH_ROTATION_1:
                //SquareAppearInit(new Color(0.0863f, 0.0863f, 0.0863f));
                Invoke("SeparatedSquareReset", 1f);
                break;
            default:
                break;
        }
        switch (currentEffectMode_Hold)
        {
            case EffectModeHold.LANE_ROTATE_1:
                if ((int)laneGroup.eulerAngles.z != 0)
                {
                    StartCoroutine(RotateJudgeCircleGap(360 - (int)laneGroup.eulerAngles.z, 0.3f));
                }
                break;
            case EffectModeHold.LANE_ARROW_1:
                laneGroup.transform.position = Vector3.zero;
                break;
            case EffectModeHold.BACK_PARGE_1:
                //SquareAppearInit(new Color(0.0863f, 0.0863f, 0.0863f));
                Invoke("SeparatedSquareReset", 1f);
                break;
            default:
                break;
        }

        orderNum_Type = 0;
        orderNum_Hold = 0;
    }

    /// <summary>
    /// 外側から呼び出せる、ゲーム終了処理の開始
    /// </summary>
    public void End()
    {
        StartCoroutine(EndAction());
    }

    /// <summary>
    /// ゲーム終了処理
    /// </summary>
    /// <returns></returns>
    IEnumerator EndAction()
    {
        // 演出のスピードに合わせて一定時間待つ
        yield return new WaitForSeconds(0.5f * GetFixedActionSpeed());

        // ノーツを全て削除する
        for(int judgeObjCount = 0; judgeObjCount < judgeObjs.Length; judgeObjCount++)
        {
            foreach(Transform child in judgeObjs[judgeObjCount])
            {
                if(child.gameObject.TryGetComponent<Notes>(out var notes))
                {
                    Destroy(child.gameObject);
                }
            }
        }

        canStart = true;
    }

    private void SquareReset()
    {
        foreach (GameObject square in Squares)
        {
            square.SetActive(false);
        }
    }

    private void SeparatedSquareReset()
    {
        foreach (Transform square in squareGroup.transform)
        {
            Destroy(square.gameObject);
        }
    }
    #endregion

    #region 便利な関数
    /// <summary>
    /// 度数法でSin関数を計算
    /// </summary>
    /// <param name="deg">角度θ</param>
    /// <returns></returns>
    private float SinbyDeg(float deg)
    {
        return Mathf.Sin(Mathf.PI * deg / 180f);
    }

    /// <summary>
    /// 度数法でCos関数を計算
    /// </summary>
    /// <param name="deg">角度θ</param>
    /// <returns></returns>
    private float CosbyDeg(float deg)
    {
        return Mathf.Cos(Mathf.PI * deg / 180f);
    }
    #endregion

    /*
    [ContextMenu("演出を変更する判定の変更")]
    public void ChangeJudgementInChange()
    {
        Debug.Log($"{(EnumData.Judgement)_selectingEffectIndex}");
        _selectingEffectIndex = (_selectingEffectIndex + 1) % (int)EnumData.Judgement.MAX;
        if ((EnumData.Judgement)_selectingEffectIndex == EnumData.Judgement.NONE) _selectingEffectIndex++;
        Debug.Log($"=> {(EnumData.Judgement)_selectingEffectIndex}");
    }
    [ContextMenu("判定後との演出を表示")]
    public void ShowDirectionByJudge()
    {
        for(int i = 1; i < (int)EnumData.Judgement.MAX; i++)
        {
            Debug.Log($"【{(EnumData.Judgement)i}】Type:{(EffectModeType)effectMode_Type[i]}, Hold:{(EffectModeHold)effectMode_Hold[i]}");
        }
    }
    */
}
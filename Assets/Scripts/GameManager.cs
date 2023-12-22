using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using AngoraUtility;

public class GameManager : MonoBehaviour
{
    // --- �萔 ---
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

    // --- �ϐ� ---
    [SerializeField] PlayableDirector playableDirector; // �^�C�����C��
    [SerializeField] NotesGenerator notesGenerator;     // �m�[�c�����I�u�W�F�N�g
    [SerializeField] ChangeTimeLine changeTimeLine;     // �y�Ȃ��Z�b�g���邽�߂̃N���X
    [SerializeField] GameObject canvas;     // UI
    [SerializeField] Transform laneGroup;   // ���[���S��
    [SerializeField] Transform judgeCircle; // ����g����
    [SerializeField] Transform[] judgeObjs; // ���ꂼ��̃��[���S��
    private SpriteRenderer judgeSr;         // ����g��SpriteRenderer
    public static EffectModeType[] effectMode_Type = new EffectModeType[(int)EnumData.Judgement.MAX]; // ���o�̎��
    public static EffectModeHold[] effectMode_Hold = new EffectModeHold[(int)EnumData.Judgement.MAX];
    private EffectModeType currentEffectMode_Type = EffectModeType.NONE;                              // ���ݎ��s���̉��o
    private EffectModeHold currentEffectMode_Hold = EffectModeHold.NONE;
    private static int _selectingEffectIndex = (int)EnumData.Judgement.PERFECT;                       // ���o��ύX���̃C���f�b�N�X�ԍ�
    public static int selectingEffectIndex { get { return _selectingEffectIndex; } }
    private int orderNum_Type = 0;  // ���ԊǗ� 
    private int orderNum_Hold = 0;
    [SerializeField] GameObject[] Squares;// �w�i�̊i�[�p
    [SerializeField] FlashSquare flashSquare;
    [SerializeField] AppearSquare square_Appear;    // �����O�̎l�p
    [SerializeField] PargeSquare square_Parge;      // �p�[�W����l�p
    private GameObject pargingSquare;
    [SerializeField] GameObject squareGroup;        // �l�p���������Ƃ���

    private List<Color>[] colorPreviews_appear;       // �F���i�[����Ƃ���
    private List<Color>[] colorPreviews_parge;        // �F���i�[����Ƃ���
    private List<bool>[] boolPreviews;                // bool�l���i�[����Ƃ���
    private List<int>[] modePreviews;                 // dropdown��Value�̒l���i�[����Ƃ���
    private List<float>[] inputFieldPreviews_type;    // type�ɂ�����inputField�̒l���i�[����Ƃ���
    private List<float>[] inputFieldPreviews_long;    // long�ɂ�����inputField�̒l���i�[����Ƃ���
    private EnumData.Judgement catchLongJudge;        // �����O�m�[�c�n�_�ɂ����锻��

    private bool isSetting = false; // �ݒ蒆���ǂ���
    private bool canStart = true;   // �v���C�����ۂ�
    private float fixedActionSpeed = 1.0f;  // �ω��̑��x
    private Coroutine savedCoroutine = default;     // �R���[�`���̕ۑ��p
    private Coroutine[] savedTypeActions = default;
    private Coroutine[] savedHoldActions = default;
    private Vector3 completedLaneGroupPos;      // �ω�������������̔���O���[�v�̈ʒu
    private Vector3 completedJudgeCirclePos;    // �ω�������������̔���g�̈ʒu
    private Vector3 completedJudgeCircleScale;  // �ω�������������̔���g�̑傫��
    private Vector3 completedLaneGroupRot;      // �ω�������������̔���O���[�v�̊p�x
    [SerializeField] GameObject[] hideObjAtSetMiss; // Miss�̉��o��ݒ肷��Ƃ��ɔ�\���ɂ���I�u�W�F�N�g

    public static ChangeMode showingSettingPage = ChangeMode.TYPE;    // ���ݐݒ��ʂɕ\�����̃y�[�W�ԍ�
    [SerializeField] Text settingPageHeaderText;                      // �ݒ��ʂ̃w�b�_�[Text
    [SerializeField] GameObject settingPageNext;                      // �ݒ��ʂ̐i�ޖ��
    [SerializeField] GameObject settingPageBack;                      // �ݒ��ʂ̖߂���

    [HideInInspector] public int fixAnimationMode = FIXANIMMODE_COMPLETE;   // ���o�␳�̕��@
    private float nextRotAnimDeg = 0;                                       // ��]���o�̎��̊p�x

    [Header("�e�X�g�p --------------------------------------------------------------------")]
    [SerializeField] EffectModeType testMode_Type = EffectModeType.NONE;
    [SerializeField] EffectModeHold testMode_Hold = EffectModeHold.NONE;
    [SerializeField] bool test = false; // �e�X�g�p���ۂ�

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
        // �ݒ�p�y�[�W�̍X�V
        if(showingSettingPage == ChangeMode.TYPE)
        {
            settingPageHeaderText.text = "1�񉟂�";
            settingPageNext.SetActive(true);
            settingPageBack.SetActive(false);
        }
        if(showingSettingPage == ChangeMode.HOLD)
        {
            settingPageHeaderText.text = "������������";
            settingPageNext.SetActive(false);
            settingPageBack.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.Alpha0) && canStart && !isSetting)
        {
            if (test)//�e�X�g�p
            {
                int effectLength = (int)EnumData.Judgement.MAX;
                for(int i = 0; i < effectLength; i++)
                {
                    effectMode_Type[i] = testMode_Type;
                    effectMode_Hold[i] = testMode_Hold;
                }
            }

            // �l�̏�����
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

        // ����g�̐F��߂�
        FadeOutJudgeColor();
    }

    #region �ω��̎��s����
    /// <summary>
    /// �ω����ɍĂѕω����s�����߂̃��Z�b�g����
    /// </summary>
    /// <param name="argTypeMode">�ω��̎�ށi�^�C�v�j</param>
    /// <param name="isPosReset">�ʒu�����Z�b�g���邩�ǂ���</param>
    /// <param name="isScaleReset">�傫�������Z�b�g���邩�ǂ���</param>
    /// <param name="isRotReset">�p�x�����Z�b�g���邩�ǂ���</param>
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
    /// �ω����ɍĂѕω����s�����߂̃��Z�b�g����
    /// </summary>
    /// <param name="argHoldMode">�ω��̎�ށi�z�[���h�j</param>
    /// <param name="isPosReset">�ʒu�����Z�b�g���邩�ǂ���</param>
    /// <param name="isScaleReset">�傫�������Z�b�g���邩�ǂ���</param>
    /// <param name="isRotReset">�p�x�����Z�b�g���邩�ǂ���</param>
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
        // �ŐV�̉��o��ۑ�
        currentEffectMode_Type = effectMode_Type[(int)argJudgement];
        // ����̔ԍ� + 1��ۑ�
        int judgeIndex = (int)argJudgement;
        // ���o�̔ԍ���ۑ�
        int directionIndex = (int)currentEffectMode_Type - 1;

        Debug.Log($"judge:{judgeIndex}, dir:{directionIndex}");

        // �m�[�c���̍X�V
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

                    //�uorderNum�v�ňꕔ�ω������邱�Ƃ��\
                }
                break;
            case EffectModeType.LANE_SLIME_1:
                if (isAction)
                {
                    RestartAction(currentEffectMode_Type, false, true, false);

                    savedTypeActions[(int)currentEffectMode_Type] = StartCoroutine(SlimeCircle(0.09f, 0.09f, 20, 16));

                    //�uorderNum�v�ňꕔ�ω������邱�Ƃ��\
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
                        case 0: moveVec = new Vector3(1.5f, 0, 0); break;           //���s
                        case 1: moveVec = new Vector3(0, 1.5f, 0); break;           //����
                        case 2: moveVec = new Vector3(customX, customY, 0); break;  //�J�X�^��
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
        // �ŐV�̉��o�Ǝn�_�̔����ۑ��i�ŏ������j
        if (orderNum_Hold == 0)
        {
            currentEffectMode_Hold = effectMode_Hold[(int)argJudgement];
            catchLongJudge = argJudgement;
        }
        // ����̔ԍ���ۑ�
        int judgeIndex = (int)argJudgement;
        // ���o�̔ԍ���ۑ�
        int directionIndex = (int)currentEffectMode_Hold - 1;

        switch (currentEffectMode_Hold)
        {
            case EffectModeHold.NONE:
                break;
            case EffectModeHold.LANE_ROTATE_1:
                if (isAction && (orderNum_Hold == 0 || (orderNum_Hold > 0 && argJudgement != EnumData.Judgement.MISS)))
                {
                    RestartAction(currentEffectMode_Hold, false, false, false);//���O�̕ω������Z�b�g���Ȃ�
                    RestartAction(EffectModeType.LANE_ROTATE_1, false, false, false);//���O�̕ω������Z�b�g���Ȃ�
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
                else if (orderNum_Hold == 1)//�r���ŗ������ꍇ
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
    /// ����ɍ��킹�Ĕ���g�̐F��ύX����
    /// </summary>
    /// <param name="judge">����</param>
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
    /// ����g�̐F�������Â��ɖ߂�
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

    #region ���s�O�̏���
    /// <summary>
    /// �\��̐F���A�ȑO�̂��̂ɍ��킹��
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

        // ���ݒ�Ȃ甲����
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
    /// �\��̐F���Z�b�g����
    /// </summary>
    /// <param name="button"></param>
    public void SetColors(Transform button)
    {
        var list = new List<Color>();
        var _newList = new List<Color>();
        // �Y���v�f�̃f�[�^���󂯎��
        foreach (Transform child in button)
        {
            if (child.gameObject.TryGetComponent<ColorImageData>(out var data) && child.gameObject.TryGetComponent<Image>(out Image image))
            {
                _newList.Add(image.color);
            }
        }

        // �󂯎�����f�[�^������Δ��f
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
    /// �\���bool�l���A�ȑO�̂��̂ɍ��킹��
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

        // ���ݒ�Ȃ珉���l�ɂ���
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
    /// �\���bool�l���Z�b�g����
    /// </summary>
    /// <param name="button"></param>
    public void SetBools(Transform button)
    {
        var list = new List<bool>();
        var _newList = new List<bool>();
        // �Y���v�f�̃f�[�^���󂯎��
        foreach (Transform child in button)
        {
            if (child.gameObject.TryGetComponent<BoolWindow>(out BoolWindow bw))
            {
                _newList.Add(bw.GetBool());
            }
        }

        // �󂯎�����f�[�^������Δ��f
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
    /// �\���int�l���A�ȑO�̂��̂ɍ��킹��
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

        // ���ݒ�Ȃ甲����
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
    /// �\���int�l���ADropdown����󂯎���ăZ�b�g����
    /// </summary>
    /// <param name="button"></param>
    public void SetIntFromDropdown(Transform button)
    {
        var list = new List<int>();
        var _newList = new List<int>();
        // �Y���v�f�̃f�[�^���󂯎��
        foreach (Transform child in button)
        {
            if (child.gameObject.TryGetComponent<DropdownWindow>(out DropdownWindow ddw))
            {
                _newList.Add(ddw.GetValueFromDropdown());
            }
        }

        // �󂯎�����f�[�^������Δ��f
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
    /// �\���float�l���A�ȑO�̂��̂ɍ��킹��
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

        // ���ݒ�Ȃ甲����
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
    /// �\���float�l���AInputField����󂯎���ăZ�b�g����
    /// </summary>
    /// <param name="button"></param>
    public void SetFloatFromInputField(Transform button)
    {
        var list = new List<float>();
        var _newList = new List<float>();
        // �Y���v�f�̃f�[�^���󂯎��
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

        // �󂯎�����f�[�^������Δ��f
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
    /// �ω��̑�����ݒ肷��
    /// </summary>
    /// <param name="slider"></param>
    public void SetActionSpeed(float argSpeed)
    {
        fixedActionSpeed = 1f / argSpeed;
    }

    /// <summary>
    /// fixedActionSpeed���擾����
    /// </summary>
    /// <returns>�ω��̃X�s�[�h</returns>
    public float GetFixedActionSpeed()
    {
        return fixedActionSpeed;
    }

    /// <summary>
    /// �ݒ蒆���ǂ����̃t���O���w��ʂ�ɐ؂�ւ���
    /// </summary>
    /// <param name="argIsSetting"></param>
    public void SetIsSetting(bool argIsSetting)
    {
        isSetting = argIsSetting;
    }

    /// <summary>
    /// ���o��ݒ蒆�̔����ύX����
    /// </summary>
    /// <param name="argJudgeNum"></param>
    public void SetChaningJudgement(int argJudgeNum)
    {
        // �C���f�b�N�X�ԍ��� 1�`3 �ł��邽�߁A�␳����
        int _judgeNum = argJudgeNum + 1;

        _selectingEffectIndex = _judgeNum;

        // MISS�̏ꍇ�́A�ꕔ�̃{�^�����B��
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
    /// ���o�̕␳���@��ݒ肷��
    /// </summary>
    /// <param name="argNum"></param>
    public void SetFixAnimMode(int argNum)
    {
        fixAnimationMode = argNum;
    }
    #endregion

    #region �ω����Ƃ̏���
    /// <summary>
    /// �K�؂Ȏ��s�Ԋu�ɂȂ�悤�Ɏ��s�񐔂𒲐�����
    /// </summary>
    /// <param name="argWaitTime">�ω��̍��v����</param>
    /// <param name="argExeTimes">�����O�̎��s��</param>
    /// <returns></returns>
    public float SetExeTimes(float argWaitTime, float argExeTimes)
    {
        float changed_exe_times = argExeTimes;
        //���s���Ԃ� 0.03�b�ȉ� �ɂȂ�悤�ɒ���
        for (int i = 0; changed_exe_times < argExeTimes * 100f; i++)
        {
            if (argWaitTime * fixedActionSpeed / changed_exe_times > 0.03f) { changed_exe_times++; }
            else { break; }
        }
        //���s�Ԋu�� 0.005�b�ȏ� �ɂȂ�悤�ɒ���
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
    /// ����g����]������
    /// </summary>
    /// <param name="angleGap">��]����p�x(�x)</param>
    /// <param name="waitTime">�ω�����������܂ł̎���</param>
    /// <returns></returns>
    IEnumerator RotateJudgeCircleGap(int angleGap, float waitTime)
    {
        completedLaneGroupRot = laneGroup.eulerAngles + new Vector3(0, 0, angleGap);//�ω���̊p�x�����Z�b�g�ʒu�Ƃ��Đݒ�
        int originalAngle = (int)laneGroup.transform.eulerAngles.z;
        yield return StartCoroutine(new AnimCoroutine(waitTime * fixedActionSpeed, AnimCoroutine.TFunctionType.FastToSlow, t =>
        {
            laneGroup.transform.eulerAngles = new Vector3(0, 0, originalAngle + angleGap * t);
        }).Anim());
    }

    /// <summary>
    /// ����g���p���X������
    /// </summary>
    /// <param name="waitTime_Ex">�g�傪��������܂ł̎���</param>
    /// <param name="waitTime_Sh">�k������������܂ł̎���</param>
    /// <param name="maxScale">�g�劮�����̑傫��</param>
    /// <param name="minScale">�k���������̑傫��</param>
    /// <returns></returns>
    IEnumerator SlimeCircle(float waitTime_Ex, float waitTime_Sh, float maxScale, float minScale)
    {
        completedJudgeCircleScale = judgeCircle.localScale;//�ω��O�̑傫�������Z�b�g�ʒu�Ƃ��Đݒ�

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
    /// ����g���ړ�������
    /// </summary>
    /// <param name="gap">�ړ��������܂߂������x�N�g��</param>
    /// <param name="waitTime">�ω�����������܂ł̎���</param>
    /// <param name="moveType">�ړ��̕��@</param>
    /// <param name="withLocal">���[���̌����ɉ������ǂ���</param>
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
    /// ����g�������ړ�������
    /// </summary>
    /// <param name="toGap">�s���́A�ړ��������܂߂������x�N�g��</param>
    /// <param name="returnGap">�A��́A�ړ��������܂߂������x�N�g��</param>
    /// <param name="waitTime">�Г��ɂ����鎞��</param>
    /// <param name="toMovetype">�s���́A�ړ����@</param>
    /// <param name="returnMovetype">�A��́A�ړ����@</param>
    /// <param name="withLocal">���[���̌����ɉ������ǂ���</param>
    /// <returns></returns>
    IEnumerator TurnMoveJudgeCircle(Vector3 toGap, Vector3 returnGap, float waitTime, MovementType toMovetype, MovementType returnMovetype, bool withLocal)
    {
        yield return StartCoroutine(MoveJudgeCircle(toGap, waitTime, toMovetype, withLocal));
        StartCoroutine(MoveJudgeCircle(returnGap, waitTime, returnMovetype, withLocal));
    }

    /// <summary>
    /// ����g��U��������
    /// </summary>
    /// <param name="originalAmplitude">�U���̐U���̔���</param>
    /// <param name="attenuation">�������邩�ǂ���</param>
    /// <param name="local">���[���̌����ɉ������ǂ���</param>
    /// <param name="withLane">���[�����U�������邩�ǂ���</param>
    /// <returns></returns>
    IEnumerator VibeJudgeCircle(Vector3 originalAmplitude, bool attenuation, bool local, bool withLane)
    {
        completedLaneGroupPos = laneGroup.position;//�ω��O�̈ʒu�����Z�b�g�ʒu�Ƃ��Đݒ�
        completedJudgeCirclePos = judgeCircle.position;//�ω��O�̈ʒu�����Z�b�g�ʒu�Ƃ��Đݒ�

        Transform vibeObj = judgeCircle;
        if (withLane) { vibeObj = laneGroup; }//withLane��true�Ȃ�A���[��������
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
            amplitude -= gap;//�ړ���������������
            //������
            pos = vibeObj.position;
            yield return StartCoroutine(new AnimCoroutine(waitTime / 2f, AnimCoroutine.TFunctionType.Liner, t =>
            {
                vibeObj.position = pos + amplitude * t * -2f;
            }).Anim());
            amplitude -= gap;//�ړ���������������
            //�t����
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

    #region ��Ԃ̃��Z�b�g
    /// <summary>
    /// �O������Ăяo����A���Z�b�g�̊J�n
    /// </summary>
    public void Reset()
    {
        StartCoroutine(ResetAction());
    }

    /// <summary>
    /// ���Z�b�g�̓��e
    /// </summary>
    /// <returns></returns>
    IEnumerator ResetAction()
    {
        //���o�̑��x�ɍ��킹�Ĉ�莞�ԑ҂�
        yield return new WaitForSeconds(0.8f * GetFixedActionSpeed());

        //���Z�b�g����
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
    /// �O������Ăяo����A�Q�[���I�������̊J�n
    /// </summary>
    public void End()
    {
        StartCoroutine(EndAction());
    }

    /// <summary>
    /// �Q�[���I������
    /// </summary>
    /// <returns></returns>
    IEnumerator EndAction()
    {
        // ���o�̃X�s�[�h�ɍ��킹�Ĉ�莞�ԑ҂�
        yield return new WaitForSeconds(0.5f * GetFixedActionSpeed());

        // �m�[�c��S�č폜����
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

    #region �֗��Ȋ֐�
    /// <summary>
    /// �x���@��Sin�֐����v�Z
    /// </summary>
    /// <param name="deg">�p�x��</param>
    /// <returns></returns>
    private float SinbyDeg(float deg)
    {
        return Mathf.Sin(Mathf.PI * deg / 180f);
    }

    /// <summary>
    /// �x���@��Cos�֐����v�Z
    /// </summary>
    /// <param name="deg">�p�x��</param>
    /// <returns></returns>
    private float CosbyDeg(float deg)
    {
        return Mathf.Cos(Mathf.PI * deg / 180f);
    }
    #endregion

    /*
    [ContextMenu("���o��ύX���锻��̕ύX")]
    public void ChangeJudgementInChange()
    {
        Debug.Log($"{(EnumData.Judgement)_selectingEffectIndex}");
        _selectingEffectIndex = (_selectingEffectIndex + 1) % (int)EnumData.Judgement.MAX;
        if ((EnumData.Judgement)_selectingEffectIndex == EnumData.Judgement.NONE) _selectingEffectIndex++;
        Debug.Log($"=> {(EnumData.Judgement)_selectingEffectIndex}");
    }
    [ContextMenu("�����Ƃ̉��o��\��")]
    public void ShowDirectionByJudge()
    {
        for(int i = 1; i < (int)EnumData.Judgement.MAX; i++)
        {
            Debug.Log($"�y{(EnumData.Judgement)i}�zType:{(EffectModeType)effectMode_Type[i]}, Hold:{(EffectModeHold)effectMode_Hold[i]}");
        }
    }
    */
}
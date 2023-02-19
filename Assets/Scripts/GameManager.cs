using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;

public class GameManager : MonoBehaviour
{
    // --- �萔 ---
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
    [SerializeField] GameObject canvas;     // UI
    [SerializeField] Transform laneGroup;   // ���[���S��
    [SerializeField] Transform judgeCircle; // ����g����
    public static EffectModeType effectMode_Type = EffectModeType.NONE; // ���o�̎��
    public static EffectModeHold effectMode_Hold = EffectModeHold.NONE;
    private int orderNum_Type = 0;  // ���ԊǗ� 
    private int orderNum_Hold = 0;
    [SerializeField] GameObject[] Squares;// �w�i�̊i�[�p
    [SerializeField] GameObject flashSquare;
    [SerializeField] AppearSquare square_Appear;    // �����O�̎l�p
    [SerializeField] PargeSquare square_Parge;      // �p�[�W����l�p
    private GameObject pargingSquare;
    [SerializeField] GameObject squareGroup;        // �l�p���������Ƃ���
    private List<Color> colorPreviews_appear;       // �F���i�[����Ƃ���
    private List<Color> colorPreviews_parge;        // �F���i�[����Ƃ���
    private List<bool> boolPreviews;                // bool�l���i�[����Ƃ���
    private List<int> modePreviews;                 // dropdown��Value�̒l���i�[����Ƃ���
    private List<float> inputFieldPreviews_type;    // type�ɂ�����inputField�̒l���i�[����Ƃ���
    private bool isSetting = false; // �ݒ蒆���ǂ���
    private bool canStart = true;   // �v���C�����ۂ�
    private float fixedActionSpeed = 1.0f;  // �ω��̑��x
    private Coroutine savedCoroutine = default;     // �R���[�`���̕ۑ��p
    private Coroutine[] savedTypeActions = default;
    private Coroutine[] savedHoldActions = default;
    private Vector3 planeGroupPos;                  // �ω������s�����O�̔���O���[�v�̈ʒu
    private Vector3 pjudgeCirclePos;                // �ω������s�����O�̔���g�̈ʒu
    private Vector3 pjudgeCircleScale;              // �ω������s�����O�̔���g�̑傫��
    private Vector3 planeGroupRot;                  // �ω������s�����O�̔���O���[�v�̊p�x

    [SerializeField] EffectModeType testMode_Type = EffectModeType.NONE;
    [SerializeField] EffectModeHold testMode_Hold = EffectModeHold.NONE;
    [SerializeField] bool test = false; // �e�X�g�p���ۂ�

    void Start()
    {
        colorPreviews_appear = new List<Color>();
        colorPreviews_parge = new List<Color>();
        boolPreviews = new List<bool>();
        modePreviews = new List<int>();
        inputFieldPreviews_type = new List<float>();
        savedTypeActions = new Coroutine[(int)EffectModeType.MAX];
        savedHoldActions = new Coroutine[(int)EffectModeHold.MAX];
        planeGroupPos = laneGroup.position;
        pjudgeCirclePos = judgeCircle.position;
        pjudgeCircleScale = judgeCircle.localScale;
        planeGroupRot = laneGroup.eulerAngles;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0) && canStart && !isSetting)
        {
            if (test)//�e�X�g�p
            {
                effectMode_Type = testMode_Type;
                effectMode_Hold = testMode_Hold;
            }

            canStart = false;
            playableDirector.Stop();
            playableDirector.Play();
        }

        if (canStart)
        {
            canvas.SetActive(true);
        }
        else
        {
            canvas.SetActive(false);
        }
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
            if (isPosReset) { laneGroup.position = planeGroupPos; judgeCircle.position = pjudgeCirclePos; }
            if (isScaleReset) { judgeCircle.localScale = pjudgeCircleScale; }
            if (isRotReset) { laneGroup.eulerAngles = planeGroupRot; }
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
            if (isPosReset) { laneGroup.position = planeGroupPos; judgeCircle.position = pjudgeCirclePos; }
            if (isScaleReset) { judgeCircle.localScale = pjudgeCircleScale; }
            if (isRotReset) { laneGroup.eulerAngles = planeGroupRot; }
        }
    }

    public void TypeAction(bool isAction = true)
    {
        switch (effectMode_Type)
        {
            case EffectModeType.NONE:
                break;
            case EffectModeType.LANE_ROTATE_1:
                if (isAction)
                {
                    RestartAction(effectMode_Type, false, false, true);

                    savedTypeActions[(int)effectMode_Type] = StartCoroutine(RotateJudgeCircleGap(90, 0.3f));

                    //�uorderNum�v�ňꕔ�ω������邱�Ƃ��\
                }
                break;
            case EffectModeType.LANE_SLIME_1:
                if (isAction)
                {
                    RestartAction(effectMode_Type, false, true, false);

                    savedTypeActions[(int)effectMode_Type] = StartCoroutine(SlimeCircle(0.09f, 0.09f, 20, 16));

                    //�uorderNum�v�ňꕔ�ω������邱�Ƃ��\
                }
                break;
            case EffectModeType.LANE_VIBE_1:
                if (isAction)
                {
                    float customX = inputFieldPreviews_type[0];
                    float customY = inputFieldPreviews_type[1];

                    RestartAction(effectMode_Type, true, false, false);

                    Vector3 moveVec = Vector3.zero;
                    Debug.Log(modePreviews[0]);
                    switch (modePreviews[0])
                    {
                        case 0: moveVec = new Vector3(1.5f, 0, 0); break;           //���s
                        case 1: moveVec = new Vector3(0, 1.5f, 0); break;           //����
                        case 2: moveVec = new Vector3(customX, customY, 0); break;  //�J�X�^��
                    }

                    savedTypeActions[(int)effectMode_Type] = StartCoroutine(VibeJudgeCircle(moveVec, boolPreviews[0], boolPreviews[1], boolPreviews[2]));
                }
                break;
            case EffectModeType.BACK_EXPAND_WITH_ROTATION_1:
                if (isAction)
                {
                    switch (orderNum_Type % 2)
                    {
                        case 0:
                            SquareAppearInit(colorPreviews_appear[0]);
                            break;
                        case 1:
                            SquareAppearInit(colorPreviews_appear[1]);
                            break;
                    }
                }
                break;
            case EffectModeType.BACK_FLASH_1:
                if (isAction)
                {
                    flashSquare.SetActive(false);
                    flashSquare.SetActive(true);
                }
                break;
        }

        orderNum_Type++;
    }

    public void HoldAction(bool isAction = true)
    {
        switch (effectMode_Hold)
        {
            case EffectModeHold.NONE:
                break;
            case EffectModeHold.LANE_ROTATE_1:
                if (isAction)
                {
                    RestartAction(effectMode_Hold, false, false, false);//���O�̕ω������Z�b�g���Ȃ�
                    RestartAction(EffectModeType.LANE_ROTATE_1, false, false, false);//���O�̕ω������Z�b�g���Ȃ�
                    RestartAction(EffectModeType.LANE_VIBE_1, true, false, false);
                    RestartAction(EffectModeType.LANE_SLIME_1, false, true, false);

                    switch (orderNum_Hold)
                    {
                        case 0:
                            savedHoldActions[(int)effectMode_Hold] = StartCoroutine(RotateJudgeCircleGap(-120, 0.8f));
                            break;
                        case 1:
                            savedHoldActions[(int)effectMode_Hold] = StartCoroutine(RotateJudgeCircleGap(360 - (int)laneGroup.eulerAngles.z + 360, 0.3f));
                            break;
                    }
                }
                break;
            case EffectModeHold.LANE_ARROW_1:
                if (isAction)
                {
                    RestartAction(EffectModeType.LANE_ROTATE_1, false, false, false);

                    switch (orderNum_Hold)
                    {
                        case 0:
                            RestartAction(EffectModeType.LANE_VIBE_1, true, false, false);
                            RestartAction(EffectModeType.LANE_SLIME_1, false, true, false);

                            float laneRot = laneGroup.transform.eulerAngles.z;
                            savedCoroutine = StartCoroutine(MoveJudgeCircle(new Vector3(-10, 0, 0), 1.0f, MovementType.STRAIGHT, true));
                            break;
                        case 1:
                            StopCoroutine(savedCoroutine);
                            savedHoldActions[(int)effectMode_Hold] = StartCoroutine(TurnMoveJudgeCircle(new Vector3(laneGroup.position.magnitude * 4, 0, 0), new Vector3(laneGroup.position.magnitude * -3, 0, 0), 0.2f, MovementType.QUICK, MovementType.SLOW, true));
                            break;
                    }
                }
                else if (orderNum_Hold == 1)//�r���ŗ������ꍇ
                {
                    StopCoroutine(savedCoroutine);
                    savedHoldActions[(int)effectMode_Hold] = StartCoroutine(TurnMoveJudgeCircle(new Vector3(laneGroup.position.magnitude * 3, 0, 0), new Vector3(laneGroup.position.magnitude * -2, 0, 0), 0.2f, MovementType.QUICK, MovementType.SLOW, true));
                }
                break;
            case EffectModeHold.BACK_PARGE_1:
                if (isAction)
                {
                    switch (orderNum_Hold)
                    {
                        case 0:
                            SquarePargeInit(colorPreviews_parge[0]);
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
    #endregion

    #region ���s�O�̏���
    /// <summary>
    /// �\��̐F���Z�b�g����
    /// </summary>
    /// <param name="button"></param>
    public void SetColors(Transform button)
    {
        var list = new List<Color>();
        switch (button.gameObject.GetComponent<ModeButton>().Getmode())
        {
            case ChangeMode.TYPE:
                list = colorPreviews_appear;
                break;
            case ChangeMode.HOLD:
                list = colorPreviews_parge;
                break;
            default:
                break;
        }
        if (list != null)
        {
            list.Clear();
        }
        foreach (Transform child in button)
        {
            if (child.gameObject.TryGetComponent<Image>(out Image image))
            {
                list.Add(image.color);
            }
        }
    }

    /// <summary>
    /// �\���bool�l���Z�b�g����
    /// </summary>
    /// <param name="button"></param>
    public void SetBools(Transform button)
    {
        if (boolPreviews != null)
        {
            boolPreviews.Clear();
        }
        foreach (Transform child in button)
        {
            if (child.gameObject.TryGetComponent<BoolWindow>(out BoolWindow bw))
            {
                boolPreviews.Add(bw.GetBool());
            }
        }
    }

    /// <summary>
    /// �\���int�l���ADropdown����󂯎���ăZ�b�g����
    /// </summary>
    /// <param name="button"></param>
    public void SetIntFromDropdown(Transform button)
    {
        if (modePreviews != null)
        {
            modePreviews.Clear();
        }
        foreach (Transform child in button)
        {
            if (child.gameObject.TryGetComponent<DropdownWindow>(out DropdownWindow ddw))
            {
                modePreviews.Add(ddw.GetValueFromDropdown());
            }
        }
    }

    /// <summary>
    /// �\���float�l���AInputField����󂯎���ăZ�b�g����
    /// </summary>
    /// <param name="button"></param>
    public void SetFloatFromInputField(Transform button)
    {
        if (inputFieldPreviews_type != null)
        {
            inputFieldPreviews_type.Clear();
        }
        foreach (Transform child in button)
        {
            if (child.gameObject.TryGetComponent<InputWindow>(out var iw))
            {
                var valueList = iw.GetValueFromInputField();
                for (int i = 0; i < valueList.Count; i++)
                {
                    inputFieldPreviews_type.Add(valueList[i]);
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
                            inputFieldPreviews_type.Add(_valueList[i]);
                        }
                    }
                }
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
        square.GetComponent<AppearSquare>().SetColor(color, orderNum_Type + orderNum_Hold);
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
        planeGroupRot = laneGroup.eulerAngles + new Vector3(0, 0, angleGap);//�ω���̊p�x�����Z�b�g�ʒu�Ƃ��Đݒ�
        //�ω�����������܂ł̎��s����ݒ肷��
        float exe_times = SetExeTimes(waitTime, 20f);
        int originalAngle = (int)laneGroup.transform.eulerAngles.z;
        for (int i = 1; i <= exe_times; i++)
        {
            yield return new WaitForSecondsRealtime(waitTime * fixedActionSpeed / exe_times);
            laneGroup.transform.eulerAngles = new Vector3(0, 0, originalAngle + angleGap * (Mathf.Sin(Mathf.PI * 0.5f * (i / exe_times))));
        }
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
        pjudgeCircleScale = judgeCircle.localScale;//�ω��O�̑傫�������Z�b�g�ʒu�Ƃ��Đݒ�

        //�ω�����������܂ł̎��s����ݒ肷��
        float ex_exe_times = SetExeTimes(waitTime_Ex, 6f);
        float sh_exe_times = SetExeTimes(waitTime_Sh, 6f);

        //Expand
        if (maxScale >= 0)
        {
            Vector3 originalScale = judgeCircle.localScale;
            float gap = maxScale - originalScale.x;
            for (int i = 1; i <= ex_exe_times; i++)
            {
                yield return new WaitForSecondsRealtime(waitTime_Ex * fixedActionSpeed / ex_exe_times);
                judgeCircle.localScale = originalScale + new Vector3(gap * (i / ex_exe_times), gap * (i / ex_exe_times), 0);
            }
        }

        //Shrink
        if (minScale >= 0)
        {
            Vector3 _originalScale = judgeCircle.localScale;
            float _gap = _originalScale.x - minScale;
            for (int i = (int)sh_exe_times - 1; i >= 0; i--)
            {
                yield return new WaitForSecondsRealtime(waitTime_Sh * fixedActionSpeed / sh_exe_times);
                judgeCircle.localScale = new Vector3(minScale, minScale, _originalScale.z) + new Vector3(_gap * (i / sh_exe_times), _gap * (i / sh_exe_times), 0);
            }
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
            Debug.Log($"{gap.x}, {gap.y} =>");
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
            Debug.Log($"{gap.x}, {gap.y}");
        }
        Vector3 pos = laneGroup.transform.position;
        float exe_times = 1f;
        switch (moveType)
        {
            case MovementType.STRAIGHT:
                //�ω�����������܂ł̎��s����ݒ肷��
                exe_times = SetExeTimes(waitTime, 20f);
                for (int i = 1; i <= exe_times; i++)
                {
                    yield return new WaitForSecondsRealtime(waitTime * fixedActionSpeed / exe_times);
                    laneGroup.transform.position = pos + gap * (i / exe_times);
                }
                break;

            case MovementType.QUICK:
                //�ω�����������܂ł̎��s����ݒ肷��
                exe_times = SetExeTimes(waitTime, 20f);
                for (int i = 1; i <= exe_times; i++)
                {
                    yield return new WaitForSecondsRealtime(waitTime * fixedActionSpeed / exe_times);
                    laneGroup.transform.position = pos + gap * (Mathf.Sin(Mathf.PI * 0.5f * (i / exe_times)));
                }
                break;

            case MovementType.SLOW:
                //�ω�����������܂ł̎��s����ݒ肷��
                exe_times = SetExeTimes(waitTime, 20f);
                for (int i = 1; i <= exe_times; i++)
                {
                    yield return new WaitForSecondsRealtime(waitTime * fixedActionSpeed / exe_times);
                    laneGroup.transform.position = pos + gap * (1f - Mathf.Cos(Mathf.PI * 0.5f * (i / exe_times)));
                }
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
        planeGroupPos = laneGroup.position;//�ω��O�̈ʒu�����Z�b�g�ʒu�Ƃ��Đݒ�
        pjudgeCirclePos = judgeCircle.position;//�ω��O�̈ʒu�����Z�b�g�ʒu�Ƃ��Đݒ�

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
        //�Г����̕ω�����������܂ł̎��s����ݒ肷��
        float oneWay_exe_times = SetExeTimes(waitTime, 3f);

        for (int i = 1; i <= oneWay_exe_times; i++)
        {
            yield return new WaitForSecondsRealtime(waitTime / oneWay_exe_times);
            vibeObj.position = pos + amplitude * (i / oneWay_exe_times);
        }
        for (int j = 0; j < 3; j++)
        {
            amplitude -= gap;//�ړ���������������
            //������
            pos = vibeObj.position;
            for (int i = 1; i <= oneWay_exe_times * 2f; i++)
            {
                yield return new WaitForSecondsRealtime(waitTime / oneWay_exe_times);
                vibeObj.position = pos + amplitude * -2 * (i / (oneWay_exe_times * 2f));
            }
            amplitude -= gap;//�ړ���������������
            //�t����
            pos = vibeObj.position;
            for (int i = 1; i <= oneWay_exe_times * 2f; i++)
            {
                yield return new WaitForSecondsRealtime(waitTime / oneWay_exe_times);
                vibeObj.position = pos + amplitude * 2 * (i / (oneWay_exe_times * 2f));
            }
        }
        pos = vibeObj.position;
        for (int i = 1; i <= oneWay_exe_times; i++)
        {
            yield return new WaitForSecondsRealtime(waitTime / oneWay_exe_times);
            vibeObj.position = pos + amplitude * -1 * (i / oneWay_exe_times);
        }
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

        switch (effectMode_Type)
        {
            case EffectModeType.LANE_ROTATE_1:
                if (effectMode_Hold != EffectModeHold.LANE_ROTATE_1)
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
        switch (effectMode_Hold)
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
        //���o�̃X�s�[�h�ɍ��킹�Ĉ�莞�ԑ҂�
        yield return new WaitForSeconds(0.5f * GetFixedActionSpeed());

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
}
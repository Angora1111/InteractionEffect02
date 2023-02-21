using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Notes : MonoBehaviour
{
    const int START_SORTINGORDER = 30000;           // �őO�ʂ̃��C���[�̒l

    protected const float LANE_BOTTUM = -10f;       // �m�[�c�����ł���ʒu
    protected const float JUDGE_POS = 0f;           // ����o�[�̈ʒu
    protected const float LOCAL_TO_GLOBAL = 16f;    // ���[�����local�Ȓ�������global�Ȓ����ɕϊ����邽�߂̒l
    protected const float JUDEGE_RADIUS = 8f;       // ���������ۂ̊�ƂȂ鋗��

    protected GameManager gm;                       // GameManager�N���X
    private Transform laneObj;                      // �Ή����郌�[����transform
    private Transform judgeBarObj;                  // �Ή����郌�[���̔���o�[��transform
    protected int lane = -1;                        // ���[���ԍ�
    protected int kindOfDirection = -1;             // ���o�̎��ʔԍ�
    protected int id = -1;                          // ID�ԍ�
    protected float speed = 0;                      // �ړ����x

    void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void Update()
    {
        if (id != -1)
        {
            // �m�[�c�̔��菈�� ------------------------
            // �Ή�����L�[���������Ƃ�
            if (Input.GetKeyDown(KeyWithLane()))
            {
                CatchProcess();
            }

            // �Ή�����L�[�������Ă����
            if (Input.GetKey(KeyWithLane()))
            {
                HoldProcess();
            }

            // �Ή�����L�[�𗣂����Ƃ�
            if (Input.GetKeyUp(KeyWithLane()))
            {
                ReleaseProcess();
            }

            // �m�[�c�̈ړ����� ------------------------
            // �m�[�c�����ł���ʒu���ゾ������
            if (transform.localPosition.y > LANE_BOTTUM)
            {
                MoveOnLane();
            }
            //�����łȂ����
            else
            {
                MissByNeglect();
            }

            EndOfUpdateProcess();
        }
    }

    /// <summary>
    /// ���[����𓮂��i�h���N���X��D��j
    /// </summary>
    protected virtual void MoveOnLane()
    {
        var plPos = transform.localPosition;
        transform.localPosition = new Vector3(plPos.x - speed * Time.deltaTime, plPos.y, plPos.z);
    }

    /// <summary>
    /// ���ł���
    /// </summary>
    protected void Disappear()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// �m�[�c����蓦�������Ƃɂ�� MISS ����
    /// </summary>
    protected void MissByNeglect()
    {
        Debug.Log($"�ylane:{lane}, id:{id}�z�m�[�c����蓦���܂���...");
        Disappear();
    }

    /// <summary>
    /// ���[���ԍ��ɑΉ�����KeyCode��Ԃ�
    /// </summary>
    /// <returns></returns>
    private KeyCode KeyWithLane()
    {
        return KeyCode.Space;
    }

    /// <summary>
    /// �ł����ɂ���m�[�c����v���Ă��邩�ǂ������ׂ�
    /// </summary>
    /// <param name="argHit2Ds"></param>
    /// <param name="hit2D"></param>
    /// <returns></returns>
    private bool IsMatchClosestNote(ref RaycastHit2D[] argHit2Ds, ref RaycastHit2D hit2D)
    {
        // �ł����[���ɂ����ĉ��ɂ���m�[�c�����o��
        foreach (var _hit2D in argHit2Ds)
        {
            if (hit2D != default)
            {
                if (_hit2D.collider.transform.localPosition.y < hit2D.collider.transform.localPosition.y)
                {
                    hit2D = _hit2D;
                }
            }
            else { hit2D = _hit2D; }
        }
        // ���o�����m�[�c����v���邩�ǂ������m�F����
        if (hit2D == default) { return false; } // �������o����Ȃ����false
        if (hit2D.collider.gameObject.TryGetComponent<Notes>(out var noteData))
        {
            return noteData.GetLane() == lane && noteData.GetId() == id;
        }
        else { return false; }
    }

    /// <summary>
    /// ��{�I�ȃm�[�c�̔��菈��
    /// </summary>
    /// <returns>���茋��</returns>
    protected EnumData.Judgement JudgementProcess()
    {
        // ���ɓ��ꃌ�[���Ŕ��菈�����Ȃ���Ă����画�菈�������Ȃ�
        if (StrikeNoteManager.GetIsStruck(lane)) { return EnumData.Judgement.NONE; }

        float radius = JUDEGE_RADIUS;
        RaycastHit2D[] hit2Ds = Physics2D.CapsuleCastAll(judgeBarObj.position, new Vector2(radius, 1f), CapsuleDirection2D.Horizontal, laneObj.localEulerAngles.z, Vector2.zero);
        RaycastHit2D hit2D = default;
        // �ł����[���ɂ����ĉ��ɂ���m�[�c���������g�ł��邩�ǂ������m�F����
        if (IsMatchClosestNote(ref hit2Ds, ref hit2D))
        {
            // �Y�����[���̃t���O�� true �ɂ���
            StrikeNoteManager.SetIsStruck(lane);

            // �m�[�c�Ɣ���o�[�̋������i�[
            float distance = Mathf.Abs(hit2D.transform.localPosition.y - judgeBarObj.localPosition.y) * LOCAL_TO_GLOBAL * 2;

            // �������画�菈�����s��
            return JudgementByDistance(distance, radius);
        }
        else { return EnumData.Judgement.NONE; }
    }

    /// <summary>
    /// �����Ɗ��n���āA������󂯎��
    /// </summary>
    /// <param name="distance">����Ώۂ̋���</param>
    /// <param name="creteria">�����̋���</param>
    /// <returns>���茋��</returns>
    protected EnumData.Judgement JudgementByDistance(float distance, float creteria)
    {
        if (distance < creteria * 3 / 8)
        {
            // PERFECT
            gm.ShowJudge(EnumData.Judgement.PERFECT);
            JudgeDirection();
            return EnumData.Judgement.PERFECT;
        }
        else if (distance < creteria * 6 / 8)
        {
            // GOOD
            gm.ShowJudge(EnumData.Judgement.GOOD);
            JudgeDirection();
            return EnumData.Judgement.GOOD;
        }
        else
        {
            // MISS
            gm.ShowJudge(EnumData.Judgement.MISS);
            JudgeDirection(false);
            return EnumData.Judgement.MISS;
        }
    }

    /// <summary>
    /// �m�[�c��������Ƃ��̏����i�h���N���X��D��j
    /// </summary>
    protected virtual void CatchProcess() { }

    /// <summary>
    /// �m�[�c���Ƃ葱���Ă���Ƃ��̏����i�h���N���X��D��j
    /// </summary>
    protected virtual void HoldProcess() { }

    /// <summary>
    /// �m�[�c�𗣂����Ƃ��̏����i�h���N���X��D��j
    /// </summary>
    protected virtual void ReleaseProcess() { }

    /// <summary>
    /// �m�[�c��������ۂ̉��o�����s���鏈���i�h���N���X��D��j
    /// </summary>
    protected virtual void JudgeDirection(bool argIsAction = true) { }

    /// <summary>
    /// Update()�֐����̊e�������I��������Ɏ��s����邻�̑��̏����i�h���N���X��D��j
    /// </summary>
    protected virtual void EndOfUpdateProcess() { }

    /// <summary>
    /// ���[���ԍ���ID��ݒ肷��
    /// </summary>
    /// <param name="argLane">�ݒ��̃��[���ԍ�</param>
    /// <param name="argId">�ݒ���ID</param>
    public void SetValues(int argLane, int argId, float argSpeed, int argKindOfDirection = -1)
    {
        lane = argLane;
        id = argId;
        kindOfDirection = argKindOfDirection;
        SetSpeed(argSpeed);
        // ���[���Ɣ���o�[��tarnsform���Z�b�g����
        laneObj = GameObject.Find("LaneGroup").transform.GetChild(lane).transform;
        judgeBarObj = laneObj.GetChild(2).transform;
    }

    /// <summary>
    /// speed���w��̒l�ɐݒ肷��
    /// </summary>
    /// <param name="argSpeed">�ݒ��̑��x</param>
    public void SetSpeed(float argSpeed)
    {
        speed = argSpeed;
    }

    /// <summary>
    /// ���C���[���m�[�c���ōŔw�ʂɂ���
    /// </summary>
    /// <param name="num"></param>
    public void SetLayer(int num)
    {
        var sr = GetComponent<SpriteRenderer>();
        sr.sortingOrder = START_SORTINGORDER - num * 10 - 3;
        int childNum = 0;
        foreach (Transform child in transform)
        {
            childNum++;
            var csr = child.gameObject.GetComponent<SpriteRenderer>();
            csr.sortingOrder = START_SORTINGORDER - num * 10 - (3 - childNum);
        }
    }

    /// <summary>
    /// lane�̃Q�b�^�[
    /// </summary>
    /// <returns></returns>
    public int GetLane() { return lane; }

    /// <summary>
    /// id�̃Q�b�^�[
    /// </summary>
    /// <returns></returns>
    public int GetId() { return id; }
}

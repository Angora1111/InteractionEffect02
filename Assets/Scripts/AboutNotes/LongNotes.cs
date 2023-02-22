using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongNotes : Notes
{
    private const float LTOG_LONGNOTES = 14.9f; // ���[�J�����W���m�[�c�ɂ�����O���[�o�����W�ɕϊ����邽�߂̒l

    [SerializeField] Transform endObj;
    [SerializeField] Transform passObj;
    private bool isHolding = false; // �ێ������ǂ���
    private bool isEndObj = false;  // �I�_�I�u�W�F�N�g����������Ă��邩�ǂ���

    protected override void CatchProcess()
    {
        EnumData.Judgement judgementResult = JudgementProcess();
        // �n�_��MISS������m�[�c���폜����
        if (judgementResult == EnumData.Judgement.MISS)
        {
            Disappear();
        }
        // ����ȊO�̔���Ȃ��ꂽ���Ƃɂ���
        else if (judgementResult != EnumData.Judgement.NONE)
        {
            isHolding = true;
            // �n�_�m�[�c�̈ʒu�𔻒�o�[�ɍ��킹��
            float gap = JUDGE_POS - transform.localPosition.x;
            transform.localPosition = new Vector3(JUDGE_POS, transform.localPosition.y, transform.localPosition.z);
            endObj.localPosition = new Vector3(endObj.localPosition.x - gap / LTOG_LONGNOTES, endObj.localPosition.y, endObj.localPosition.z);
        }
    }

    protected override void ReleaseProcess()
    {
        // �n�_�����Ă����ԂȂ�
        if (isHolding)
        {
            // �n�_�ƏI�_�̋�������A���菈�����s��
            float rangeOfStartAndEnd = Mathf.Abs(passObj.localPosition.x * LTOG_LONGNOTES);
            JudgementByDistance(rangeOfStartAndEnd, JUDEGE_RADIUS);

            Disappear();
        }
    }

    protected override void MoveOnLane()
    {
        base.MoveOnLane();

        // �m�[�c�̗��������𐧌䂷��
        // �n�_���������
        if (isHolding)
        {
            // �n�_���Œ肷��
            transform.localPosition = new Vector3(transform.localPosition.x + speed * Time.deltaTime, transform.localPosition.y, transform.localPosition.z);
            endObj.localPosition = new Vector3(endObj.localPosition.x - speed / LTOG_LONGNOTES * Time.deltaTime, endObj.localPosition.y, endObj.localPosition.z);
        }
        // �I�_�����������O
        if (!isEndObj)
        {
            // �I�_���Œ肷��
            endObj.localPosition = new Vector3(endObj.localPosition.x + speed / LTOG_LONGNOTES * Time.deltaTime, endObj.localPosition.y, endObj.localPosition.z);
        }
    }

    protected override void EndOfUpdateProcess()
    {
        // �p�X�I�u�W�F�N�g�𐧌䂷��
        float rangeOfStartAndEnd = Mathf.Max(endObj.localPosition.x, 0);// ���̒l�ɂ͂Ȃ�Ȃ��悤�ɂ���
        passObj.transform.localPosition = new Vector3(rangeOfStartAndEnd / 2f, passObj.transform.localPosition.y, passObj.transform.localPosition.z);
        passObj.localScale = new Vector3(rangeOfStartAndEnd, passObj.localScale.y, passObj.localScale.z);

        // �I�_���w��̈ʒu�܂ŗ����� MISS �ɂ���
        if (endObj.localPosition.x < (LANE_BOTTUM - JUDGE_POS) / LTOG_LONGNOTES) { MissByNeglect(); }
    }

    protected override void JudgeDirection(bool argIsAction = true)
    {
        gm.HoldAction(argIsAction);
    }

    /// <summary>
    /// �I�_�I�u�W�F�N�g����������Ă��邩�ǂ����̃t���O�� true �ɂ���
    /// </summary>
    public void SetOnIsEndObj()
    {
        isEndObj = true;
    }
}

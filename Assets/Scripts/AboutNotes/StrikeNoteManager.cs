using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrikeNoteManager : MonoBehaviour
{
    const int LANE_NUM = 1;// ���[���̐�

    static bool[] isStruck;// 1�t���[�����ŁA���[�����ƂɃm�[�c���菈�����Ȃ��ꂽ���ǂ���

    void Start()
    {
        isStruck = new bool[LANE_NUM];
        for (int i = 0; i < LANE_NUM; i++)
        {
            isStruck[i] = false;
        }
    }

    void Update()
    {
        // ���t���[���t���O�����Z�b�g����
        for (int i = 0; i < LANE_NUM; i++)
        {
            isStruck[i] = false;
        }
    }

    /// <summary>
    /// 1�t���[�����ŁA���[�����ƂɃm�[�c���菈�����Ȃ��ꂽ���ǂ�����\���t���O�� true �ɂ���
    /// </summary>
    /// <param name="argLaneNum">true �ɂ��郌�[���ԍ�</param>
    public static void SetIsStruck(int argLaneNum)
    {
        int num = argLaneNum;
        // �C���f�b�N�X�͈̔͊O�Ȃ�I������
        if (num < 0 || num > LANE_NUM) { return; }
        // �w�肵�����[����isStruck�� true �ɂ���
        isStruck[num] = true;
    }

    /// <summary>
    /// 1�t���[�����ŁA���[�����ƂɃm�[�c���菈�����Ȃ��ꂽ���ǂ�����\���t���O���擾����
    /// </summary>
    /// <param name="argLaneNum">�擾���郌�[���ԍ�</param>
    /// <returns></returns>
    public static bool GetIsStruck(int argLaneNum)
    {
        int num = argLaneNum;
        // �C���f�b�N�X�͈̔͊O�Ȃ�I������
        if (num < 0 || num > LANE_NUM) { return false; }
        // �w�肵�����[����isStruck�� true �ɂ���
        return isStruck[num];
    }
}

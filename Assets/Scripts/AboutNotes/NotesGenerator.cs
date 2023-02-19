using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotesGenerator : MonoBehaviour
{
    //�ϐ�
    [SerializeField] TypeNotes note;
    [SerializeField] LongNotes longNote;
    [SerializeField] GameObject laneGroup;
    public static float StartPos = 70f;
    private int generateCount = 0;
    private float speed = 1f;
    private Stack<int>[] longIds;

    //���X�g
    public List<int> idList = new List<int>();

    private void Awake()
    {
        idList.Add(-1);
    }

    private void Update()
    {
        // �e�X�g�p
        if (Input.GetKeyDown(KeyCode.J))
        {
            GenerateTypeNote();
        }
        else if (Input.GetKeyDown(KeyCode.F))
        {
            GenerateLongNote_S(0);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            GenerateLongNote_E(0);
        }
    }

    // �m�[�}���m�[�c
    public void GenerateTypeNote()
    {
        var _note = Instantiate(note, transform);
        _note.transform.SetParent(laneGroup.transform);
        _note.transform.localPosition = new Vector3(StartPos, 0, 0);
        _note.SetValues(0, generateCount, speed);
        _note.SetLayer(generateCount);

        generateCount++;
    }

    // �����O�m�[�c�n�_
    public void GenerateLongNote_S(int laneNum)
    {
        var _note = Instantiate(longNote, transform);
        _note.transform.SetParent(laneGroup.transform);
        _note.transform.localPosition = new Vector3(StartPos, 0, 0);
        _note.GetComponent<Notes>().SetValues(laneNum, generateCount, speed);
        _note.SetLayer(generateCount);
        longIds[0].Push(generateCount);

        generateCount++;
    }

    // �����O�m�[�c�I�_
    public void GenerateLongNote_E(int laneNum)
    {
        // �Ō�ɕۑ�����ID�����o��
        int startId = longIds[laneNum].Pop();
        foreach (Transform child in laneGroup.transform)
        {
            // �����O�m�[�c�����邩�ǂ����m�F����
            if (child.gameObject.TryGetComponent<LongNotes>(out var ln))
            {
                // �����O�m�[�c�̂����A�Ō�ɕۑ�����ID�Ɠ������̂����邩�ǂ����m�F����
                if (ln.GetId() == startId)
                {
                    // ����΁A�I�_�I�u�W�F�N�g����������Ă��邩�ǂ����̃t���O�� true �ɂ���
                    ln.SetOnIsEndObj();
                    return;
                }
            }
        }
    }
}

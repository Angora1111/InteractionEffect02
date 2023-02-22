using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class NotesGenerator : MonoBehaviour
{
    private const int MAX_LANE_NUM = 10;
    //�ϐ�
    [SerializeField] TypeNotes note;
    [SerializeField] LongNotes longNote;
    [SerializeField] GameObject laneGroup;
    public static float StartPos = 70f;
    private int generateCount = 0;
    private float speed = 30f;
    private Stack<int>[] longIds;

    private string dataPath;        // ���ʃf�[�^�̃t�@�C���p�X�̂����A�Œ�̕���
    private string notesMapPath;    // ���ʃf�[�^�̃t�@�C���p�X�̂����A�ς�镔��
    private NotesMapData notesMap;  // ���ʃf�[�^���i�[����N���X

    // ���ʃf�[�^�̃N���X
    [System.Serializable]
    public class NotesMapData
    {
        public List<NoteData> notesData;

        public NotesMapData()
        {
            notesData = new List<NoteData>();
        }
    }

    [System.Serializable]
    public class NoteData
    {
        public float startTime;
        public float endTime;
        public int type;
        public float speed;

        public NoteData(float startTime, float endTime, int type, float speed)
        {
            this.startTime = startTime;
            this.endTime = endTime;
            this.type = type;
            this.speed = speed;
        }
    }

    private void Awake()
    {
        longIds = new Stack<int>[MAX_LANE_NUM];
        for(int i = 0; i < MAX_LANE_NUM; i++)
        {
            longIds[i] = new Stack<int>();
        }

        dataPath = Application.dataPath + "/NotesMaps";
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
        var _note = Instantiate(note.gameObject, transform);
        _note.transform.SetParent(laneGroup.transform.GetChild(0).transform);
        _note.transform.localPosition = new Vector3(StartPos, 0, 0);
        _note.GetComponent<Notes>().SetValues(0, generateCount, speed);
        _note.GetComponent<Notes>().SetLayer(generateCount);

        generateCount++;
    }

    // �����O�m�[�c�n�_
    public void GenerateLongNote_S(int laneNum)
    {
        var _note = Instantiate(longNote, transform);
        _note.transform.SetParent(laneGroup.transform.GetChild(0).transform);
        _note.transform.localPosition = new Vector3(StartPos, 0, 0);
        _note.GetComponent<Notes>().SetValues(laneNum, generateCount, speed);
        _note.GetComponent<Notes>().SetLayer(generateCount);
        longIds[0].Push(generateCount);

        generateCount++;
    }

    // �����O�m�[�c�I�_
    public void GenerateLongNote_E(int laneNum)
    {
        // �Ō�ɕۑ�����ID�����o��
        int startId = longIds[laneNum].Pop();
        foreach (Transform child in laneGroup.transform.GetChild(0).transform)
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

    /// <summary>
    /// �Q�[�����J�n����
    /// </summary>
    public void StartGame()
    {
        // json�t�@�C����ǂݍ���
        notesMap = LoadNotesMapData(dataPath + notesMapPath);

        // �ǂݍ��񂾃f�[�^�ɏ]���ăm�[�c�𐶐�����
    }

    /// <summary>
    /// ���ʃf�[�^�̃p�X��ݒ肷��
    /// </summary>
    /// <param name="argNum">���ʔԍ�</param>
    public void SetNotesMapPath(int argNum)
    {
        switch (argNum)
        {
            case 0:
                notesMapPath = "/ienm01_IE_1.json";
                break;
            case 1:
                notesMapPath = "/ienm01_IE_2.json";
                break;
            default:
                break;
        }
    }

    public NotesMapData LoadNotesMapData(string argDataPath)
    {
        using (StreamReader reader = new StreamReader(argDataPath))
        {
            string datastr = reader.ReadToEnd();
            reader.Close();

            return JsonUtility.FromJson<NotesMapData>(datastr);
        }
    }
}

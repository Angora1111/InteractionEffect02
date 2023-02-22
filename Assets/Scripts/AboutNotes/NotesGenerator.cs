using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class NotesGenerator : MonoBehaviour
{
    private const int MAX_LANE_NUM = 10;
    private readonly Vector3 poolPos = new Vector3(-1000, -1000, 0);
    //�ϐ�
    [SerializeField] TypeNotes note;
    [SerializeField] LongNotes longNote;
    [SerializeField] GameObject laneGroup;
    [SerializeField] Transform notesGroup;
    public static float StartPos = 70f;
    private int generateCount = 0;
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
            GenerateTypeNote(0, 0, 2);
        }
        else if (Input.GetKeyDown(KeyCode.F))
        {
            GenerateLongNote(0, 10, 0, 2);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            GenerateLongNote_E(0);
        }
    }

    // �m�[�}���m�[�c
    public void GenerateTypeNote(float argStartTime, int argLane, float argSpeed)
    {
        var _note = Instantiate(note.gameObject, transform);
        _note.transform.SetParent(notesGroup);
        _note.transform.localPosition = poolPos;
        _note.GetComponent<Notes>().SetValues(argStartTime, -1, argLane, generateCount, argSpeed * 15f);
        _note.GetComponent<Notes>().SetLayer(generateCount);

        generateCount++;
    }

    // �����O�m�[�c
    public void GenerateLongNote(float argStartTime, float argEndTime, int argLane, float argSpeed)
    {
        var _note = Instantiate(longNote, transform);
        _note.transform.SetParent(notesGroup);
        _note.transform.localPosition = poolPos;
        _note.GetComponent<Notes>().SetValues(argStartTime, argEndTime, argLane, generateCount, argSpeed * 15f);
        _note.GetComponent<Notes>().SetLayer(generateCount);
        longIds[0].Push(generateCount);

        generateCount++;
    }

    // �����I�Ƀ����O�m�[�c�I�_���o��������
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
        string fileName = dataPath + notesMapPath;
        notesMap = LoadNotesMapData(fileName);

        // �ǂݍ��񂾃f�[�^�ɏ]���ăm�[�c�𐶐�����
        for(int i = 0; i < notesMap.notesData.Count; i++)
        {
            var nd = notesMap.notesData[i];
            switch (nd.type)
            {
                case 1:
                    GenerateTypeNote(nd.startTime, 0, nd.speed);
                    break;
                case 2:
                    GenerateLongNote(nd.startTime, nd.endTime, 0, nd.speed);
                    break;
                default:
                    Debug.Log($"�y{i}�z�m�[�c�ȊO�̂��̂�����܂���");
                    break;
            }
        }
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

    private NotesMapData LoadNotesMapData(string argDataPath)
    {
        using (StreamReader reader = new StreamReader(argDataPath))
        {
            string datastr = reader.ReadToEnd();
            reader.Close();

            return JsonUtility.FromJson<NotesMapData>(datastr);
        }
    }
}

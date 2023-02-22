using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class NotesGenerator : MonoBehaviour
{
    private const int MAX_LANE_NUM = 10;
    private readonly Vector3 poolPos = new Vector3(-1000, -1000, 0);
    //変数
    [SerializeField] TypeNotes note;
    [SerializeField] LongNotes longNote;
    [SerializeField] GameObject laneGroup;
    [SerializeField] Transform notesGroup;
    public static float StartPos = 70f;
    private int generateCount = 0;
    private Stack<int>[] longIds;

    private string dataPath;        // 譜面データのファイルパスのうち、固定の部分
    private string notesMapPath;    // 譜面データのファイルパスのうち、変わる部分
    private NotesMapData notesMap;  // 譜面データを格納するクラス

    // 譜面データのクラス
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
        // テスト用
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

    // ノーマルノーツ
    public void GenerateTypeNote(float argStartTime, int argLane, float argSpeed)
    {
        var _note = Instantiate(note.gameObject, transform);
        _note.transform.SetParent(notesGroup);
        _note.transform.localPosition = poolPos;
        _note.GetComponent<Notes>().SetValues(argStartTime, -1, argLane, generateCount, argSpeed * 15f);
        _note.GetComponent<Notes>().SetLayer(generateCount);

        generateCount++;
    }

    // ロングノーツ
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

    // 強制的にロングノーツ終点を出現させる
    public void GenerateLongNote_E(int laneNum)
    {
        // 最後に保存したIDを取り出す
        int startId = longIds[laneNum].Pop();
        foreach (Transform child in laneGroup.transform.GetChild(0).transform)
        {
            // ロングノーツがあるかどうか確認する
            if (child.gameObject.TryGetComponent<LongNotes>(out var ln))
            {
                // ロングノーツのうち、最後に保存したIDと同じものがあるかどうか確認する
                if (ln.GetId() == startId)
                {
                    // あれば、終点オブジェクトが生成されているかどうかのフラグを true にする
                    ln.SetOnIsEndObj();
                    return;
                }
            }
        }
    }

    /// <summary>
    /// ゲームを開始する
    /// </summary>
    public void StartGame()
    {
        // jsonファイルを読み込む
        string fileName = dataPath + notesMapPath;
        notesMap = LoadNotesMapData(fileName);

        // 読み込んだデータに従ってノーツを生成する
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
                    Debug.Log($"【{i}】ノーツ以外のものがありました");
                    break;
            }
        }
    }

    /// <summary>
    /// 譜面データのパスを設定する
    /// </summary>
    /// <param name="argNum">譜面番号</param>
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

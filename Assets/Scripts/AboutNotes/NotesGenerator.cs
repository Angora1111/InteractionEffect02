using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotesGenerator : MonoBehaviour
{
    private const int MAX_LANE_NUM = 10;
    //変数
    [SerializeField] TypeNotes note;
    [SerializeField] LongNotes longNote;
    [SerializeField] GameObject laneGroup;
    public static float StartPos = 70f;
    private int generateCount = 0;
    private float speed = 30f;
    private Stack<int>[] longIds;

    private void Awake()
    {
        longIds = new Stack<int>[MAX_LANE_NUM];
        for(int i = 0; i < MAX_LANE_NUM; i++)
        {
            longIds[i] = new Stack<int>();
        }
    }

    private void Update()
    {
        // テスト用
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

    // ノーマルノーツ
    public void GenerateTypeNote()
    {
        var _note = Instantiate(note.gameObject, transform);
        _note.transform.SetParent(laneGroup.transform.GetChild(0).transform);
        _note.transform.localPosition = new Vector3(StartPos, 0, 0);
        _note.GetComponent<Notes>().SetValues(0, generateCount, speed);
        _note.GetComponent<Notes>().SetLayer(generateCount);

        generateCount++;
    }

    // ロングノーツ始点
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

    // ロングノーツ終点
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
}

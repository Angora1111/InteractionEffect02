using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotesGenerator : MonoBehaviour
{
    //変数
    [SerializeField] TypeNotes note;
    [SerializeField] LongNotes longNote;
    [SerializeField] GameObject laneGroup;
    public static float StartPos = 70f;
    private int generateCount = 0;
    private float speed = 1f;
    private Stack<int>[] longIds;

    //リスト
    public List<int> idList = new List<int>();

    private void Awake()
    {
        idList.Add(-1);
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
        var _note = Instantiate(note, transform);
        _note.transform.SetParent(laneGroup.transform);
        _note.transform.localPosition = new Vector3(StartPos, 0, 0);
        _note.SetValues(0, generateCount, speed);
        _note.SetLayer(generateCount);

        generateCount++;
    }

    // ロングノーツ始点
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

    // ロングノーツ終点
    public void GenerateLongNote_E(int laneNum)
    {
        // 最後に保存したIDを取り出す
        int startId = longIds[laneNum].Pop();
        foreach (Transform child in laneGroup.transform)
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

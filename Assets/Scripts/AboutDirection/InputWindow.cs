using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using Newtonsoft.Json.Linq;
using System;

public class InputWindow : MonoBehaviour
{
    protected GameManager gm;                     // GameManager
    [SerializeField] protected Transform modeButton;      // 対応する演出のボタン    
    [SerializeField] protected RectTransform backWindow;  // ウィンドウのtransform
    [SerializeField] protected GameObject inputFieldObj;  // インプットフィールドのプレハブ
    [Header("インプットフィールドの個数を指定します（1～8）")]

    [SerializeField] protected int objNum = 1;            // インプットフィールドの数
    [HideInInspector]
    [SerializeField] protected int pobjNum = 1;
    protected List<GameObject> destroyedObjects;
    /// <summary>
    /// Inputfieldの数
    /// </summary>
    public int inputFieldCount { get { return objNum; } }

    #region 実行外で設定するための部分
    private void OnValidate()
    {
        if (objNum != 0)// 値を消したときは反応しないようにする
        {
            // 値を補正する
            if (objNum > 8) objNum = 8;
            if (objNum < 1) objNum = 1;

            // 変化した値を求める
            int objNumDifference = objNum - pobjNum;
            pobjNum = objNum;

            // 変化した値に応じてウィンドウの大きさを変更する
            float windowHeight = 40 + 110 * objNum;
            backWindow.sizeDelta = new Vector2(backWindow.sizeDelta.x, windowHeight);

            // 変化した値に応じてインプットフィールドを増加・減少させる
            if (objNumDifference > 0)     // 増加
            {
                for (int i = 0; i < objNumDifference; i++)
                {
                    Instantiate(inputFieldObj, transform);
                }
            }
            else if (objNumDifference < 0)// 減少
            {
                int childNum = transform.childCount;
                destroyedObjects.Clear();
                for (int i = 0; i < -objNumDifference; i++)
                {
                    // 下の方から削除する
                    destroyedObjects.Add(transform.GetChild(childNum - (i + 1)).gameObject);
                }
                EditorApplication.delayCall += DestroyInEditor;
            }

            // インプットフィールドの位置を調整する
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i).GetComponent<RectTransform>();
                float childPosY = windowHeight / 2 - 75 - 110 * i;
                child.localPosition = new Vector3(215f, childPosY, 0f);
            }
        }
    }

    protected void DestroyInEditor()
    {
        EditorApplication.delayCall -= DestroyInEditor;
        if (destroyedObjects != null)
        {
            int listCount = destroyedObjects.Count;
            for (int i = 0; i < listCount; i++)
            {
                DestroyImmediate(destroyedObjects[i]);
            }
        }
    }
    #endregion

    private void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void OnEnable()
    {
        //gm.SetPriviousFloatFromInputField(modeButton);
    }

    private void Update()
    {
        //gm.SetFloatFromInputField(modeButton);
    }

    /// <summary>
    /// InputField内の値をまとめて返す
    /// </summary>
    /// <returns></returns>
    public List<float> GetValueFromInputField()
    {
        var values = new List<float>();
        foreach (Transform child in transform)
        {
            if (child.gameObject.TryGetComponent<InputField>(out var inputField))
            {
                if (float.TryParse(inputField.text, out float value))
                {
                    values.Add(value);
                }
            }
        }
        return values;
    }
    /// <summary>
    /// InputFieldの値をセットする
    /// </summary>
    /// <param name="argFloatList"></param>
    public void SetValue(float argVal, int childNum)
    {
        int _index = 0;
        foreach (Transform child in transform)
        {
            if (child.gameObject.TryGetComponent<InputField>(out var inputField))
            {
                if (_index == childNum)
                {
                    inputField.text = argVal.ToString();
                    return;
                }
                _index++;
            }
        }
        Debug.LogError($"指定した番号の Inputfield はありません : {childNum}");
    }
}

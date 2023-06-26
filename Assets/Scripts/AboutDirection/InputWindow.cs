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
    [SerializeField] protected Transform modeButton;      // �Ή����鉉�o�̃{�^��    
    [SerializeField] protected RectTransform backWindow;  // �E�B���h�E��transform
    [SerializeField] protected GameObject inputFieldObj;  // �C���v�b�g�t�B�[���h�̃v���n�u
    [Header("�C���v�b�g�t�B�[���h�̌����w�肵�܂��i1�`8�j")]

    [SerializeField] protected int objNum = 1;            // �C���v�b�g�t�B�[���h�̐�
    [HideInInspector]
    [SerializeField] protected int pobjNum = 1;
    protected List<GameObject> destroyedObjects;
    /// <summary>
    /// Inputfield�̐�
    /// </summary>
    public int inputFieldCount { get { return objNum; } }

    #region ���s�O�Őݒ肷�邽�߂̕���
    private void OnValidate()
    {
        if (objNum != 0)// �l���������Ƃ��͔������Ȃ��悤�ɂ���
        {
            // �l��␳����
            if (objNum > 8) objNum = 8;
            if (objNum < 1) objNum = 1;

            // �ω������l�����߂�
            int objNumDifference = objNum - pobjNum;
            pobjNum = objNum;

            // �ω������l�ɉ����ăE�B���h�E�̑傫����ύX����
            float windowHeight = 40 + 110 * objNum;
            backWindow.sizeDelta = new Vector2(backWindow.sizeDelta.x, windowHeight);

            // �ω������l�ɉ����ăC���v�b�g�t�B�[���h�𑝉��E����������
            if (objNumDifference > 0)     // ����
            {
                for (int i = 0; i < objNumDifference; i++)
                {
                    Instantiate(inputFieldObj, transform);
                }
            }
            else if (objNumDifference < 0)// ����
            {
                int childNum = transform.childCount;
                destroyedObjects.Clear();
                for (int i = 0; i < -objNumDifference; i++)
                {
                    // ���̕�����폜����
                    destroyedObjects.Add(transform.GetChild(childNum - (i + 1)).gameObject);
                }
                EditorApplication.delayCall += DestroyInEditor;
            }

            // �C���v�b�g�t�B�[���h�̈ʒu�𒲐�����
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
    /// InputField���̒l���܂Ƃ߂ĕԂ�
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
    /// InputField�̒l���Z�b�g����
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
        Debug.LogError($"�w�肵���ԍ��� Inputfield �͂���܂��� : {childNum}");
    }
}

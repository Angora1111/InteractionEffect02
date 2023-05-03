using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class InputWindow : MonoBehaviour
{
    private GameManager gm;                     // GameManager
    [SerializeField] Transform modeButton;      // �Ή����鉉�o�̃{�^��    
    [SerializeField] RectTransform backWindow;  // �E�B���h�E��transform
    [SerializeField] GameObject inputFieldObj;  // �C���v�b�g�t�B�[���h�̃v���n�u
    [Header("�C���v�b�g�t�B�[���h�̌����w�肵�܂��i1�`8�j")]

    [SerializeField] int objNum = 1;            // �C���v�b�g�t�B�[���h�̐�
    [HideInInspector]
    [SerializeField] int pobjNum = 1;
    List<GameObject> destroyedObjects;

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

    private void DestroyInEditor()
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

    private void Update()
    {
        gm.SetFloatFromInputField(modeButton);
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
}

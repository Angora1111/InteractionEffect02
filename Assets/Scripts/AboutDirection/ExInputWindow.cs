using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ExInputWindow : InputWindow
{
    private const int TOGGLE_HEIGHT = 120;

    [SerializeField] Toggle isEdittingToggle;
    [SerializeField] GameObject backBlackObj;

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
            float windowHeight = 40 + 110 * objNum + TOGGLE_HEIGHT;
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
            for (int i = 1; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i).GetComponent<RectTransform>();
                float childPosY = windowHeight / 2 - 75 - 110 * i;
                child.localPosition = new Vector3(215f, childPosY, 0f);
            }
        }
    }

    private void Update()
    {
        ChangeInputInteractable();
        gm.SetFloatFromInputField(modeButton);
    }

    private void ChangeInputInteractable()
    {
        gm.SetIsSetting(isEdittingToggle.isOn);

        foreach(Transform child in transform)
        {
            if(child.gameObject.TryGetComponent<InputField>(out var inf))
            {
                inf.interactable = isEdittingToggle.isOn;
            }
        }

        backBlackObj.SetActive(isEdittingToggle.isOn);
    }
}

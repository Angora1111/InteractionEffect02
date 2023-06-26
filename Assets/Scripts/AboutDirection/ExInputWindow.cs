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
        if (objNum != 0)// 値を消したときは反応しないようにする
        {
            // 値を補正する
            if (objNum > 8) objNum = 8;
            if (objNum < 1) objNum = 1;

            // 変化した値を求める
            int objNumDifference = objNum - pobjNum;
            pobjNum = objNum;

            // 変化した値に応じてウィンドウの大きさを変更する
            float windowHeight = 40 + 110 * objNum + TOGGLE_HEIGHT;
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VariousPreview : MonoBehaviour
{
    [SerializeField] GameObject[] previewObjs;
    private GameManager gm;
    private Button modeButton;
    private ModeButton modeButtonData;

    private void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        modeButton = GetComponent<Button>();
        modeButtonData = GetComponent<ModeButton>();
    }

    private void Update()
    {
        // ボタンが選択されていないとき
        if (modeButton.interactable)
        {
            // 非表示
            if(previewObjs != null)
            {
                foreach(var obj in previewObjs)
                {
                    obj.SetActive(false);
                }
            }
        }
        // ボタンが選択されているとき
        else
        {
            // 表示
            if (previewObjs != null && GameManager.showingSettingPage == modeButtonData.Getmode())
            {
                foreach (var obj in previewObjs)
                {
                    obj.SetActive(true);
                }

                //GameManagerに設定する
                gm.SetColors(transform);
                gm.SetBools(transform);
                gm.SetFloatFromInputField(transform);
            }
            else
            {
                // 非表示
                if (previewObjs != null)
                {
                    foreach (var obj in previewObjs)
                    {
                        obj.SetActive(false);
                    }
                }
            }
        }
    }
}

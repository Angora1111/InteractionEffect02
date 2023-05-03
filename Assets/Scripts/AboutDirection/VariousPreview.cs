using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VariousPreview : MonoBehaviour
{
    [SerializeField] GameObject[] previewObjs;
    private GameManager gm;
    private Button modeButton;

    private void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        modeButton = GetComponent<Button>();
    }

    private void Update()
    {
        // �{�^�����I������Ă��Ȃ��Ƃ�
        if (modeButton.interactable)
        {
            // ��\��
            if(previewObjs != null)
            {
                foreach(var obj in previewObjs)
                {
                    obj.SetActive(false);
                }
            }
        }
        // �{�^�����I������Ă���Ƃ�
        else
        {
            // �\��
            if (previewObjs != null)
            {
                foreach (var obj in previewObjs)
                {
                    obj.SetActive(true);
                }

                //GameManager�ɐݒ肷��
                gm.SetColors(transform);
                gm.SetBools(transform);
                gm.SetFloatFromInputField(transform);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropdownWindow : MonoBehaviour
{
    [SerializeField] Transform modeButton;
    [SerializeField] Dropdown dropdown;
    [Tooltip("����̒l�ɂȂ����Ƃ��ɕ\������I�u�W�F�N�g")]
    [SerializeField] GameObject[] showObjects;
    [Tooltip("����̒l")]
    [SerializeField] int specificValue = -1;
    [Tooltip("����̒l�ɂȂ����Ƃ��ɃC���v�b�g�t�B�[���h�̒l���Z�b�g���邩�ǂ���")]
    [SerializeField] bool setInputField = false;
    private int value;
    private GameManager gm;
    private int pEffectIndex = 0;

    private void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    void Update()
    {
        if(GameManager.selectingEffectIndex != pEffectIndex)
        {
            gm.SetPreviousIntFromDropdown(modeButton);
            pEffectIndex = GameManager.selectingEffectIndex;
        }

        value = dropdown.value;
        ChangeInSpecificValue(specificValue);
        gm.SetIntFromDropdown(modeButton);
    }

    public int GetValueFromDropdown()
    {
        return value;
    }

    public void SetValue(int argVal)
    {
        dropdown.value = argVal;
    }

    private void ChangeInSpecificValue(int valueNum)
    {
        if (showObjects != null)
        {
            if (value == valueNum)
            {
                foreach (GameObject showObj in showObjects)
                {
                    showObj.SetActive(true);
                    if (setInputField) { gm.SetFloatFromInputField(modeButton); }
                }
            }
            else
            {
                foreach (GameObject showObj in showObjects)
                {
                    showObj.SetActive(false);
                }
            }
        }
    }
}

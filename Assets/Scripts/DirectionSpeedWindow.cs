using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DirectionSpeedWindow : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] InputField inputField;
    private float pValue;//�ύX�O�̃X���C�_�[�̒l
    private GameManager gm;

    private void Awake()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();

        //�ŏ��͂P�{��
        slider.value = 0f;
        pValue = 0f;
        SliderValueToInputField();
    }

    /// <summary>
    /// �X���C�_�[�̒l����͘g�ɔ��f������
    /// </summary>
    public void SliderValueToInputField()
    {
        float fixedValue = slider.value < 0 ? 10f / (10f - slider.value) : (10f + slider.value) / 10f;
        pValue = slider.value;
        inputField.text = slider.value < 0 ? "1 / " + ((10f - slider.value) / 10f).ToString() : ((10f + slider.value) / 10f).ToString();
        gm.SetActionSpeed(fixedValue);
    }

    /// <summary>
    /// ���͘g�̒l���X���C�_�[�ɔ��f������
    /// </summary>
    public void InputFieldValueToSlider()
    {
        //���l�ϊ��ł���΁i1�{���ȏ�ł���΁j
        if (float.TryParse(inputField.text, out float value))
        {
            value = value * 10f - 10f;
            value = Mathf.FloorToInt(value);//�K�������_���ʂ܂łɂ���
            if (value < 0)
            {
                value = 0;
            }
            if (value > slider.maxValue)
            {
                value = slider.maxValue;
            }
            pValue = value;
            inputField.text = ((10f + value) / 10f).ToString();
            slider.value = value;
            gm.SetActionSpeed((10f + value) / 10f);
        }
        else//���l�ϊ��ł��Ȃ����
        {
            //����Z�̌`�ł��邩�ǂ����m�F����
            int divisionType = 0;
            int divisionPos = -1;
            string inputText = inputField.text;
            for (int i = 0; i < inputText.Length; i++)
            {
                //1���������Ă���
                string character = inputText.Substring(i, 1);
                //�u1 /�v�̌`�����邩�ǂ����m�F���A����΂��̈ʒu���L�^����
                if (divisionType == 0)
                {
                    if (character == "1" || character == "1")
                    {
                        divisionType = 1;
                    }
                    else if (!(character == " ") && !(character == "�@")) { break; }
                }
                else if (divisionType == 1)
                {
                    if (character == "/" || character == "�^")
                    {
                        divisionPos = i + 1;
                        break;
                    }
                    else if (!(character == " ") && !(character == "�@")) { break; }
                }
            }
            //����Z�̌`�ł����
            if (divisionPos != -1)
            {
                //����ɂ����镔�������o��
                string numText = inputText.Substring(divisionPos, inputText.Length - divisionPos);
                //���o���������������ɕϊ��ł���Ȃ�
                if (float.TryParse(numText, out float divisionValue))
                {
                    divisionValue = divisionValue * 10f - 10f;
                    divisionValue = Mathf.FloorToInt(divisionValue);
                    if (divisionValue < 1)//�ŏ��l��1.1�ɂ���
                    {
                        divisionValue = 1f;
                    }
                    else if (divisionValue > slider.maxValue)
                    {
                        divisionValue = slider.maxValue;
                    }
                    pValue = -divisionValue;
                    inputField.text = "1 / " + ((divisionValue + 10f) / 10f).ToString();
                    slider.value = -divisionValue;
                    gm.SetActionSpeed(10f / (divisionValue + 10f));
                }
                else//���ꂪ�����łȂ����1�O�̒l�ɖ߂�
                {
                    slider.value = pValue;
                    SliderValueToInputField();
                }
            }
            else//����Z�̌`�łȂ����1�O�̒l�ɖ߂�
            {
                slider.value = pValue;
                SliderValueToInputField();
            }
        }
    }
}

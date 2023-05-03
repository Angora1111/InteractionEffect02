using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DecideDistWindow : MonoBehaviour
{
    private const float SLIDER_MAX = 750.0f;
    private const float DISAPPEAR_START = 10.00f;

    // �e�C���v�b�g�t�B�[���h�́A���O�̒l
    private float pValue_perfect = -1f;
    private float pValue_good = -1f;
    private float pValue_miss = -1f;
    private float pValue_disappear = -1f;
    // ���O�̑S�̂̒l
    private float pValue_c_perfect = 0f;
    private float pValue_c_good = 0f;
    private float pValue_c_miss = 0f;
    private float pValue_c_neglect = 0f;

    [Header("�m�u")]
    [SerializeField] DistBarNob nob_perfect;
    [SerializeField] DistBarNob nob_good, nob_miss;
    [Header("���ňʒu�̃e�L�X�g")]
    [SerializeField] Text label_disappear;
    [Header("�C���v�b�g�t�B�[���h")]
    [SerializeField] InputField inputField_perfect;
    [SerializeField] InputField inputField_good, inputField_miss, inputField_disappear;
    [Header("�K�C�h���C��")]
    [SerializeField] DrawJudgeGuideline guideline;

    void Start()
    {
        InputFieldIntoSlider(3);

        SliderIntoInputField(0);
        SliderIntoInputField(1);
        SliderIntoInputField(2);
    }

    void Update()
    {
        // �C���v�b�g�t�B�[���h���S�ē��͂���Ă����
        if (float.TryParse(inputField_perfect.text, out var value))
        {
            if (float.TryParse(inputField_good.text, out var _value))
            {
                if (float.TryParse(inputField_miss.text, out var __value))
                {
                    if (float.TryParse(label_disappear.text, out var ___value))
                    {
                        // �l���ύX����Ă���Δ��f����
                        if (pValue_c_perfect != value
                           || pValue_c_good != _value
                           || pValue_c_miss != __value
                           || pValue_c_neglect != ___value)
                        {
                            // �S�̂̒l�ɐݒ肷��
                            CommonData.perfectDist = value;
                            CommonData.goodDist = _value;
                            CommonData.missDist = __value;
                            CommonData.neglectDist = ___value;

                            // �K�C�h���C���ɔ��f����
                            guideline.Draw();

                            // �l��ۑ�����
                            pValue_c_perfect = CommonData.perfectDist;
                            pValue_c_good = CommonData.goodDist;
                            pValue_c_miss = CommonData.missDist;
                            pValue_c_neglect = CommonData.neglectDist;
                        }
                    }
                }
            }
        }
    }

    // �X���C�_�[�̒l���C���v�b�g�t�B�[���h�ɔ��f����
    public void SliderIntoInputField(int trigger)
    {
        float max = 0.0f;
        float value = 0.0f;
        switch (trigger)
        {
            // PERFECT
            case 0:
                max = float.Parse(label_disappear.text);
                value = max * (nob_perfect.rectTransform.anchoredPosition.x / SLIDER_MAX);
                value = Truncate(value, 2);
                inputField_perfect.text = value.ToString();

                // �l���L�^
                pValue_perfect = value;
                break;

            // GOOD
            case 1:
                max = float.Parse(label_disappear.text);
                value = max * (nob_good.rectTransform.anchoredPosition.x / SLIDER_MAX);
                value = Truncate(value, 2);
                inputField_good.text = value.ToString();

                // �l���L�^
                pValue_good = value;
                break;

            // MISS
            case 2:
                max = float.Parse(label_disappear.text);
                value = max * (nob_miss.rectTransform.anchoredPosition.x / SLIDER_MAX);
                value = Truncate(value, 2);
                inputField_miss.text = value.ToString();

                // �l���L�^
                pValue_miss = value;
                break;
        }
    }

    // �C���v�b�g�t�B�[���h�̒l���X���C�_�[�ɔ��f����
    public void InputFieldIntoSlider(int trigger)
    {
        float max = 0.0f;
        float value = 0.0f;
        switch (trigger)
        {
            // PERFECT
            case 0:
                // �C���v�b�g�t�B�[���h�ɒl�������
                if (float.TryParse(inputField_perfect.text, out var perfectValue))
                {
                    // �X���C�_�[�ɔ��f����
                    max = float.Parse(label_disappear.text);
                    value = Truncate(perfectValue, 2);
                    nob_perfect.rectTransform.anchoredPosition = new Vector2(SLIDER_MAX * (value / max), nob_perfect.rectTransform.anchoredPosition.y);

                    // �l���L�^
                    pValue_perfect = value;

                    // �m�u�̍ŏ��l/�ő�l�ȓ��Ɏ��܂�悤�ɕ␳����
                    if (nob_perfect.FixCursorPos())
                    {
                        // �␳���ꂽ�Ȃ�A���߂Ēl���L�^
                        value = max * (nob_perfect.rectTransform.anchoredPosition.x / SLIDER_MAX);
                        value = Truncate(value, 2);
                        pValue_perfect = value;
                        // �C���v�b�g�t�B�[���h�ɔ��f
                        inputField_perfect.text = value.ToString();
                    }
                }
                // �l���Ȃ����
                else
                {
                    // ���O�̒l��ݒ肷��
                    inputField_perfect.text = Truncate(pValue_perfect, 2).ToString();
                }
                break;

            // GOOD
            case 1:
                // �C���v�b�g�t�B�[���h�ɒl�������
                if (float.TryParse(inputField_good.text, out var goodValue))
                {
                    // �X���C�_�[�ɔ��f����
                    max = float.Parse(label_disappear.text);
                    value = Truncate(goodValue, 2);
                    nob_good.rectTransform.anchoredPosition = new Vector2(SLIDER_MAX * (value / max), nob_good.rectTransform.anchoredPosition.y);

                    // �l���L�^
                    pValue_good = value;

                    // �m�u�̍ŏ��l/�ő�l�ȓ��Ɏ��܂�悤�ɕ␳����
                    if (nob_good.FixCursorPos())
                    {
                        // �␳���ꂽ�Ȃ�A���߂Ēl���L�^
                        value = max * (nob_good.rectTransform.anchoredPosition.x / SLIDER_MAX);
                        value = Truncate(value, 2);
                        pValue_good = value;
                        // �C���v�b�g�t�B�[���h�ɔ��f
                        inputField_good.text = value.ToString();
                    }
                }
                // �l���Ȃ����
                else
                {
                    // ���O�̒l��ݒ肷��
                    inputField_good.text = Truncate(pValue_good, 2).ToString();
                }
                break;

            // MISS
            case 2:
                // �C���v�b�g�t�B�[���h�ɒl�������
                if (float.TryParse(inputField_miss.text, out var missValue))
                {
                    // �X���C�_�[�ɔ��f����
                    max = float.Parse(label_disappear.text);
                    value = Truncate(missValue, 2);
                    nob_miss.rectTransform.anchoredPosition = new Vector2(SLIDER_MAX * (value / max), nob_miss.rectTransform.anchoredPosition.y);

                    // �l���L�^
                    pValue_miss = value;

                    // �m�u�̍ŏ��l/�ő�l�ȓ��Ɏ��܂�悤�ɕ␳����
                    if (nob_miss.FixCursorPos())
                    {
                        // �␳���ꂽ�Ȃ�A���߂Ēl���L�^
                        value = max * (nob_miss.rectTransform.anchoredPosition.x / SLIDER_MAX);
                        value = Truncate(value, 2);
                        pValue_miss = value;
                        // �C���v�b�g�t�B�[���h�ɔ��f
                        inputField_miss.text = value.ToString();
                    }
                }
                // �l���Ȃ����
                else
                {
                    // ���O�̒l��ݒ肷��
                    inputField_miss.text = Truncate(pValue_miss, 2).ToString();
                }
                break;

            // Disappear
            case 3:
                // �C���v�b�g�t�B�[���h�ɒl�������
                if(float.TryParse(inputField_disappear.text, out var disappearValue))
                {
                    // �l���L�^
                    pValue_disappear = disappearValue;

                    // �e�L�X�g�ɔ��f����
                    value = Truncate(disappearValue, 1);
                    label_disappear.text = value.ToString();
                }
                // �l���Ȃ����
                else
                {
                    // �n�߂ĂȂ�
                    if (pValue_disappear == -1f)
                    {
                        // �l���L�^
                        pValue_disappear = DISAPPEAR_START;

                        // �����l��ݒ肷��
                        inputField_disappear.text = Truncate(DISAPPEAR_START, 2).ToString();
                        label_disappear.text = Truncate(DISAPPEAR_START, 1).ToString();
                    }
                    // �ȑO�ɓ��͂������Ƃ������
                    else
                    {
                        // ���O�̒l��ݒ肷��
                        inputField_disappear.text = Truncate(pValue_disappear, 2).ToString();
                        label_disappear.text = Truncate(pValue_disappear, 1).ToString();
                    }
                }

                // �S�̂ɉe�����邽�߁A�e����̒l��ύX����
                SliderIntoInputField(0);
                SliderIntoInputField(1);
                SliderIntoInputField(2);

                break;
        }
    }

    /// <summary>
    /// ������n�ʂŐ؂�̂�
    /// </summary>
    /// <param name="number">����</param>
    /// <param name="decimalPlace">n</param>
    /// <returns></returns>
    private float Truncate(float number, int decimalPlace)
    {
        for(int i = 0; i < decimalPlace; i++)
        {
            number *= 10f;
        }
        number = Mathf.Floor(number);
        for (int i = 0; i < decimalPlace; i++)
        {
            number /= 10f;
        }
        return number;
    }
}

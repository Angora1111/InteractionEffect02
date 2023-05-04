using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DecideFrameWindow : MonoBehaviour
{
    private const float SLIDER_MAX = 750.0f;
    private const int DISAPPEAR_START = 20;
    private const int DISAPPEAR_MIN = 4;
    private const float FPS = 60f;
    private const float NOTESPEED = 30.0f;

    // �e�C���v�b�g�t�B�[���h�́A���O�̒l
    private int pValue_perfect = -1;
    private int pValue_good = -1;
    private int pValue_miss = -1;
    private int pValue_disappear = -1;
    // ���O�̑S�̂̒l
    private int pValue_c_perfect = 0;
    private int pValue_c_good = 0;
    private int pValue_c_miss = 0;
    private int pValue_c_neglect = 0;

    [Header("�m�u")]
    [SerializeField] FrameBarNob nob_perfect;
    [SerializeField] FrameBarNob nob_good, nob_miss;
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

    private void OnEnable()
    {
        // �S�̂̒l�ɐݒ肷��
        CommonData.perfectDist = pValue_c_perfect / FPS * NOTESPEED;
        CommonData.goodDist = pValue_c_good / FPS * NOTESPEED;
        CommonData.missDist = pValue_c_miss / FPS * NOTESPEED;
        CommonData.neglectDist = pValue_c_neglect / FPS * NOTESPEED;

        // �K�C�h���C���ɔ��f����
        guideline.Draw();
    }

    void Update()
    {
        // �C���v�b�g�t�B�[���h���S�ē��͂���Ă����
        if (int.TryParse(inputField_perfect.text, out var value))
        {
            if (int.TryParse(inputField_good.text, out var _value))
            {
                if (int.TryParse(inputField_miss.text, out var __value))
                {
                    if (int.TryParse(label_disappear.text, out var ___value))
                    {
                        // �l���ύX����Ă���Δ��f����
                        if (pValue_c_perfect != value
                           || pValue_c_good != _value
                           || pValue_c_miss != __value
                           || pValue_c_neglect != ___value)
                        {
                            // �S�̂̒l�ɐݒ肷��
                            CommonData.perfectDist = value / FPS * NOTESPEED;
                            CommonData.goodDist = _value / FPS * NOTESPEED;
                            CommonData.missDist = __value / FPS * NOTESPEED;
                            CommonData.neglectDist = ___value / FPS * NOTESPEED;

                            // �K�C�h���C���ɔ��f����
                            guideline.Draw();

                            // �l��ۑ�����
                            pValue_c_perfect = value;
                            pValue_c_good = _value;
                            pValue_c_miss = __value;
                            pValue_c_neglect = ___value;
                        }
                    }
                }
            }
        }
    }

    // �X���C�_�[�̒l���C���v�b�g�t�B�[���h�ɔ��f����
    public void SliderIntoInputField(int trigger)
    {
        int max = 0;
        int value = 0;
        switch (trigger)
        {
            // PERFECT
            case 0:
                max = int.Parse(label_disappear.text);
                value = Mathf.RoundToInt(max * (nob_perfect.rectTransform.anchoredPosition.x / SLIDER_MAX));
                inputField_perfect.text = value.ToString();

                // �l���L�^
                pValue_perfect = value;
                break;

            // GOOD
            case 1:
                max = int.Parse(label_disappear.text);
                value = Mathf.RoundToInt(max * (nob_good.rectTransform.anchoredPosition.x / SLIDER_MAX));
                inputField_good.text = value.ToString();

                // �l���L�^
                pValue_good = value;
                break;

            // MISS
            case 2:
                max = int.Parse(label_disappear.text);
                value = Mathf.RoundToInt(max * (nob_miss.rectTransform.anchoredPosition.x / SLIDER_MAX));
                inputField_miss.text = value.ToString();

                // �l���L�^
                pValue_miss = value;
                break;
        }
        Debug.Log($"max:{max}, value:{value}");
    }

    // �C���v�b�g�t�B�[���h�̒l���X���C�_�[�ɔ��f����
    public void InputFieldIntoSlider(int trigger)
    {
        int max = 0;
        int value = 0;
        switch (trigger)
        {
            // PERFECT
            case 0:
                // �C���v�b�g�t�B�[���h�ɒl�������
                if (int.TryParse(inputField_perfect.text, out var perfectValue))
                {
                    // �X���C�_�[�ɔ��f����
                    max = int.Parse(label_disappear.text);
                    value = perfectValue;
                    nob_perfect.rectTransform.anchoredPosition = new Vector2(SLIDER_MAX * ((float)value / max), nob_perfect.rectTransform.anchoredPosition.y);

                    // �l���L�^
                    pValue_perfect = value;

                    // �m�u�̍ŏ��l/�ő�l�ȓ��Ɏ��܂�悤�ɕ␳����
                    if (nob_perfect.FixCursorPos())
                    {
                        // �␳���ꂽ�Ȃ�A���߂Ēl���L�^
                        value = Mathf.RoundToInt(max * nob_perfect.rectTransform.anchoredPosition.x / SLIDER_MAX);
                        pValue_perfect = value;
                        // �C���v�b�g�t�B�[���h�ɔ��f
                        inputField_perfect.text = value.ToString();
                    }
                }
                // �l���Ȃ����
                else
                {
                    // ���O�̒l��ݒ肷��
                    inputField_perfect.text = pValue_perfect.ToString();
                }
                break;

            // GOOD
            case 1:
                // �C���v�b�g�t�B�[���h�ɒl�������
                if (int.TryParse(inputField_good.text, out var goodValue))
                {
                    // �X���C�_�[�ɔ��f����
                    max = int.Parse(label_disappear.text);
                    value = goodValue;
                    nob_good.rectTransform.anchoredPosition = new Vector2(SLIDER_MAX * ((float)value / max), nob_good.rectTransform.anchoredPosition.y);

                    // �l���L�^
                    pValue_good = value;

                    // �m�u�̍ŏ��l/�ő�l�ȓ��Ɏ��܂�悤�ɕ␳����
                    if (nob_good.FixCursorPos())
                    {
                        // �␳���ꂽ�Ȃ�A���߂Ēl���L�^
                        value = Mathf.RoundToInt(max * nob_good.rectTransform.anchoredPosition.x / SLIDER_MAX);
                        pValue_good = value;
                        // �C���v�b�g�t�B�[���h�ɔ��f
                        inputField_good.text = value.ToString();
                    }
                }
                // �l���Ȃ����
                else
                {
                    // ���O�̒l��ݒ肷��
                    inputField_good.text = pValue_good.ToString();
                }
                break;

            // MISS
            case 2:
                // �C���v�b�g�t�B�[���h�ɒl�������
                if (int.TryParse(inputField_miss.text, out var missValue))
                {
                    // �X���C�_�[�ɔ��f����
                    max = int.Parse(label_disappear.text);
                    value = missValue;
                    nob_miss.rectTransform.anchoredPosition = new Vector2(SLIDER_MAX * ((float)value / max), nob_miss.rectTransform.anchoredPosition.y);

                    // �l���L�^
                    pValue_miss = value;

                    // �m�u�̍ŏ��l/�ő�l�ȓ��Ɏ��܂�悤�ɕ␳����
                    if (nob_miss.FixCursorPos())
                    {
                        // �␳���ꂽ�Ȃ�A���߂Ēl���L�^
                        value = Mathf.RoundToInt(max * nob_miss.rectTransform.anchoredPosition.x / SLIDER_MAX);
                        pValue_miss = value;
                        // �C���v�b�g�t�B�[���h�ɔ��f
                        inputField_miss.text = value.ToString();
                    }
                }
                // �l���Ȃ����
                else
                {
                    // ���O�̒l��ݒ肷��
                    inputField_miss.text = pValue_miss.ToString();
                }
                break;

            // Disappear
            case 3:
                // �C���v�b�g�t�B�[���h�ɒl�������
                if (int.TryParse(inputField_disappear.text, out var disappearValue))
                {
                    // �l���ŏ��l��菬������Ε␳����
                    if (disappearValue < DISAPPEAR_MIN)
                    {
                        disappearValue = DISAPPEAR_MIN;
                        inputField_disappear.text = DISAPPEAR_MIN.ToString();
                    }

                    // �l���L�^
                    pValue_disappear = disappearValue;

                    // �e�L�X�g�ɔ��f����
                    value = disappearValue;
                    label_disappear.text = value.ToString();
                }
                // �l���Ȃ����
                else
                {
                    // �n�߂ĂȂ�
                    if (pValue_disappear == -1)
                    {
                        // �l���L�^
                        pValue_disappear = DISAPPEAR_START;

                        // �����l��ݒ肷��
                        inputField_disappear.text = DISAPPEAR_START.ToString();
                        label_disappear.text = DISAPPEAR_START.ToString();
                    }
                    // �ȑO�ɓ��͂������Ƃ������
                    else
                    {
                        // ���O�̒l��ݒ肷��
                        inputField_disappear.text = pValue_disappear.ToString();
                        label_disappear.text = pValue_disappear.ToString();
                    }
                }

                // �S�̂ɉe�����邽�߁A�e����̒l��ύX����
                SliderIntoInputField(0);
                SliderIntoInputField(1);
                SliderIntoInputField(2);

                break;
        }
    }
}

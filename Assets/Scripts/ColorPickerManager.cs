using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorPickerManager : MonoBehaviour
{
    [SerializeField] Image colorImage;//�F�̕\���p�l��
    [SerializeField] Slider hueSlider;//�F���̃X���C�_�[
    [SerializeField] Slider saturationSlider;//�ʓx�̃X���C�_�[
    [SerializeField] Slider valueSlider;//���x�̃X���C�_�[
    [SerializeField] Image hueImage;//�F���̂܂݂̉摜
    [SerializeField] Image saturationImage;//�ʓx�̂܂݂̉摜
    [SerializeField] Image valueImage;//���x�̂܂݂̉摜
    [SerializeField] InputField inputR;//R�̒l������InputField
    [SerializeField] InputField inputG;//G�̒l������InputField
    [SerializeField] InputField inputB;//B�̒l������InputField
    [SerializeField] Toggle toggleRGB;//RGB�ŌŒ肷�邩�ǂ�����\���`�F�b�N�{�b�N�X
    [SerializeField] Image saturationBackImage;//�ʓx�̃X���C�_�[�̊�{�F
    private ColorImageData targetCID = default;//�F��ύX����Image�̃f�[�^
    private int pvalueR;//�ύX�O��R�̒l
    private int pvalueG;//�ύX�O��G�̒l
    private int pvalueB;//�ύX�O��B�̒l

    /// <summary>
    /// �X���C�_�[�̒l����F��ς���
    /// </summary>
    public void SetColorFromSlider()
    {
        if (!toggleRGB.isOn)
        {
            //�X���C�_�[�ɂ��HSV�̒l����RGB�ɕϊ�����
            float hue_value = hueSlider.value;
            float saturation_value = saturationSlider.value;
            float value_value = valueSlider.value;
            Color showColor = Color.HSVToRGB(hue_value, saturation_value, value_value);
            //�\�����Ă���F��RGB�̒l�ɔ��f������
            colorImage.color = showColor;
            saturationBackImage.color = Color.HSVToRGB(hue_value, 1f, 1f);
            inputR.text = ((int)(showColor.r * 255)).ToString();
            inputG.text = ((int)(showColor.g * 255)).ToString();
            inputB.text = ((int)(showColor.b * 255)).ToString();
            ChangeNobColor(hue_value, saturation_value, value_value);
            //RGB�̒l��ۑ����Ă���
            pvalueR = (int)(showColor.r * 255);
            pvalueG = (int)(showColor.g * 255);
            pvalueB = (int)(showColor.b * 255);
        }
    }

    /// <summary>
    /// �C���v�b�g�t�B�[���h�̒l����F��ς���
    /// </summary>
    public void SetColorFromInputField()
    {
        if (int.TryParse(inputR.text, out int inputValueR))
        {
            if (int.TryParse(inputG.text, out int inputValueG))
            {
                if (int.TryParse(inputB.text, out int inputValueB))
                {
                    //���͂��ꂽ�l��␳����
                    inputValueR = CorrectInputValue(inputValueR);
                    inputValueG = CorrectInputValue(inputValueG);
                    inputValueB = CorrectInputValue(inputValueB);
                    inputR.text = inputValueR.ToString();
                    inputG.text = inputValueG.ToString();
                    inputB.text = inputValueB.ToString();
                    //�C���v�b�g�t�B�[���h�ɂ��RGB�̒l����HSV�ɕϊ�����
                    Color inputColor_rgb = new Color(inputValueR / 255f, inputValueG / 255f, inputValueB / 255f);
                    Color.RGBToHSV(inputColor_rgb, out float inputHue, out float inputSaturation, out float inputValue);
                    //�\�����Ă���F�ɔ��f����
                    colorImage.color = inputColor_rgb;
                    saturationBackImage.color = Color.HSVToRGB(inputHue, 1f, 1f);
                    //�X���C�_�[�̒l�ɔ��f������
                    hueSlider.value = inputHue;
                    saturationSlider.value = inputSaturation;
                    valueSlider.value = inputValue;
                    ChangeNobColor(inputHue, inputSaturation, inputValue);
                    //RGB�̒l��ۑ����Ă���
                    pvalueR = inputValueR;
                    pvalueG = inputValueG;
                    pvalueB = inputValueB;
                    return;
                }
            }
        }
        //�ϊ��ł��Ȃ����1�O�̒l�ɖ߂�
        inputR.text = pvalueR.ToString();
        inputG.text = pvalueG.ToString();
        inputB.text = pvalueB.ToString();
        return;
    }

    /// <summary>
    /// �X���C�_�[�̂܂݂̐F��ς���
    /// </summary>
    private void ChangeNobColor(float argHue_value, float argSaturation_value, float argValue_value)
    {
        //�F��
        hueImage.color = Color.HSVToRGB(argHue_value, 1f, 1f);
        //�ʓx
        saturationImage.color = Color.HSVToRGB(argHue_value, argSaturation_value, 1f);
        //���x
        valueImage.color = new Color(argValue_value, argValue_value, argValue_value);
    }

    /// <summary>
    /// ColorPreview��Ή��t������
    /// </summary>
    /// <param name="argCID">�Ώۂ�ColorPreview</param>
    public void TargetImageSetting(ColorImageData argCID)
    {
        targetCID = argCID;
        colorImage.color = targetCID.GetImageObj().color;
        toggleRGB.isOn = targetCID.GetIsSetAsRGB();

        //�\�����Ă���F��RGB�̒l�ɔ��f������
        inputR.text = ((int)(colorImage.color.r * 255)).ToString();
        inputG.text = ((int)(colorImage.color.g * 255)).ToString();
        inputB.text = ((int)(colorImage.color.b * 255)).ToString();

        //�X���C�_�[�ɔ��f����
        SetColorFromInputField();
    }

    /// <summary>
    /// ColorPreview�ɔ��f������
    /// </summary>
    public void TargetImageChange()
    {
        targetCID.GetImageObj().color = colorImage.color;
        targetCID.SetIsSetAsRGB(toggleRGB.isOn);
    }

    /// <summary>
    /// int�^�̒l��0�`255�̒l�Ɋۂߍ���
    /// </summary>
    /// <param name="argValue">�␳����l</param>
    /// <returns></returns>
    private int CorrectInputValue(int argValue)
    {
        argValue = Mathf.Max(0, argValue);
        argValue = Mathf.Min(255, argValue);
        return argValue;
    }
}

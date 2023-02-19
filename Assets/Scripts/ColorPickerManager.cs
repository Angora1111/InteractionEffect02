using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorPickerManager : MonoBehaviour
{
    [SerializeField] Image colorImage;//色の表示パネル
    [SerializeField] Slider hueSlider;//色相のスライダー
    [SerializeField] Slider saturationSlider;//彩度のスライダー
    [SerializeField] Slider valueSlider;//明度のスライダー
    [SerializeField] Image hueImage;//色相のつまみの画像
    [SerializeField] Image saturationImage;//彩度のつまみの画像
    [SerializeField] Image valueImage;//明度のつまみの画像
    [SerializeField] InputField inputR;//Rの値を示すInputField
    [SerializeField] InputField inputG;//Gの値を示すInputField
    [SerializeField] InputField inputB;//Bの値を示すInputField
    [SerializeField] Toggle toggleRGB;//RGBで固定するかどうかを表すチェックボックス
    [SerializeField] Image saturationBackImage;//彩度のスライダーの基本色
    private ColorImageData targetCID = default;//色を変更するImageのデータ
    private int pvalueR;//変更前のRの値
    private int pvalueG;//変更前のGの値
    private int pvalueB;//変更前のBの値

    /// <summary>
    /// スライダーの値から色を変える
    /// </summary>
    public void SetColorFromSlider()
    {
        if (!toggleRGB.isOn)
        {
            //スライダーによるHSVの値からRGBに変換する
            float hue_value = hueSlider.value;
            float saturation_value = saturationSlider.value;
            float value_value = valueSlider.value;
            Color showColor = Color.HSVToRGB(hue_value, saturation_value, value_value);
            //表示している色とRGBの値に反映させる
            colorImage.color = showColor;
            saturationBackImage.color = Color.HSVToRGB(hue_value, 1f, 1f);
            inputR.text = ((int)(showColor.r * 255)).ToString();
            inputG.text = ((int)(showColor.g * 255)).ToString();
            inputB.text = ((int)(showColor.b * 255)).ToString();
            ChangeNobColor(hue_value, saturation_value, value_value);
            //RGBの値を保存しておく
            pvalueR = (int)(showColor.r * 255);
            pvalueG = (int)(showColor.g * 255);
            pvalueB = (int)(showColor.b * 255);
        }
    }

    /// <summary>
    /// インプットフィールドの値から色を変える
    /// </summary>
    public void SetColorFromInputField()
    {
        if (int.TryParse(inputR.text, out int inputValueR))
        {
            if (int.TryParse(inputG.text, out int inputValueG))
            {
                if (int.TryParse(inputB.text, out int inputValueB))
                {
                    //入力された値を補正する
                    inputValueR = CorrectInputValue(inputValueR);
                    inputValueG = CorrectInputValue(inputValueG);
                    inputValueB = CorrectInputValue(inputValueB);
                    inputR.text = inputValueR.ToString();
                    inputG.text = inputValueG.ToString();
                    inputB.text = inputValueB.ToString();
                    //インプットフィールドによるRGBの値からHSVに変換する
                    Color inputColor_rgb = new Color(inputValueR / 255f, inputValueG / 255f, inputValueB / 255f);
                    Color.RGBToHSV(inputColor_rgb, out float inputHue, out float inputSaturation, out float inputValue);
                    //表示している色に反映する
                    colorImage.color = inputColor_rgb;
                    saturationBackImage.color = Color.HSVToRGB(inputHue, 1f, 1f);
                    //スライダーの値に反映させる
                    hueSlider.value = inputHue;
                    saturationSlider.value = inputSaturation;
                    valueSlider.value = inputValue;
                    ChangeNobColor(inputHue, inputSaturation, inputValue);
                    //RGBの値を保存しておく
                    pvalueR = inputValueR;
                    pvalueG = inputValueG;
                    pvalueB = inputValueB;
                    return;
                }
            }
        }
        //変換できなければ1つ前の値に戻す
        inputR.text = pvalueR.ToString();
        inputG.text = pvalueG.ToString();
        inputB.text = pvalueB.ToString();
        return;
    }

    /// <summary>
    /// スライダーのつまみの色を変える
    /// </summary>
    private void ChangeNobColor(float argHue_value, float argSaturation_value, float argValue_value)
    {
        //色相
        hueImage.color = Color.HSVToRGB(argHue_value, 1f, 1f);
        //彩度
        saturationImage.color = Color.HSVToRGB(argHue_value, argSaturation_value, 1f);
        //明度
        valueImage.color = new Color(argValue_value, argValue_value, argValue_value);
    }

    /// <summary>
    /// ColorPreviewを対応付けする
    /// </summary>
    /// <param name="argCID">対象のColorPreview</param>
    public void TargetImageSetting(ColorImageData argCID)
    {
        targetCID = argCID;
        colorImage.color = targetCID.GetImageObj().color;
        toggleRGB.isOn = targetCID.GetIsSetAsRGB();

        //表示している色とRGBの値に反映させる
        inputR.text = ((int)(colorImage.color.r * 255)).ToString();
        inputG.text = ((int)(colorImage.color.g * 255)).ToString();
        inputB.text = ((int)(colorImage.color.b * 255)).ToString();

        //スライダーに反映する
        SetColorFromInputField();
    }

    /// <summary>
    /// ColorPreviewに反映させる
    /// </summary>
    public void TargetImageChange()
    {
        targetCID.GetImageObj().color = colorImage.color;
        targetCID.SetIsSetAsRGB(toggleRGB.isOn);
    }

    /// <summary>
    /// int型の値を0～255の値に丸め込む
    /// </summary>
    /// <param name="argValue">補正する値</param>
    /// <returns></returns>
    private int CorrectInputValue(int argValue)
    {
        argValue = Mathf.Max(0, argValue);
        argValue = Mathf.Min(255, argValue);
        return argValue;
    }
}

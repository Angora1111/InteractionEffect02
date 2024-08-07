using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorImageData : MonoBehaviour
{
    private bool isSetAsRGB = true;
    private Image colorImage;

    private void Awake()
    {
        colorImage = GetComponent<Image>();
    }

    /// <summary>
    /// RGBで固定するかどうかを設定する
    /// </summary>
    /// <param name="argBool">設定後のbool値</param>
    public void SetIsSetAsRGB(bool argBool) { isSetAsRGB = argBool; }

    /// <summary>
    /// RGBで固定するかどうかのbool値を取得する
    /// </summary>
    /// <returns>現在のbool値</returns>
    public bool GetIsSetAsRGB() { return isSetAsRGB; }

    /// <summary>
    /// Imageを取得する
    /// </summary>
    /// <returns>対応するImage</returns>
    public Image GetImageObj() { return colorImage; }
}

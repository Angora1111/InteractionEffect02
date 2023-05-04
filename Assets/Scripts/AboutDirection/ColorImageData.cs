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
    /// RGB�ŌŒ肷�邩�ǂ�����ݒ肷��
    /// </summary>
    /// <param name="argBool">�ݒ���bool�l</param>
    public void SetIsSetAsRGB(bool argBool) { isSetAsRGB = argBool; }

    /// <summary>
    /// RGB�ŌŒ肷�邩�ǂ�����bool�l���擾����
    /// </summary>
    /// <returns>���݂�bool�l</returns>
    public bool GetIsSetAsRGB() { return isSetAsRGB; }

    /// <summary>
    /// Image���擾����
    /// </summary>
    /// <returns>�Ή�����Image</returns>
    public Image GetImageObj() { return colorImage; }
}

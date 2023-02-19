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
    /// RGB‚ÅŒÅ’è‚·‚é‚©‚Ç‚¤‚©‚ğİ’è‚·‚é
    /// </summary>
    /// <param name="argBool">İ’èŒã‚Ìbool’l</param>
    public void SetIsSetAsRGB(bool argBool) { isSetAsRGB = argBool; }

    /// <summary>
    /// RGB‚ÅŒÅ’è‚·‚é‚©‚Ç‚¤‚©‚Ìbool’l‚ğæ“¾‚·‚é
    /// </summary>
    /// <returns>Œ»İ‚Ìbool’l</returns>
    public bool GetIsSetAsRGB() { return isSetAsRGB; }

    /// <summary>
    /// Image‚ğæ“¾‚·‚é
    /// </summary>
    /// <returns>‘Î‰‚·‚éImage</returns>
    public Image GetImageObj() { return colorImage; }
}
